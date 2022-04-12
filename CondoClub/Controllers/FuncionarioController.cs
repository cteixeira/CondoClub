using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CondoClub.Web.Controllers
{
    [Authorize]
    public class FuncionarioController : Controller
    {

        #region Acções

        public ViewResult Index()
        {
            try
            {
                List<Regras.Enum.Permissao> permissoes = Regras.Funcionario.Permissoes(ControladorSite.Utilizador);

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
                if (!Regras.Funcionario.Permissoes(ControladorSite.Utilizador).Contains(Regras.Enum.Permissao.Visualizar))
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
                List<Regras.Enum.Permissao> permissoes = Regras.Funcionario.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.Funcionario model = new Models.Funcionario(
                    new Regras.Funcionario().Abrir(id, ControladorSite.Utilizador)
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
                List<Regras.Enum.Permissao> permissoes = Regras.Funcionario.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Gravar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.Funcionario model = new Models.Funcionario(
                    new Regras.Funcionario().Abrir(id, ControladorSite.Utilizador)
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
        public ActionResult Gravar(Models.Funcionario registo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!registo.ID.HasValue)
                    {
                        Regras.BD.Funcionario bdModel = registo.ToBDModel();
                        new Regras.Funcionario().Inserir(bdModel, ControladorSite.Utilizador);
                        List<Models.Funcionario> modelList = new List<Models.Funcionario>();
                        modelList.Add(new Models.Funcionario(bdModel));
                        return PartialView("_Lista", modelList);
                    }
                    else
                    {
                        Regras.BD.Funcionario bdModel = registo.ToBDModel();
                        new Regras.Funcionario().Actualizar(bdModel, ControladorSite.Utilizador);
                        return PartialView("_DetalheVisualizar", new Models.Funcionario(bdModel));
                    }
                }
                else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            }
            catch (Regras.Exceptions.NomeRepetido)
            {
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.NomeRepetido));
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
                    new Regras.Funcionario().Apagar(id, ControladorSite.Utilizador);
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

        public IEnumerable<Models.Funcionario> ConstroiLista(string nome)
        {
            IEnumerable<Regras.BD.Funcionario> listaObj = new Regras.Funcionario().Lista(
                nome, ControladorSite.Utilizador);

            List<Models.Funcionario> modelList = new List<Models.Funcionario>();
            foreach (var obj in listaObj)
            {
                modelList.Add(new Models.Funcionario(obj));
            }

            return modelList.OrderBy(x => x.Nome);
        }


        public SelectList ConstroiDropDown()
        {
            IEnumerable<Models.Funcionario> lista = ConstroiLista(null);
            if (lista.Count() == 1)
                return new SelectList(lista, "ID", "Nome", lista.First().ID);
            else
                return new SelectList(lista, "ID", "Nome");
        }

        #endregion

    }
}
