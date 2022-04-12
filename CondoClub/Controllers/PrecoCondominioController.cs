using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CondoClub.Web.Controllers {

    public class PrecoCondominioController : Controller {

        #region Accoes


        public ActionResult Index() {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.PrecoCondominio.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);
                return View(ConstroiLista(null, null, null));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }

        public PartialViewResult _Lista(int? paisID, int? opcaoPagamentoID, int? extratoSocialID) {
            try {
                if (!Regras.PrecoCondominio.Permissoes(ControladorSite.Utilizador).Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                return PartialView(ConstroiLista(paisID, opcaoPagamentoID, extratoSocialID));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        public PartialViewResult _DetalheVisualizar(long id) {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.PrecoCondominio.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.PrecoCondominio model = new Models.PrecoCondominio(
                    new Regras.PrecoCondominio().Abrir(id, ControladorSite.Utilizador)
                );

                return PartialView("_DetalheVisualizar", model);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        public PartialViewResult _DetalheEditar(long id) {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.PrecoCondominio.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Gravar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.PrecoCondominio model = new Models.PrecoCondominio(
                    new Regras.PrecoCondominio().Abrir(id, ControladorSite.Utilizador)
                );

                return PartialView("_DetalheEditar", model);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        [HttpPost]
        public ActionResult Gravar(Models.PrecoCondominio registo) {
            try {
                if (ModelState.IsValid) {

                    Regras.PrecoCondominio regras = new Regras.PrecoCondominio();

                    if (!registo.ID.HasValue) {
                        Regras.BD.PrecoCondominio bdModel = registo.ToBDModel();
                        regras.Inserir(bdModel, ControladorSite.Utilizador);
                        List<Models.PrecoCondominio> modelList = new List<Models.PrecoCondominio>();
                        modelList.Add(
                            new Models.PrecoCondominio(regras.Abrir(bdModel.PrecoCondominioID, ControladorSite.Utilizador)
                        ));
                        return PartialView("_Lista", modelList);

                    } else {
                        Regras.BD.PrecoCondominio bdModel = registo.ToBDModel();
                        new Regras.PrecoCondominio().Actualizar(bdModel, ControladorSite.Utilizador);
                        return PartialView("_DetalheVisualizar", new Models.PrecoCondominio(
                            regras.Abrir(bdModel.PrecoCondominioID, ControladorSite.Utilizador)
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
                    new Regras.PrecoCondominio().Apagar(id, ControladorSite.Utilizador);
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

        public IEnumerable<Models.PrecoCondominio> ConstroiLista(int? paisID, int? opcaoPagamentoID, int? extratoSocialID) {

            IEnumerable<Regras.BD.PrecoCondominio> listaObj = new Regras.PrecoCondominio().Lista(paisID, opcaoPagamentoID, extratoSocialID, ControladorSite.Utilizador);

            List<Models.PrecoCondominio> modelList = new List<Models.PrecoCondominio>();
            foreach (var obj in listaObj) {
                modelList.Add(new Models.PrecoCondominio(obj));
            }

            return modelList.OrderBy(x => x.PaisID).ThenBy(x => x.OpcaoPagamentoID).ThenBy(x => x.ExtratoSocialID).ThenBy(x => x.FraccoesAte);
        }

        #endregion

    }
}
