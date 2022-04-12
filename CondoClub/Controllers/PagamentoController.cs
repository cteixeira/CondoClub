using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace CondoClub.Web.Controllers {

    [Authorize]
    public class PagamentoController : Controller {

        #region Constantes

        private const int nrPagamentosIni = 30;
        private const int nrPagamentosNext = 10;

        #endregion

        #region Acções

        public ActionResult Index() {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.Pagamento.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);
                ViewData.Add("nrPagamentosIni", nrPagamentosIni);
                ViewData.Add("nrPagamentosNext", nrPagamentosNext);

                return View(ConstroiLista(null, null, null, null, 0, nrPagamentosIni));

            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }

        public PartialViewResult _Lista(string termoPesquisa, DateTime? dataPagamentoInicio, DateTime? dataPagamentoFim, bool? pago, int? count) {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.Pagamento.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                int skip = count.HasValue ? count.Value : 0;
                //se o count é 0 então é uma nova pesquisa, carregar o numero de registos iniciais
                int take = (!count.HasValue || count.Value == 0) ? nrPagamentosIni : nrPagamentosNext;

                return PartialView("_Lista", ConstroiLista(termoPesquisa, dataPagamentoInicio, dataPagamentoFim, pago, skip, take));

            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }

        public ActionResult Validar(string idCifrado) {
            if (String.IsNullOrEmpty(idCifrado)) {
                return null;
            }
            try {

                List<Regras.Enum.Permissao> permissoes = Regras.Pagamento.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                Regras.Enum.OrigemPagamento origem;
                long id;
                DecifrarOrigemId(idCifrado, out origem, out id);

                Regras.Pagamento rPagamento = new Regras.Pagamento();

                Regras.RegistoPagamento pag = new Regras.Pagamento().Abrir(origem, id, ControladorSite.Utilizador);
                if (!Regras.Pagamento.Permissoes(ControladorSite.Utilizador, pag).Contains(Regras.PagamentoPermissao.Pagar)) {
                    throw new Regras.Exceptions.SemPermissao();
                }

                Models.PagamentoValidar model = new Models.PagamentoValidar(pag);

                return View(model);

            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }

        public ActionResult ConfirmarValidacao(Models.PagamentoValidar model) {
            try {

                if (ModelState.IsValid) {

                    string urlFinalizarPagamento = String.Concat(ConfigurationManager.AppSettings["AppURL"], Url.Action("Finalizar", "Pagamento", new { idCifrado = model.IdCifrado }));

                    string redirectToUrl = new Regras.Pagamento().ExecutarPagamento(model.Origem, model.ID, model.TipoCartao.Value, urlFinalizarPagamento, ControladorSite.Utilizador);

                    if (!String.IsNullOrEmpty(redirectToUrl)) {
                        return Redirect(redirectToUrl);
                    } else {
                        return View("Erro", new Exception("O processo de pagamento não retornou url de redirecionamento"));
                    }

                }
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
            return null;
        }

        public ActionResult Finalizar(string idCifrado) {
            if (String.IsNullOrEmpty(idCifrado)) {
                return null;
            }
            try {

                List<Regras.Enum.Permissao> permissoes = Regras.Pagamento.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                Regras.Enum.OrigemPagamento origem;
                long id;
                DecifrarOrigemId(idCifrado, out origem, out id);

                Regras.Pagamento rPagamento = new Regras.Pagamento();

                Regras.RegistoPagamento pag = new Regras.Pagamento().FinalizarPagamento(origem, id, ControladorSite.Utilizador);

                Models.PagamentoFinalizar model = new Models.PagamentoFinalizar(pag);

                return View(model);

            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }

        public ActionResult ImprimirBoleto(string idCifrado) {
            if (String.IsNullOrEmpty(idCifrado)) {
                return null;
            }
            try {

                List<Regras.Enum.Permissao> permissoes = Regras.Pagamento.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                Regras.Enum.OrigemPagamento origem;
                long id;
                DecifrarOrigemId(idCifrado, out origem, out id);

                Regras.Pagamento rPagamento = new Regras.Pagamento();

                string html = rPagamento.GerarHtmlBoleto(origem, id, ControladorSite.Utilizador);

                return Content(html, "text/html");

            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }

        public PartialViewResult _MarcarPago(string idCifrado) {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.Pagamento.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                Regras.Enum.OrigemPagamento origem;
                long id;
                DecifrarOrigemId(idCifrado, out origem, out id);

                Regras.Pagamento rPagamento = new Regras.Pagamento();
                rPagamento.MarcarPago(origem, id, ControladorSite.Utilizador);

                return PartialView("_Detalhe", new Models.Pagamento(rPagamento.Abrir(origem, id, ControladorSite.Utilizador)));

            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;

        }

        #endregion

        #region Funcões Auxiliares

        public SelectList ConstroiDropDownEstadoPagamento() {
            Dictionary<bool, string> estadoPagamento = new Dictionary<bool, string>();
            estadoPagamento.Add(true, Resources.Pagamento.Pago);
            estadoPagamento.Add(false, Resources.Pagamento.NaoPago);
            return new SelectList(
                estadoPagamento.Select(x => new { ID = x.Key, Name = x.Value }).ToList(),
                "ID",
                "Name"
            );
        }

        public SelectList ConstroiDropDownTipoCartao() {
            IEnumerable<Regras.Enum.TipoCartaoCredito> cartoes = Regras.Pagamento.ListaTipoCartaoAceite();

            return new SelectList(
                cartoes.Select(x => new { ID = x, Name = Resources.Pagamento.ResourceManager.GetString(String.Concat("TipoCartao_", x.ToString())) }).ToList(),
                "ID",
                "Name"
            );

        }

        public IEnumerable<Models.Pagamento> ConstroiLista(string termoPesquisa, DateTime? dataPagamentoInicio, DateTime? dataPagamentoFim, bool? pago, int skip, int take) {

            IEnumerable<Regras.RegistoPagamento> pagamentos = Regras.Pagamento.Lista(termoPesquisa, dataPagamentoInicio, dataPagamentoFim, pago, skip, take, ControladorSite.Utilizador);

            List<Models.Pagamento> modelList = new List<Models.Pagamento>();
            foreach (var obj in pagamentos) {
                modelList.Add(new Models.Pagamento(obj));
            }

            return modelList;
        }

        private static void DecifrarOrigemId(string idCifrado, out Regras.Enum.OrigemPagamento origem, out long identificador) {
            string idDecifrado = Regras.Util.Decifra(Regras.Util.UrlDecode(idCifrado));
            string[] idDecifradoArray = idDecifrado.Split('_');
            if (idDecifradoArray.Length != 2) {
                throw new ArgumentException("o parametro id não é válido");
            }

            origem = (Regras.Enum.OrigemPagamento)Enum.Parse(typeof(Regras.Enum.OrigemPagamento), idDecifradoArray[0]);
            identificador = Convert.ToInt64(idDecifradoArray[1]);
        }

        #endregion

    }

}
