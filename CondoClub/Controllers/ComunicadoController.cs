using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web;

namespace CondoClub.Web.Controllers
{
    [Authorize]
    public class ComunicadoController : Controller
    {

        private const int nrComunicadosIni = 10;
        private const int nrComunicadosNext = 5;

        #region Acções

        public ViewResult Index() {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.Comunicado.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);
                ViewData.Add("nrComunicadosIni", nrComunicadosIni);
                ViewData.Add("nrComunicadosNext", nrComunicadosNext);

                return View(ConstroiLista(0, nrComunicadosIni));
            }
            catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        public PartialViewResult _Lista(int count) {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.Comunicado.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                return PartialView(ConstroiLista(count, nrComunicadosNext));
            }
            catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }

        
        [HttpPost]
        public ActionResult Gravar(Models.Comunicado registo) {
            try {
                if (ModelState.IsValid) {

                    /*if (fileUp != null && fileUp.ContentLength > 0 && fileUp.FileName.Contains(".")) {
                        //Grava ficheiro
                        JsonResult result = new FicheiroController().GravarImagemComunicado(new List<HttpPostedFileBase>() { fileUp });
                        registo.ImagemID = ((List<Models.Ficheiro>)result.Data)[0].ID;
                    }*/

                    long id = new Regras.Comunicado().Inserir(registo.ToBDModel(), ControladorSite.Utilizador);
                    
                    List<Models.Comunicado> modelList = new List<Models.Comunicado>();
                    modelList.Add(ConstroiDetalhe(id));

                    return PartialView("_Lista", modelList);
                    //return RedirectToAction("Index");
                }
                else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            }
            catch (Regras.Exceptions.DadosIncorrectos) {
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.DadosIncorrectos));
                //return View("Erro", new Exception(Resources.Erro.DadosIncorrectos) );
            }
            catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
                //return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        [HttpPost]
        public ActionResult GravarComentario(Models.ComunicadoComentario registo) {
            try {
                if (ModelState.IsValid && (registo.Gosto != null || !String.IsNullOrEmpty(registo.Comentario))) {
                    new Regras.Comunicado().InserirComentario(registo.ToBDModel(), ControladorSite.Utilizador);
                    return PartialView("_Detalhe", ConstroiDetalhe(registo.ComunicadoID));
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
        public ActionResult Apagar(long id) {
            try {
                new Regras.Comunicado().Apagar(id, ControladorSite.Utilizador);
                return JavaScript("DelOk(" + id.ToString() +");");
            }
            catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }

        }


        [HttpDelete]
        public ActionResult ApagarComentario(long id) {
            try {
                new Regras.Comunicado().ApagarComentario(id, ControladorSite.Utilizador);
                return JavaScript("DelComOk(" + id.ToString() + ");");
            }
            catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }

        }

        #endregion


        #region Funcões Auxiliares

        public List<Models.Comunicado> ConstroiLista(int skip, int take) {

            IEnumerable<Regras.BD.Comunicado> listaObj = Regras.Comunicado.Lista(ControladorSite.Utilizador, skip, take);

            List<Models.Comunicado> modelList = new List<Models.Comunicado>();
            foreach (var obj in listaObj) {
                modelList.Add(new Models.Comunicado(obj));
            }

            return modelList;
        }


        public Models.Comunicado ConstroiDetalhe(long id) {
            return new Models.Comunicado(new Regras.Comunicado().Abrir(id, ControladorSite.Utilizador));
        }

        #endregion

    }

}