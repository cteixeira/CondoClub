using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CondoClub.Web.Models;
using System.Web.Script.Serialization;
using System.Configuration;

namespace CondoClub.Web.Controllers {
    [Authorize]
    public class CondominioController : Controller {
        private const int nrCondominiosIni = 20;
        private const int nrCondominiosNext = 10;


        #region Acções

        public ActionResult Index() {
            try {
                List<Regras.CondominioPermissao> permissoes = Regras.Condominio.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.CondominioPermissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);
                ViewData.Add("nrCondominiosIni", nrCondominiosIni);
                ViewData.Add("nrCondominiosNext", nrCondominiosNext);

                Models.CondominioFiltro filtro = new CondominioFiltro();
                ViewData.Add("Filtro", filtro);
                return View(Pesquisa(filtro, 0, nrCondominiosIni));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        public PartialViewResult _Lista(Models.CondominioFiltro filtro, int count) {
            try {
                if (!Regras.Condominio.Permissoes(ControladorSite.Utilizador).Contains(Regras.CondominioPermissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("nrCondominiosIni", nrCondominiosIni);
                ViewData.Add("nrCondominiosNext", nrCondominiosNext);
                return PartialView(Pesquisa(filtro, count, nrCondominiosNext));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        public ViewResult Pesquisa(Models.CondominioFiltro filtro) {
            try {
                List<Regras.CondominioPermissao> permissoes = Regras.Condominio.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.CondominioPermissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);
                ViewData.Add("nrCondominiosIni", nrCondominiosIni);
                ViewData.Add("nrCondominiosNext", nrCondominiosNext);
                return View("Index", Pesquisa(filtro, 0, nrCondominiosIni));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        public PartialViewResult _DetalheVisualizar(long id) {
            try {
                List<Regras.CondominioPermissao> permissoes = Regras.Condominio.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.CondominioPermissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.Condominio model = new Models.Condominio(
                    new Regras.Condominio().Abrir(id, ControladorSite.Utilizador)
                );

                return PartialView("_DetalheVisualizar", model);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        public PartialViewResult _DetalheEditar(long id) {
            try {
                List<Regras.CondominioPermissao> permissoes = Regras.Condominio.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.CondominioPermissao.Gravar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.Condominio model = new Models.Condominio(
                    new Regras.Condominio().Abrir(id, ControladorSite.Utilizador)
                );

                return PartialView("_DetalheEditar", model);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        [HttpPost]
        public ActionResult Gravar(Models.Condominio registo) {
            try {
                if (string.IsNullOrEmpty(registo.Latitude) || string.IsNullOrEmpty(registo.Longitude)) {
                    return JavaScript(String.Format("alert('{0}')", Resources.Erro.MoradaGoogleMaps));
                }

                if (ModelState.IsValid) {
                    Regras.Condominio regras = new Regras.Condominio();

                    if (!registo.ID.HasValue) {
                        Regras.BD.Condominio bdModel = registo.ToBDModel();
                        regras.Inserir(bdModel, ControladorSite.Utilizador);
                        List<Models.Condominio> modelList = new List<Models.Condominio>();
                        modelList.Add(new Models.Condominio(
                            regras.Abrir(bdModel.CondominioID, ControladorSite.Utilizador)
                        ));
                        return PartialView("_Lista", modelList);
                    } else {
                        Regras.BD.Condominio bdModel = registo.ToBDModel();
                        regras.Actualizar(bdModel, ControladorSite.Utilizador);
                        return PartialView("_DetalheVisualizar", new Models.Condominio(
                            regras.Abrir(bdModel.CondominioID, ControladorSite.Utilizador)
                        ));
                    }
                } else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            } catch (Regras.Exceptions.DesignacaoRepetida) {
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.DesignacaoRepetida));
            } catch (Regras.Exceptions.CondominioSemSindico) {
                return JavaScript(String.Format("alert('{0}')", Resources.Condominio.ErroActivarCondominioSemSindico));
            }catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }


        [HttpDelete]
        public ActionResult Apagar(long id) {
            try {
                if (id > 0) {
                    new Regras.Condominio().Apagar(id, ControladorSite.Utilizador);
                    return JavaScript("AfterDelete(" + id + ");");
                } else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            } catch (Regras.Exceptions.TemDependencias) {
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.TemDependencias));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }


        [HttpPost]
        public ActionResult EnviarConvite(Models.CondominioConvite convite) {
            try {
                List<Regras.CondominioPermissao> permissoes = Regras.Condominio.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.CondominioPermissao.EnviarConvites))
                    throw new Exception(Resources.Erro.Acesso);

                string erro;
                if (!Util.ValidarDestinatariosConvites(convite.Destinatarios, out erro)) {
                    return JavaScript(String.Format("alert('{0}')", erro));
                } else {
                    string url;
                    if (ControladorSite.Utilizador.EmpresaID.HasValue) {
                        url = ConfigurationManager.AppSettings["AppURL"] + "/Registo/Condominio?cifra=" +
                            Regras.Util.Cifra(Regras.Util.UrlEncode(ControladorSite.Utilizador.EmpresaID.Value.ToString())
                        );
                    } else
                        url = ConfigurationManager.AppSettings["AppURL"] + "/Registo/Condominio";

                    if (!new Regras.Condominio().EnviarConvite(
                        ConfigurationManager.AppSettings["EmailComercial"],
                        convite.Destinatarios.Replace(" ", ""),
                        convite.Assunto,
                        String.Format(convite.Mensagem, url),
                        ControladorSite.Utilizador
                    )) {
                        return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
                    }

                    return JavaScript(String.Format("alert('{0}'); Reset();", Resources.Condominio.ConvitesEnviados));
                }
            } catch (Regras.Exceptions.DadosIncorrectos) {
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.DadosIncorrectos));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }


        public ActionResult Perfil(bool redirect) {
            try {
                if (redirect)
                    return JavaScript("location='" + Url.Action("Perfil", new { @redirect = false }) + "'");

                Regras.BD.Condominio obj = new Regras.Condominio().Abrir(
                    ControladorSite.Utilizador.CondominioID.Value, ControladorSite.Utilizador);

                if (obj != null)
                    return View("Perfil", new Models.Condominio(obj));
                else
                    return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, null);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        public ActionResult EditarPerfil() {
            try {
                Regras.BD.Condominio obj = new Regras.Condominio().Abrir(
                   ControladorSite.Utilizador.CondominioID.Value, ControladorSite.Utilizador);

                if (obj != null)
                    return View("EditarPerfil", new Models.Condominio(obj));
                else
                    throw new Regras.Exceptions.DadosIncorrectos();
            } catch (Regras.Exceptions.DadosIncorrectos ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.DadosIncorrectos));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, null);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        [HttpPost]
        public ActionResult GravarPerfil(Models.Condominio registo) {
            try {
                if (ModelState.IsValid) {
                    Regras.Condominio regras = new Regras.Condominio();
                    Regras.BD.Condominio bdModel = registo.ToBDModel();
                    regras.ActualizarPerfil(bdModel, ControladorSite.Utilizador);

                    return View("Perfil", new Models.Condominio(
                        regras.Abrir(bdModel.CondominioID, ControladorSite.Utilizador)
                    ));
                } else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            } catch (Regras.Exceptions.DadosIncorrectos) {
                ModelState.AddModelError("DadosIncorrectos", Resources.Erro.DadosIncorrectos);
                return View("EditarPerfil", registo);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, null);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        #endregion

        #region Métodos Auxiliares

        public SelectList ConstroiDropDown() {
            IEnumerable<Regras.BD.Condominio> lista = new Regras.Condominio().Lista(ControladorSite.Utilizador);
            return new SelectList(lista, "CondominioID", "Nome");
        }


        public SelectList ConstroiDropDownEstados() {
            Dictionary<Regras.CondominioEstado, string> estadoCondominio = new Dictionary<Regras.CondominioEstado, string>();
            estadoCondominio.Add(Regras.CondominioEstado.Activo, Resources.Condominio.Activo);
            estadoCondominio.Add(Regras.CondominioEstado.Inactivo, Resources.Condominio.Inactivo);
            estadoCondominio.Add(Regras.CondominioEstado.PorValidar, Resources.Condominio.PorValidar);
            return new SelectList(
                estadoCondominio.Select(x => new { ID = x.Key, Name = x.Value }).ToList(),
                "ID",
                "Name"
            );
        }


        private IEnumerable<Models.Condominio> Pesquisa(Models.CondominioFiltro filtro, int skip, int take) {
            long? empresa;
            if (ControladorSite.Utilizador.Perfil == Regras.Enum.Perfil.CondoClub)
                empresa = filtro.EmpresaPesquisa;
            else
                empresa = ControladorSite.Utilizador.EmpresaID.Value;


            List<Regras.BD.Condominio> listaObj = new List<Regras.BD.Condominio>();
            listaObj.AddRange(new Regras.Condominio().Pesquisa(
                empresa,
                filtro.EstadoPesquisa,
                filtro.TermoPesquisa,
                skip,
                take,
                ControladorSite.Utilizador
            ));


            List<Models.Condominio> modelList = new List<Models.Condominio>();
            listaObj.ForEach(obj => modelList.Add(new Models.Condominio(obj)));

            return modelList;
        }


        public JsonResult ConstroiAutoComplete(string term) {
            try {
                return Json(PesquisaAutoComplete(term), JsonRequestBehavior.AllowGet);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }


        private List<CondominioPesquisa> PesquisaAutoComplete(string termo) {
            List<Regras.BD.Condominio> listaObj = new List<Regras.BD.Condominio>();

            if (ControladorSite.Utilizador.Perfil == Regras.Enum.Perfil.CondoClub) {
                listaObj.AddRange(new Regras.Condominio().Pesquisa(null, Regras.CondominioEstado.Activo, termo, ControladorSite.Utilizador));
            } else if (ControladorSite.Utilizador.Perfil == Regras.Enum.Perfil.Empresa) {
                listaObj.AddRange(new Regras.Condominio().Pesquisa(ControladorSite.Utilizador.EmpresaID.Value, Regras.CondominioEstado.Activo,
                    termo, ControladorSite.Utilizador));
            }

            List<CondominioPesquisa> modelList = new List<CondominioPesquisa>();
            listaObj.ForEach(obj => modelList.Add(new CondominioPesquisa(obj)));

            return modelList;
        }

        #endregion

    }
}