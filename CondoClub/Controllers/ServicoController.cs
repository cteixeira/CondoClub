using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web;

namespace CondoClub.Web.Controllers
{
    [Authorize]
    public class ServicoController : Controller
    {
        private const int nrServicosIni = 10;
        private const int nrServicosNext = 5;


        #region Acções


        public ViewResult Index() {
            try {
                List<Regras.Fornecedor.Permissao> permissoes = Regras.Fornecedor.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Fornecedor.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);
                ViewData.Add("nrServicosIni", nrServicosIni);
                ViewData.Add("nrServicosNext", nrServicosNext);

                return View(ListaCategoria());
            }
            catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        public ViewResult Pesquisa(string termoPesquisa) {
            try {
                List<Regras.Fornecedor.Permissao> permissoes = Regras.Fornecedor.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Fornecedor.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);
                ViewData.Add("TermoPesquisa", termoPesquisa);
                ViewData.Add("nrServicosIni", nrServicosIni);

                return View(ListaServico(termoPesquisa, 0, nrServicosIni));
            }
            catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        public ViewResult Fornecedor(string nome) {
            try {
                List<Regras.Fornecedor.Permissao> permissoes = Regras.Fornecedor.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Fornecedor.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                return View(ConstroiDetalhe(nome));
            }
            catch (Regras.Exceptions.FornecedorNaoExiste) {
                return View("Erro", new Exception(Resources.Erro.FornecedorNaoExiste));
            }
            catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", ex);
            }
        }


        public PartialViewResult _ListaServico(string termoPesquisa, int count) {
            try {
                List<Regras.Fornecedor.Permissao> permissoes = Regras.Fornecedor.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Fornecedor.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                return PartialView(ListaServico(termoPesquisa, count, nrServicosNext));
            }
            catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        [HttpPost]
        public ActionResult NovaMensagem(long ID, string Mensagem) {
            try {
                if (!String.IsNullOrEmpty(Mensagem)) {
                    new Regras.Fornecedor().EnviarMensagem(ID, Mensagem, ControladorSite.Utilizador);
                    return JavaScript(String.Format("alert('{0}')", Resources.Servico.EnvioMensagemComSucesso));
                }
                else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            }
            catch (Regras.Exceptions.DadosIncorrectos) {
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.DadosIncorrectos));
            }
            catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }


        [HttpPost]
        public ActionResult NovaClassificacao(Models.FornecedorClassificacao registo) {
            try {
                if (ModelState.IsValid) {
                    Regras.BD.Fornecedor fornecedor = new Regras.Fornecedor()
                        .InserirClassificacao(registo.ToBDModel(), ControladorSite.Utilizador);

                    return PartialView("_Classificacao", new Models.ServicoFornecedor(fornecedor));
                }
                else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            }
            catch (Regras.Exceptions.DadosIncorrectos) {
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.DadosIncorrectos));
            }
            catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }


        [HttpDelete]
        public ActionResult ApagarClassificacao(long id) {
            try {
                Regras.BD.Fornecedor fornecedor = new Regras.Fornecedor()
                    .ApagarClassificacao(id, ControladorSite.Utilizador);
                return PartialView("_Classificacao", new Models.ServicoFornecedor(fornecedor));
            }
            catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }

        }

        #endregion


        #region Funcões Auxiliares


        public List<Regras.Fornecedor.Categoria> ListaCategoria() {
            return Regras.Fornecedor.CategoriasDisponiveis(ControladorSite.Utilizador);
        }


        public List<Models.Servico> ListaServico(string termoPesquisa, int skip, int take) {

            IEnumerable<Regras.BD.Fornecedor> listaObj = Regras.Fornecedor.PesquisaServico(termoPesquisa, ControladorSite.Utilizador, skip, take);

            List<Models.Servico> modelList = new List<Models.Servico>();
            foreach (var obj in listaObj) {
                modelList.Add(new Models.Servico(obj));
            }

            return modelList;
        }


        public Models.ServicoFornecedor ConstroiDetalhe(string nome) 
        {
            Models.ServicoFornecedor model = new Models.ServicoFornecedor(
                new Regras.Fornecedor().Abrir(nome, ControladorSite.Utilizador)
            );

            return model;
        }


        public static string ObterCssClassificacao(short classificacao) {

            switch (classificacao) {
                case 0:
                    return "rating-star0";
                case 1:
                    return "rating-star10";
                case 2:
                    return "rating-star20";
                case 3:
                    return "rating-star30";
                case 4:
                    return "rating-star40";
                case 5:
                    return "rating-star50";
                default:
                    return "rating-star0";
            }
        }

        #endregion

    }

}