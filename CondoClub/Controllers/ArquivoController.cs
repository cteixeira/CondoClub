using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CondoClub.Web.Controllers
{
    [Authorize]
    public class ArquivoController : Controller
    {
        #region Acções

        public ViewResult Index(string caminho)
        {
            try
            {
                List<Regras.Enum.Permissao> permissoes = Regras.ArquivoDirectoria.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                long directoriaID;
                List<Models.DirectoriaLink> caminhoDirs = ObterCaminhoDirectorias(caminho, out directoriaID);

                ViewData.Add("Permissoes", permissoes);
                ViewData.Add("Caminho", caminhoDirs);

                return View(ConstroiDirectoriaConteudo(ControladorSite.Utilizador.CondominioID.Value, directoriaID));
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        public PartialViewResult _Lista(long? directoriaID)
        {
            try
            {
                List<Regras.Enum.Permissao> permissoes = Regras.ArquivoDirectoria.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                return PartialView("_Lista", ConstroiDirectoriaConteudo(
                    ControladorSite.Utilizador.CondominioID.Value, directoriaID));
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        public PartialViewResult _DetalheVisualizarDirectoria(long id)
        {
            try
            {
                List<Regras.Enum.Permissao> permissoes = Regras.ArquivoDirectoria.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                Regras.BD.ArquivoDirectoria bdModel = new Regras.ArquivoDirectoria().Abrir(id, ControladorSite.Utilizador);

                Models.ArquivoDirectoria model = new Models.ArquivoDirectoria(bdModel);
                model.Link = ConstroiLinkDirectoria(bdModel.Nome);
                return PartialView("_DetalheVisualizarDirectoria", model);
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        public PartialViewResult _DetalheEditarDirectoria(long id)
        {
            try
            {
                List<Regras.Enum.Permissao> permissoes = Regras.ArquivoDirectoria.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Gravar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.ArquivoDirectoria model = new Models.ArquivoDirectoria(
                    new Regras.ArquivoDirectoria().Abrir(id, ControladorSite.Utilizador)
                );

                return PartialView("_DetalheEditarDirectoria", model);
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        public PartialViewResult _DetalheVisualizarFicheiro(long id)
        {
            try
            {
                List<Regras.Enum.Permissao> permissoes = Regras.ArquivoFicheiro.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                Regras.BD.ArquivoFicheiro bdModel = new Regras.ArquivoFicheiro().Abrir(id, ControladorSite.Utilizador);
                
                Models.ArquivoFicheiro model = new Models.ArquivoFicheiro(bdModel);
                model.Link = ConstroiLinkFicheiro(bdModel.FicheiroID);
                return PartialView("_DetalheVisualizarFicheiro", model);
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        public PartialViewResult _DetalheEditarFicheiro(long id)
        {
            try
            {
                List<Regras.Enum.Permissao> permissoes = Regras.ArquivoFicheiro.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Gravar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.ArquivoFicheiro model = new Models.ArquivoFicheiro(
                    new Regras.ArquivoFicheiro().Abrir(id, ControladorSite.Utilizador)
                );

                return PartialView("_DetalheEditarFicheiro", model);
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        [HttpPost]
        public ActionResult GravarDirectoria(Models.ArquivoDirectoria registo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Regras.BD.ArquivoDirectoria bdModel = registo.ToBDModel();

                    if (!registo.ID.HasValue)
                    {
                        new Regras.ArquivoDirectoria().Inserir(bdModel, ControladorSite.Utilizador);

                        Models.ArquivoDirectoria model = new Models.ArquivoDirectoria(bdModel);
                        model.Link = Request.UrlReferrer + "/" + Regras.Util.UrlEncode(model.Nome);
                        return PartialView("_Lista",
                            new Models.ConteudoDirectoria() 
                            { 
                                DirectoriaID = model.ArquivoDirectoriaPaiID,
                                Directorias = new List<Models.ArquivoDirectoria>() { model }, 
                                Ficheiros = new List<Models.ArquivoFicheiro>()
                            }
                        );
                    }
                    else
                    {
                        new Regras.ArquivoDirectoria().Actualizar(bdModel, ControladorSite.Utilizador);

                        Models.ArquivoDirectoria model = new Models.ArquivoDirectoria(bdModel);
                        model.Link = Request.UrlReferrer + "/" + Regras.Util.UrlEncode(model.Nome);
                        return PartialView("_DetalheVisualizarDirectoria", model);
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


        [HttpPost]
        public ActionResult GravarFicheiros(long directoriaID, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                List<Models.Ficheiro> ficheiros = new FicheiroController().GravarFicheiros(files, Regras.Enum.DimensaoFicheiro.Indiferente).ToList();   
                List<Regras.BD.ArquivoFicheiro> bdModels = new List<Regras.BD.ArquivoFicheiro>();

                foreach (Models.Ficheiro ficheiro in ficheiros)
                {
                    bdModels.Add(new Regras.BD.ArquivoFicheiro()
                    {
                        ArquivoDirectoriaID = directoriaID,
                        DataHora = DateTime.Now,
                        FicheiroID = ficheiro.ID.Value,
                        Ficheiro = new Regras.Ficheiro().Abrir(ficheiro.ID.Value)
                    });
                }

                new Regras.ArquivoFicheiro().Inserir(bdModels, ControladorSite.Utilizador);
                return Json(new { partialview = RenderHTMLViewFicheiro(bdModels) });
            }
            catch (Regras.Exceptions.DesignacaoRepetida)
            {
                return Json(Resources.Erro.DesignacaoRepetida);
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return Json(Resources.Erro.Geral);
            }
        }


        [HttpPost]
        public ActionResult ActualizarFicheiro(Models.ArquivoFicheiro registo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Regras.BD.ArquivoFicheiro bdModel = new Regras.ArquivoFicheiro().Abrir(registo.ID.Value, ControladorSite.Utilizador);
                    Regras.Ficheiro regras = new Regras.Ficheiro();
                    bdModel.Ficheiro.Nome = registo.Nome;
                    regras.Actualizar(bdModel.Ficheiro);

                    Models.ArquivoFicheiro model = new Models.ArquivoFicheiro(bdModel);
                    model.Link = ConstroiLinkFicheiro(bdModel.FicheiroID);
                    return PartialView("_DetalheVisualizarFicheiro", model);
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
        public ActionResult ApagarDirectoria(long id)
        {
            try
            {
                if (id > 0)
                {
                    new Regras.ArquivoDirectoria().Apagar(id, ControladorSite.Utilizador);
                    return JavaScript("AfterDelete('dir_" + id + "');");
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


        [HttpDelete]
        public ActionResult ApagarFicheiro(long id)
        {
            try
            {
                if (id > 0)
                {
                    new Regras.ArquivoFicheiro().Apagar(id, ControladorSite.Utilizador);
                    return JavaScript("AfterDelete('file_" + id + "');");
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

        public Models.ConteudoDirectoria ConstroiDirectoriaConteudo(long condominioID, long? directoriaID)
        {
            return new Models.ConteudoDirectoria()
            {
                DirectoriaID = directoriaID,
                Directorias = CarregarDirectorias(condominioID, directoriaID),
                Ficheiros = CarregarFicheiros(condominioID, directoriaID)
            };
        }


        private List<Models.ArquivoDirectoria> CarregarDirectorias(long condominioID, long? directoriaID)
        {
            List<Models.ArquivoDirectoria> ret = new List<Models.ArquivoDirectoria>();

            IEnumerable<Regras.BD.ArquivoDirectoria> listaObj = new Regras.ArquivoDirectoria().Lista(
                directoriaID, ControladorSite.Utilizador);

            Models.ArquivoDirectoria model;
            foreach (var obj in listaObj)
            {
                model = new Models.ArquivoDirectoria(obj);
                model.Link = ConstroiLinkDirectoria(model.Nome);
                ret.Add(model);
            }

            return ret.OrderBy(x => x.Nome).ToList();
        }


        private List<Models.ArquivoFicheiro> CarregarFicheiros(long condominioID, long? directoriaID)
        {
            List<Models.ArquivoFicheiro> ret = new List<Models.ArquivoFicheiro>();

            IEnumerable<Regras.BD.ArquivoFicheiro> listaObj = new Regras.ArquivoFicheiro().Lista(directoriaID,
                ControladorSite.Utilizador);

            Models.ArquivoFicheiro model;
            foreach (var obj in listaObj)
            {
                model = new Models.ArquivoFicheiro(obj);
                model.Link = ConstroiLinkFicheiro(obj.FicheiroID);
                ret.Add(model);
            }

            return ret.OrderBy(x => x.Nome).ToList();
        }


        private List<Models.DirectoriaLink> ObterCaminhoDirectorias(string caminho, out long directoriaID)
        {
            directoriaID = 0;
            List<Models.DirectoriaLink> ret = new List<Models.DirectoriaLink>();

            Regras.ArquivoDirectoria regras = new Regras.ArquivoDirectoria();
            Regras.BD.ArquivoDirectoria aux = regras.AbrirOuCriarDirectoriaBase(ControladorSite.Utilizador);

            //Adicionar directoria root
            ret.Add(new Models.DirectoriaLink(aux, "Início", Url.Action("")));

            if (!string.IsNullOrEmpty(caminho))
            {
                string[] dirs = caminho.Split('/');

                string url = Url.Action("");
                foreach (string dir in dirs)
                {
                    aux = regras.Abrir(Regras.Util.UrlDecode(dir), aux.ArquivoDirectoriaID, ControladorSite.Utilizador);
                    url += "/" + Regras.Util.UrlEncode(aux.Nome);
                    ret.Add(new Models.DirectoriaLink(aux, null, url));
                }

                directoriaID = ret.Last().ID;
            }
            else
                directoriaID = aux.ArquivoDirectoriaID;
            
            return ret;
        }


        private string RenderHTMLViewFicheiro(List<Regras.BD.ArquivoFicheiro> bdModels)
        {
            string html = string.Empty;

            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            {
                ViewEngineResult viewResult;
                ViewContext viewContext;
                Models.ArquivoFicheiro model;
                foreach (Regras.BD.ArquivoFicheiro bdModel in bdModels)
                {
                    model = new Models.ArquivoFicheiro(bdModel);
                    model.Link = ConstroiLinkFicheiro(bdModel.FicheiroID);

                    this.ViewData.Model = new Models.ConteudoDirectoria()
                    {
                        DirectoriaID = bdModel.ArquivoDirectoriaID,
                        Directorias = new List<Models.ArquivoDirectoria>(),
                        Ficheiros = new List<Models.ArquivoFicheiro>() { model }
                    };
                    viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "_Lista");
                    viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                    viewResult.View.Render(viewContext, sw);
                    html += sw.GetStringBuilder().ToString();
                }
            }

            return html;
        }


        private string ConstroiLinkDirectoria(string nomeDirectoria)
        {
            string link = Url.Action("Index", "Arquivo");
            return link + (link.EndsWith("/") ? "" : "/") + Regras.Util.UrlEncode(nomeDirectoria);
        }


        private string ConstroiLinkFicheiro(long ficheiroID)
        {
            return Url.Action("Other", "Ficheiro", new 
            { 
                id = Regras.Util.UrlEncode(Regras.Util.Cifra(ficheiroID.ToString()))
            });
        }

        #endregion
    }
}