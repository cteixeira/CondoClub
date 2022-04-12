using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

namespace CondoClub.Web.Controllers
{
    [Authorize]
    public class AgendaController : Controller
    {
        #region Acções

        public ViewResult Index()
        {
            try
            {
                List<Regras.Enum.Permissao> permissoes = Regras.Agenda.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);
                return View(ConstroiLista(null));
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        public PartialViewResult _Lista()
        {
            try
            {
                if (!Regras.Agenda.Permissoes(ControladorSite.Utilizador).Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                return PartialView(ConstroiLista(null));
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        public PartialViewResult _DetalheVisualizar(long id)
        {
            try
            {
                List<Regras.Enum.Permissao> permissoes = Regras.Agenda.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.Agenda model = new Models.Agenda(
                    new Regras.Agenda().Abrir(id, ControladorSite.Utilizador)
                );

                return PartialView("_DetalheVisualizar", model);
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        public PartialViewResult _DetalheEditar(long id)
        {
            try
            {
                List<Regras.Enum.Permissao> permissoes = Regras.Agenda.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Gravar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.Agenda model = new Models.Agenda(
                    new Regras.Agenda().Abrir(id, ControladorSite.Utilizador)
                );

                return PartialView("_DetalheEditar", model);
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }

        [HttpPost]
        public ActionResult Gravar(Models.Agenda registo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!registo.ID.HasValue)
                    {
                        Regras.BD.Agenda bdModel = registo.ToBDModel();
                        new Regras.Agenda().Inserir(bdModel, ControladorSite.Utilizador);
                        List<Models.Agenda> modelList = new List<Models.Agenda>();
                        modelList.Add(new Models.Agenda(bdModel));
                        return PartialView("_Lista", modelList);
                    }
                    else
                    {
                        Regras.BD.Agenda bdModel = registo.ToBDModel();
                        new Regras.Agenda().Actualizar(bdModel, ControladorSite.Utilizador);
                        return PartialView("_DetalheVisualizar", new Models.Agenda(bdModel));
                    }
                }
                else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            }
            catch (Regras.Exceptions.DesignacaoRepetida)
            {
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.DesignacaoRepetida));
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }

        [HttpDelete]
        public ActionResult Apagar(long id)
        {
            try
            {
                if (id > 0)
                {
                    new Regras.Agenda().Apagar(id, ControladorSite.Utilizador);
                    return JavaScript("AfterDelete(" + id + ");");
                }
                else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }

        #endregion


        #region Funcões Auxiliares

        public IEnumerable<Models.Agenda> ConstroiLista(string designacao)
        {
            IEnumerable<Regras.BD.Agenda> listaObj = new Regras.Agenda().Lista(
                designacao, ControladorSite.Utilizador);

            List<Models.Agenda> modelList = new List<Models.Agenda>();
            foreach (var obj in listaObj)
            {
                modelList.Add(new Models.Agenda(obj));
            }

            return modelList.OrderBy(x => x.Designacao);
        }

        #endregion
    }
}