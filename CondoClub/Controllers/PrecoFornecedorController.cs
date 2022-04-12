using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CondoClub.Web.Controllers {

    public class PrecoFornecedorController : Controller {

        #region Accoes


        public ActionResult Index() {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.PrecoFornecedor.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);
                return View(ConstroiLista(null, null));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }

        public PartialViewResult _Lista(int? paisID, int? opcaoPagamentoID) {
            try {
                if (!Regras.PrecoFornecedor.Permissoes(ControladorSite.Utilizador).Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                return PartialView(ConstroiLista(paisID, opcaoPagamentoID));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        public PartialViewResult _DetalheVisualizar(long id) {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.PrecoFornecedor.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.PrecoFornecedor model = new Models.PrecoFornecedor(
                    new Regras.PrecoFornecedor().Abrir(id, ControladorSite.Utilizador)
                );

                return PartialView("_DetalheVisualizar", model);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        public PartialViewResult _DetalheEditar(long id) {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.PrecoFornecedor.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Gravar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.PrecoFornecedor model = new Models.PrecoFornecedor(
                    new Regras.PrecoFornecedor().Abrir(id, ControladorSite.Utilizador)
                );

                return PartialView("_DetalheEditar", model);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        [HttpPost]
        public ActionResult Gravar(Models.PrecoFornecedor registo) {
            try {
                if (ModelState.IsValid) {

                    Regras.PrecoFornecedor regras = new Regras.PrecoFornecedor();

                    if (!registo.ID.HasValue) {
                        Regras.BD.PrecoFornecedor bdModel = registo.ToBDModel();
                        regras.Inserir(bdModel, ControladorSite.Utilizador);
                        List<Models.PrecoFornecedor> modelList = new List<Models.PrecoFornecedor>();
                        modelList.Add(
                            new Models.PrecoFornecedor(regras.Abrir(bdModel.PrecoFornecedorID, ControladorSite.Utilizador)
                        ));
                        return PartialView("_Lista", modelList);

                    } else {
                        Regras.BD.PrecoFornecedor bdModel = registo.ToBDModel();
                        new Regras.PrecoFornecedor().Actualizar(bdModel, ControladorSite.Utilizador);
                        return PartialView("_DetalheVisualizar", new Models.PrecoFornecedor(
                            regras.Abrir(bdModel.PrecoFornecedorID, ControladorSite.Utilizador)
                        ));
                    }
                } else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            } catch (Regras.Exceptions.PrecoRepetidoRange) {
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.PrecoRepetidoRange));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }


        [HttpDelete]
        public ActionResult Apagar(long id) {
            try {
                if (id > 0) {
                    new Regras.PrecoFornecedor().Apagar(id, ControladorSite.Utilizador);
                    return JavaScript("AfterDelete(" + id + ");");
                } else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }


        #endregion


        #region Funcões Auxiliares

        public IEnumerable<Models.PrecoFornecedor> ConstroiLista(int? paisID, int? opcaoPagamentoID) {

            IEnumerable<Regras.BD.PrecoFornecedor> listaObj = new Regras.PrecoFornecedor().Lista(paisID, opcaoPagamentoID, ControladorSite.Utilizador);

            List<Models.PrecoFornecedor> modelList = new List<Models.PrecoFornecedor>();
            foreach (var obj in listaObj) {
                modelList.Add(new Models.PrecoFornecedor(obj));
            }

            return modelList.OrderBy(x => x.PaisID).ThenBy(x => x.OpcaoPagamentoID).ThenBy(x => x.FraccoesAte);
        }

        #endregion

    }
}
