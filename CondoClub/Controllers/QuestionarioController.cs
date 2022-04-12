using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

namespace CondoClub.Web.Controllers
{
    [Authorize]
    public class QuestionarioController : Controller
    {

        #region Acções

        public ViewResult Index() {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.Questionario.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);
                return View(ConstroiLista());
            }
            catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        public PartialViewResult _Lista() {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.Questionario.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                return PartialView(ConstroiLista());
            }
            catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        public PartialViewResult _DetalheVisualizar(long id) {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.Questionario.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                DateTime dataActual = DateTime.Now.Date;
                Models.Questionario model = new Models.Questionario(
                    new Regras.Questionario().Abrir(id, ControladorSite.Utilizador)
                );

                if (!model.JaRespondi && dataActual >= model.Inicio.Date && dataActual <= model.Fim.Date) {
                    return PartialView("_DetalheVotar", model);
                }
                else if (dataActual >= model.Inicio.Date) {
                    return PartialView("_DetalheResultado", model);
                }
                else {
                    return PartialView("_DetalheVisualizar", model);
                }
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        public PartialViewResult _DetalheEditar(long id) {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.Questionario.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Gravar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.Questionario model = new Models.Questionario(
                    new Regras.Questionario().Abrir(id, ControladorSite.Utilizador)
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
        public ActionResult Gravar(Models.Questionario registo) {
            try {
                if (ModelState.IsValid && ControladorSite.Utilizador.CondominioID.HasValue) {

                    Regras.BD.Questionario bdModel = registo.ToBDModel();

                    if (!registo.ID.HasValue) {
                        new Regras.Questionario().Inserir(bdModel, ControladorSite.Utilizador);

                        List<Models.Questionario> modelList = new List<Models.Questionario>();
                        modelList.Add(new Models.Questionario(bdModel));
                        return PartialView("_Lista", modelList);
                    }
                    else {
                        new Regras.Questionario().Actualizar(bdModel, ControladorSite.Utilizador);

                        DateTime dataActual = DateTime.Now.Date;
                        Models.Questionario fullModel = new Models.Questionario(
                            new Regras.Questionario().Abrir(bdModel.QuestionarioID, ControladorSite.Utilizador));

                        if (!fullModel.JaRespondi && dataActual >= fullModel.Inicio.Date && dataActual <= fullModel.Fim.Date) {
                            return PartialView("_DetalheVotar", fullModel);
                        }
                        else if (dataActual >= fullModel.Inicio.Date) {
                            return PartialView("_DetalheResultado", fullModel);
                        }
                        else {
                            return PartialView("_DetalheVisualizar", fullModel);
                        }
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
        public ActionResult Apagar(long id) {
            try {
                if (id > 0) {
                    new Regras.Questionario().Apagar(id, ControladorSite.Utilizador);
                    return JavaScript("AfterDelete(" + id + ");");
                }
                else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            }
            catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }


        [HttpPost]
        public ActionResult Resposta(long id, short opcao, string outraOpcao) {
            try {
                if (id > 0 && opcao > 0) {
                    Models.QuestionarioResposta resposta = new Models.QuestionarioResposta() { 
                        QuestionarioID = id,
                        OpcaoSeleccionada = opcao,
                        OutraOpcao = outraOpcao
                    };
                    new Regras.Questionario().InserirResposta(resposta.ToBDModel(), ControladorSite.Utilizador);

                    Models.Questionario model = new Models.Questionario(new Regras.Questionario().Abrir(id, ControladorSite.Utilizador));
                    return PartialView("_DetalheResultado", model);
                }
                else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            }
            catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }

        #endregion


        #region Funcões Auxiliares


        public IEnumerable<Models.Questionario> ConstroiLista() {

            IEnumerable<Regras.BD.Questionario> listaObj = new Regras.Questionario().Lista(ControladorSite.Utilizador);

            List<Models.Questionario> modelList = new List<Models.Questionario>();
            foreach (var obj in listaObj) 
                modelList.Add(new Models.Questionario(obj));

            return modelList;
        }


        #endregion


    }
}