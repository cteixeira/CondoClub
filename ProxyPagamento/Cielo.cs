using System;                                               
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Net;
using System.Xml.Linq;
using System.IO;

namespace CondoClub.ProxyPagamento {

    public class Cielo {

        public enum TipoCartao {
            Visa,
            MasterCard,
            Diners,
            Discover,
            Elo,
            Amex,
            Jcb,
            Aura
        }

        public enum EstadoTransacao {
            Criada = 0,
            Andamento = 1,
            Autenticada = 2,
            NaoAutenticada = 3,
            Autorizada = 4,
            NaoAutorizada = 5,
            Capturada = 6,
            Cancelada = 9,
            EmAutenticacao = 10,
            EmCancelamento = 12
        }

        #region campos
        
        private string urlWebService = null;
        private string numeroRegisto = null;
        private string chaveRegisto = null;

        #endregion

        #region constantes

        private const string _codigoMoeda = "986";
        private const string _codigoPais = "PT"; 

        private const string _xmlCriarTransacao =
            @"<requisicao-transacao versao=""1.2.1"" id="""" xmlns=""http://ecommerce.cbmp.com.br"">
	            <dados-ec>
		            <numero></numero>
		            <chave></chave>
	            </dados-ec>
	            <dados-pedido>
		            <numero></numero>
		            <valor></valor>
		            <moeda></moeda>
		            <data-hora></data-hora>
		            <idioma></idioma>
                    <soft-descriptor></soft-descriptor>
	            </dados-pedido>
	            <forma-pagamento>
		            <bandeira></bandeira>
		            <produto></produto>
		            <parcelas></parcelas>
	            </forma-pagamento>
	            <url-retorno></url-retorno>
	            <autorizar>3</autorizar>
	            <capturar>true</capturar>
            </requisicao-transacao>";

        private const string _xmlConsultarTransacao =
            @"<requisicao-consulta id= """"  versao=""1.2.1"" xmlns=""http://ecommerce.cbmp.com.br"">
                <tid></tid>
                <dados-ec>
                    <numero></numero>
                    <chave></chave>
                </dados-ec>
            </requisicao-consulta>";

        #endregion

        #region construtor
        
        public Cielo(string urlWebService, string numeroRegisto, string chaveRegisto) {
            if (String.IsNullOrEmpty(urlWebService)) {
                throw new ArgumentNullException("urlWebService");
            }
            if (String.IsNullOrEmpty(numeroRegisto)) {
                throw new ArgumentNullException("numeroRegisto");
            }
            if (String.IsNullOrEmpty(chaveRegisto)) {
                throw new ArgumentNullException("chaveRegisto");
            }

            this.urlWebService = urlWebService;
            this.numeroRegisto = numeroRegisto;
            this.chaveRegisto = chaveRegisto;
        }

        #endregion

        #region metodos publicos

