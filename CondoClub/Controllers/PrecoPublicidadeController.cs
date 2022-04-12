using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CondoClub.Web.Controllers {

    public class PrecoPublicidadeController : Controller {

        #region Accoes


        public ActionResult Index() {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.PrecoPublicidade.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);
                return View(ConstroiLista(null, null));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }

        public PartialViewResult _Lista(int? paisID, int? zonaPublicidadeID) {
            try {
                if (!Regras.PrecoPublicidade.Permissoes(ControladorSite.Utilizador).Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                return PartialView(ConstroiLista(paisID, zonaPublicidadeID));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        public PartialViewResult _DetalheVisualizar(long id) {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.PrecoPublicidade.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.PrecoPublicidade model = new Models.PrecoPublicidade(
                    new Regras.PrecoPublicidade().Abrir(id, ControladorSite.Utilizador)
                );

                return PartialView("_DetalheVisualizar", model);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        public PartialViewResult _DetalheEditar(long id) {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.PrecoPublicidade.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Gravar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.PrecoPublicidade model = new Models.PrecoPublicidade(
                    new Regras.PrecoPublicidade().Abrir(id, ControladorSite.Utilizador)
                );

                return PartialView("_DetalheEditar", model);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        [HttpPost]
        public ActionResult Gravar(Models.PrecoPublicidade registo) {
            try {
                if (ModelState.IsValid) {

                    Regras.PrecoPublicidade regras = new Regras.PrecoPublicidade();

                    if (!registo.ID.HasValue) {
                        Regras.BD.PrecoPublicidade bdModel = registo.ToBDModel();
                        regras.Inserir(bdModel, ControladorSite.Utilizador);
                        List<Models.PrecoPublicidade> modelList = new List<Models.PrecoPublicidade>();
                        modelList.Add(
                            new Models.PrecoPublicidade(regras.Abrir(bdModel.PrecoPublicidadeID, ControladorSite.Utilizador)
                        ));
                        return PartialView("_Lista", modelList);

                    } else {
                        Regras.BD.PrecoPublicidade bdModel = registo.ToBDModel();
                        new Regras.PrecoPublicidade().Actualizar(bdModel, ControladorSite.Utilizador);
                        return PartialView("_DetalheVisualizar", new Models.PrecoPublicidade(
                            regras.Abrir(bdModel.PrecoPublicidadeID, ControladorSite.Utilizador)
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
                    new Regras.PrecoPublicidade().Apagar(id, ControladorSite.Utilizador);
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

        public IEnumerable<Models.PrecoPublicidade> ConstroiLista(int? paisID, int? zonaPublicidadeID) {

            IEnumerable<Regras.BD.PrecoPublicidade> listaObj = new Regras.PrecoPublicidade().Lista(paisID, zonaPublicidadeID, ControladorSite.Utilizador);

            List<Models.PrecoPublicidade> modelList = new List<Models.PrecoPublicidade>();
            foreach (var obj in listaObj) {
                modelList.Add(new Models.PrecoPublicidade(obj));
            }

            return modelList.OrderBy(x => x.PaisID).ThenBy(x => x.ZonaPublicidadeID).ThenBy(x => x.FraccoesAte);
        }

        #endregion

    }
}
