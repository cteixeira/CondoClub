using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CondoClub.Web.Controllers
{
    [Authorize]
    public class VeiculoController : Controller
    {

        #region Acções

        public ViewResult Index()
        {
            try
            {
                List<Regras.Enum.Permissao> permissoes = Regras.Veiculo.Permissoes(ControladorSite.Utilizador);

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
                if (!Regras.Veiculo.Permissoes(ControladorSite.Utilizador).Contains(Regras.Enum.Permissao.Visualizar))
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
                List<Regras.Enum.Permissao> permissoes = Regras.Veiculo.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.Veiculo model = new Models.Veiculo(
                    new Regras.Veiculo().Abrir(id, ControladorSite.Utilizador)
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
                List<Regras.Enum.Permissao> permissoes = Regras.Veiculo.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Gravar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.Veiculo model = new Models.Veiculo(
                    new Regras.Veiculo().Abrir(id, ControladorSite.Utilizador)
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
        public ActionResult Gravar(Models.Veiculo registo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!registo.ID.HasValue)
                    {
                        Regras.BD.Veiculo bdModel = registo.ToBDModel();
                        new Regras.Veiculo().Inserir(bdModel, ControladorSite.Utilizador);
                        List<Models.Veiculo> modelList = new List<Models.Veiculo>();
                        modelList.Add(new Models.Veiculo(bdModel));

                        return PartialView("_Lista", modelList);
                    }
                    else
                    {
                        Regras.BD.Veiculo bdModel = registo.ToBDModel();
                        new Regras.Veiculo().Actualizar(bdModel, ControladorSite.Utilizador);

                        return PartialView("_DetalheVisualizar", new Models.Veiculo(bdModel));
                    }
                }
                else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            }
            catch (Regras.Exceptions.MatriculaRepetida)
            {
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.MatriculaRepetida));
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
                    new Regras.Veiculo().Apagar(id, ControladorSite.Utilizador);
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

        public IEnumerable<Models.Veiculo> ConstroiLista(string matricula)
        {
            IEnumerable<Regras.BD.Veiculo> listaObj = new Regras.Veiculo().Lista(
                matricula, ControladorSite.Utilizador);

            List<Models.Veiculo> modelList = new List<Models.Veiculo>();
            foreach (var obj in listaObj)
            {
                modelList.Add(new Models.Veiculo(obj));
            }

            return modelList.OrderBy(x => x.Matricula);
        }


        public SelectList ConstroiDropDown()
        {
            IEnumerable<Models.Veiculo> lista = ConstroiLista(null);
            if (lista.Count() == 1)
                return new SelectList(lista, "ID", "Matricula", lista.First().ID);
            else
                return new SelectList(lista, "ID", "Matricula");
        }

        #endregion

    }
}