        public void CriarTransacao(decimal valor, TipoCartao tipoCartao, string descricaoPagamento, string urlRetornoFimPagamento, 
            string numeroPedido, out string idTransacao, out string urlAutenticacaoCartao) {

            if (String.IsNullOrEmpty(urlRetornoFimPagamento)) {
                throw new ArgumentNullException("urlRetornoFimPagamento");
            }

            if (String.IsNullOrEmpty(numeroPedido)) {
                throw new ArgumentNullException("numeroPedido");
            }
            
            urlAutenticacaoCartao = string.Empty;
            idTransacao = string.Empty;

            try {
                XElement xmlData = XElement.Parse(_xmlCriarTransacao);
                string ns = xmlData.GetDefaultNamespace().NamespaceName;

                xmlData.Attribute("id").Value = Guid.NewGuid().ToString();

                XElement dadosEcElem = xmlData.Element(XName.Get("dados-ec", ns));
                dadosEcElem.Element(XName.Get("numero", ns)).Value = numeroRegisto;
                dadosEcElem.Element(XName.Get("chave", ns)).Value = chaveRegisto;

                XElement dadosPedidoElem = xmlData.Element(XName.Get("dados-pedido", ns));
                dadosPedidoElem.Element(XName.Get("numero", ns)).Value = numeroPedido.Length > 20 ? numeroPedido.Substring(0, 20) : numeroPedido;
                dadosPedidoElem.Element(XName.Get("valor", ns)).Value = valor.ToString("N2").Replace(",", "").Replace(".", "");
                dadosPedidoElem.Element(XName.Get("moeda", ns)).Value = _codigoMoeda;
                dadosPedidoElem.Element(XName.Get("data-hora", ns)).Value = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                dadosPedidoElem.Element(XName.Get("idioma", ns)).Value = _codigoPais;
                if (!String.IsNullOrEmpty(descricaoPagamento)) {
                    descricaoPagamento = System.Text.RegularExpressions.Regex.Replace(descricaoPagamento, @"[^a-zA-Z0-9]+", "");
                    dadosPedidoElem.Element(XName.Get("soft-descriptor", ns)).Value = descricaoPagamento.Length > 13 ? descricaoPagamento.Substring(0, 13) : descricaoPagamento;
                }

                XElement formaPagamentoElem = xmlData.Element(XName.Get("forma-pagamento", ns));
                formaPagamentoElem.Element(XName.Get("bandeira", ns)).Value = tipoCartao.ToString().ToLower();
                formaPagamentoElem.Element(XName.Get("produto", ns)).Value = "1";
                formaPagamentoElem.Element(XName.Get("parcelas", ns)).Value = "1";

                xmlData.Element(XName.Get("url-retorno", ns)).Value = urlRetornoFimPagamento;

                XElement xmlResponse = EnviarWebRequestMensagemCielo(xmlData);
                if (xmlResponse.Name == XName.Get("erro", ns)) {
                    //O retorno foi erro
                    string codigo = xmlResponse.Element(XName.Get("codigo", ns)).Value;
                    string mensagem = xmlResponse.Element(XName.Get("mensagem", ns)).Value;
                    throw new Excepcoes.ErroCielo(String.Concat(codigo, " - ", mensagem));
                }

                idTransacao = xmlResponse.Element(XName.Get("tid", ns)).Value;
                urlAutenticacaoCartao = xmlResponse.Element(XName.Get("url-autenticacao", ns)).Value;

            } catch (Exception) {
                throw;
            }
        }

        public EstadoTransacao ConsultarEstadoTransacao(string idTransacao) {

            if (String.IsNullOrEmpty(idTransacao)) {
                throw new ArgumentNullException("idTransacao");
            }

            try {

                XElement xmlData = XElement.Parse(_xmlConsultarTransacao);
                string ns = xmlData.GetDefaultNamespace().NamespaceName;

                xmlData.Attribute("id").Value = Guid.NewGuid().ToString();

                XElement tidElem = xmlData.Element(XName.Get("tid", ns));
                tidElem.Value = idTransacao;

                XElement dadosEcElem = xmlData.Element(XName.Get("dados-ec", ns));
                dadosEcElem.Element(XName.Get("numero", ns)).Value = numeroRegisto;
                dadosEcElem.Element(XName.Get("chave", ns)).Value = chaveRegisto;

                XElement xmlResponse = EnviarWebRequestMensagemCielo(xmlData);

                if (xmlResponse.Name == XName.Get("erro", ns)) {
                    //O retorno foi erro
                    string codigo = xmlResponse.Element(XName.Get("codigo", ns)).Value;
                    string mensagem = xmlResponse.Element(XName.Get("mensagem", ns)).Value;
                    throw new Excepcoes.ErroCielo(String.Concat(codigo, " - ", mensagem));
                }

                return (EstadoTransacao)Convert.ToInt32(xmlResponse.Element(XName.Get("status", ns)).Value);

            } catch (Exception) {
                
                throw;
            }
        }

        #endregion

        #region metodos privados

        private XElement EnviarWebRequestMensagemCielo(XElement xmlData) {
            WebRequest req = WebRequest.Create(urlWebService);
            req.Timeout = 30000;
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            using (MemoryStream ms = new MemoryStream()) {
                using (TextWriter tw = new StreamWriter(ms)) {
                    tw.Write("mensagem=");
                    XDocument xDoc = new XDocument(new XDeclaration("1.0", "ISO-8859-1", "yes"), xmlData);
                    xDoc.Save(tw, SaveOptions.DisableFormatting);
                    ms.WriteTo(req.GetRequestStream());
                }
            }

            System.Net.WebResponse response = req.GetResponse();
            return XElement.Load(response.GetResponseStream());
        }

        #endregion

    }

}
