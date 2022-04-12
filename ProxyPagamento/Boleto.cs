using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CondoClub.ProxyPagamento.BoletoServiceReference;

namespace CondoClub.ProxyPagamento {

    public class Boleto {

        #region campos

        private string chaveRegisto = null;

        #endregion

        public Boleto(string chaveRegisto) {
            this.chaveRegisto = chaveRegisto;
        }

        #region metodos publicos

        public BoletoResult GerarBoleto(decimal valor, string contribuinte, string nome, string endereco, string localidade, string cidade, string codigoPostal, string estado, string numeroDocumento) {

            ProxySoapClient _client = new ProxySoapClient();
            
            //Essas sao as informações do condoclub, só deverá
            //ser alterada caso o condoclub mude de conta
            BoletoServiceReference.CedenteModel cedente = new BoletoServiceReference.CedenteModel() {
                CPFCNPJ = "10.593.372/0001-22",//obrigatorio
                CEP = "59.022-600",
                Cidade = "Natal",
                Bairro = "Barro Vermelho",
                Logradouro = "Rua Almirante Nelson Fernandes",
                Nome = "Intermídia Comunicação LTDA - EPP",
                Numero = "774",
                UF = "RN",
                Complemento = "Loja 02",//obrigatorio
                Conta = "8715-7",//obrigatorio
                Agencia = "4717-1",
                CodigoCedente = "4717-1",
                Convenio = "1234567"
            };

            //Esse modelo refere-se as informaçoes do cliente.
            //SacadoModel sacado = new SacadoModel() {
            //    CPFCNPJ = "10.944.608/0001-28",//obrigatorio
            //    CEP = "59.064-000",
            //    Cidade = "Natal",
            //    Bairro = "Candelária",
            //    Logradouro = "Av. Senador Salgado Filho",
            //    Nome = "M E W COMPLEXO DESPORTIVO LTDA",
            //    Numero = "3010",
            //    UF = "RN"
            //};

            SacadoModel sacado = new SacadoModel() {
                CPFCNPJ = contribuinte,//obrigatorio
                CEP = codigoPostal,
                Cidade = cidade,
                Logradouro = endereco,
                Nome = nome,
                UF = estado
            };

            //Instrução model é o tipo de instrução que deve aparecer no boleto 
            //Protestar = 9,        Emite aviso ao sacado após N dias do vencto, e envia ao cartório após 5 dias úteis
            //NaoProtestar = 10,    Inibe protesto, quando houver instrução permanente na conta corrente
            //ImportanciaporDiaDesconto = 30,
            //ProtestoFinsFalimentares = 42,
            //ProtestarAposNDiasCorridos = 81,
            //ProtestarAposNDiasUteis = 82,
            //NaoReceberAposNDias = 91,
            //DevolverAposNDias = 92,
            //JurosdeMora = 998,
            //DescontoporDia = 999,
            List<InstrucaoModel> instructs = new List<InstrucaoModel>();
            instructs.Add(new BoletoServiceReference.InstrucaoModel() {
                //Nao receber apos 30 dias do vencimento
                Codigo = "91",
                //Numero de dias
                NDias = "30"
            });


            //TODO: Rever dados
            BoletoModel boleto = new BoletoModel() {
                NossoNumero = "2503883000",
                EspecieDocumento = "2",//DM - Duplicata Mercantil
                NumeroDocumento = numeroDocumento,//Numero de controle do documento gerado pela aplicação
                ValorBoleto = valor.ToString().Replace(",", "."),
                Vencimento = DateTime.Now.AddYears(1).ToString("MM/dd/yyyy 00:00:00"),  //TODO:Validar com o carlos
                Carteira = "18-019",
            };

            RetornoModel model = _client.GerarBoleto(boleto, cedente, sacado, instructs.ToArray(), chaveRegisto);

            if (model.Excecoes != null && model.Excecoes.Length > 0) {
                //TODO:Verificar o que deve de ser feito aqui
            }

            return new BoletoResult {
                IdTransacao = model.ID,
                ReferenciaPagamento = model.LinhaDigitavel,
                BoletoHtml = System.Web.HttpUtility.HtmlDecode(model.HtmlBoleto)
            };

        }

        public BoletoResult ImprimirBoleto(string referenciaPagamento) {
            ProxySoapClient _client = new ProxySoapClient();
            string html = _client.GetBoleto(referenciaPagamento, chaveRegisto);


            return new BoletoResult {
                IdTransacao = referenciaPagamento,
                BoletoHtml = System.Web.HttpUtility.HtmlDecode(html)

            };
        }

        #endregion

    }

    public class BoletoResult {
        public string IdTransacao { get; set; }
        public string ReferenciaPagamento { get; set; }
        public string BoletoHtml { get; set; }
    }

}
