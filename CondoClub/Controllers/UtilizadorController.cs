using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;
using System.Web.Security;
using CondoClub.Web.Models;
using System.Web;

namespace CondoClub.Web.Controllers {

    [Authorize]
    public class UtilizadorController : Controller {
        private const int nrUtilizadoresIni = 20;
        private const int nrUtilizadoresNext = 10;


        #region Login

        [AllowAnonymous]
        public ActionResult Login() {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["MaqProd"]))
                return View(new Models.Login());
            else
                return View("LoginAdministrativo", new Models.LoginAdministrativo());
        }


        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(Models.Login model, string returnUrl) {
            try {
                if (!Convert.ToBoolean(ConfigurationManager.AppSettings["MaqProd"]))
                    return RedirectToAction("Login");

                if (ModelState.IsValid) {
                    Regras.BD.Utilizador utilizador = new Regras.Utilizador().Login(model.Email, model.Password);
                    FormsAuthentication.SetAuthCookie(utilizador.UtilizadorID.ToString(), model.RememberMe);

                    if (Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                    else
                        return RedirectToRoute("Default");
                }
            }
            catch (Regras.Exceptions.Autenticacao) {
                ModelState.AddModelError("", Resources.Erro.Autenticacao);
                Regras.Util.TratamentoErro(null, GetType().FullName, 
                    new Exception(Resources.Erro.Autenticacao + " Email: " + model.Email), null);
            }
            catch (Regras.Exceptions.RegistoPorAprovar) {
                ModelState.AddModelError("", Resources.Erro.RegistoPorAprovar);
                Regras.Util.TratamentoErro(null, GetType().FullName,
                    new Exception(Resources.Erro.RegistoPorAprovar + " Email: " + model.Email), null);
            }
            catch (Exception ex) {
                ModelState.AddModelError("", Resources.Erro.Geral);
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, null);
            }

            return View(model);
        }


        [AllowAnonymous]
        [HttpPost]
        public ActionResult LoginAdministrativo(Models.LoginAdministrativo model, string returnUrl) {
            try {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["MaqProd"]))
                    return RedirectToAction("Login");

                if (ModelState.IsValid) {
                    try {
                        Regras.Utilizador regras = new Regras.Utilizador();
                        if (!regras.ValidaUtilizadorActivo(regras.Abrir(model.UtilizadorSeleccionado.Value))) {
                            ModelState.AddModelError("", Resources.Erro.UtilizadorInactivo);
                        } else {
                            ControladorSite.Utilizador = DadosUtilizadorAutenticado(model.UtilizadorSeleccionado.Value);
                            if (ControladorSite.Utilizador != null) {
                                FormsAuthentication.SetAuthCookie(ControladorSite.Utilizador.ID.ToString(), false);

                                if (Url.IsLocalUrl(returnUrl))
                                    return Redirect(returnUrl);
                                else
                                    return RedirectToRoute("Default");
                            }
                        }
                    } catch (Exception ex) {
                        ModelState.AddModelError("", Resources.Erro.Geral);
                        Regras.Util.TratamentoErro(null, GetType().FullName, ex, null);
                    }
                }
                return View(model);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, null);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        public ActionResult Sair() {
            FormsAuthentication.SignOut();

            if (ControladorSite.Utilizador.Impersonating)
                Util.DeleteCookie(HttpContext);

            return JavaScript("location='" + Url.Action("Login") + "'");
        }

        #endregion


        #region Alterar contexto utilizador

        [HttpGet]
        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult AlterarContextoVisualizar() {
            return JavaScript("$('.top-hidden-bar').slideToggle(); $('#condominios').focus();");
        }


        [HttpPost]
        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult AlterarContexto(long condominioID) {
            try {
                ControladorSite.Utilizador.CondominioID = condominioID;
                ControladorSite.Utilizador.EntidadeNome = new Regras.Condominio().Abrir(condominioID, ControladorSite.Utilizador).Nome;
                ControladorSite.Utilizador.PerfilOriginal = ControladorSite.Utilizador.Perfil;
                ControladorSite.Utilizador.Perfil = Regras.Enum.Perfil.Síndico;
                ControladorSite.Utilizador.Impersonating = true;
                Util.SetCookie(Response, ControladorSite.Utilizador.ID, condominioID);

                return JavaScript("location='" + Url.Action("Index", "Comunicado") + "'");
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, null);
                return null;
            }
        }


        [HttpGet]
        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult AlterarContextoSair() {
            try {
                if (ControladorSite.Utilizador.Impersonating) {
                    ControladorSite.Utilizador = new Regras.Utilizador().AbrirUtilizadorAutenticado(ControladorSite.Utilizador.ID);
                    Util.DeleteCookie(HttpContext);

                    return JavaScript("location='" + Url.Action("Index", "Comunicado") + "'");
                }

                return null;
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, null);
                return null;
            }
        }

        #endregion


        #region Perfil

        public ActionResult Perfil(bool redirect) {
            try {
                if (redirect)
                    return JavaScript("location='" + Url.Action("Perfil", new { @redirect = false }) + "'");


                Regras.BD.Utilizador obj = new Regras.Utilizador().Abrir(ControladorSite.Utilizador.ID,
                    ControladorSite.Utilizador);

                if (obj != null) {
                    Models.Utilizador model = new Utilizador(obj);
                    return View("Perfil", model);
                } else
                    return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, null);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        public ActionResult EditarPerfil() {
            try {
                Regras.BD.Utilizador obj = new Regras.Utilizador().Abrir(ControladorSite.Utilizador.ID,
                    ControladorSite.Utilizador);
                if (obj != null) {
                    Models.Utilizador model = new Utilizador(obj);
                    return View("EditarPerfil", model);
                } else
                    throw new Regras.Exceptions.DadosIncorrectos();
            } catch (Regras.Exceptions.DadosIncorrectos ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.DadosIncorrectos));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, null);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        public ActionResult EditarPassword() {
            try {
                Regras.BD.Utilizador obj = new Regras.Utilizador().Abrir(ControladorSite.Utilizador.ID,
                    ControladorSite.Utilizador);

                if (obj != null)
                    return View("EditarPassword", new Models.UtilizadorPassword(obj));
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
        public ActionResult GravarPerfil(Models.Utilizador registo) {
            try {
                if (registo.PerfilID == (int)Regras.Enum.Perfil.CondoClub ||
                    registo.PerfilID == (int)Regras.Enum.Perfil.Empresa ||
                    registo.PerfilID == (int)Regras.Enum.Perfil.Fornecedor ||
                    registo.PerfilID == (int)Regras.Enum.Perfil.Portaria) {
                    ModelState.Remove("Fraccao");
                }
                

                ModelState.Remove("Password");
                ModelState.Remove("ConfirmarPassword");

                if (ModelState.IsValid) {
                    new Regras.Utilizador().ActualizarPerfil(registo.ToBDModel(), ControladorSite.Utilizador);
                    return RedirectToAction("Perfil", new { @redirect = false });
                } else
                    throw new Regras.Exceptions.DadosIncorrectos();
            } catch (Regras.Exceptions.EmailRepetido) {
                ModelState.AddModelError("EmailRepetido", Resources.Erro.EmailRepetido);
                return View("EditarPerfil", registo);
            } catch (Regras.Exceptions.DadosIncorrectos ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.DadosIncorrectos));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, null);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        [HttpPost]
        public ActionResult GravarPassword(Models.UtilizadorPassword registo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!new Regras.Utilizador().ValidarPassword(ControladorSite.Utilizador.ID, registo.PasswordActual))
                    {
                        ModelState.AddModelError("ErroPasswordActualErrada", Resources.Erro.PasswordActualErrada);
                        return View("EditarPassword", registo);
                    }

                    string erro;
                    if (!ValidarActualizacaoPassword(registo.Password, registo.ConfirmarPassword, out erro))
                    {
                        ModelState.AddModelError("ErroPassword", erro);
                        return View("EditarPassword", registo);
                    }

                    Regras.Utilizador regras = new Regras.Utilizador();
                    regras.ActualizarPassword(registo.ToBDModel(), ControladorSite.Utilizador);

                    return RedirectToAction("Perfil", new { @redirect = false });
                }
                else
                    throw new Regras.Exceptions.DadosIncorrectos();
            }
            catch (Regras.Exceptions.DadosIncorrectos ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.DadosIncorrectos));
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, null);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }

        #endregion


        #region BO

        public ActionResult Index() {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.Utilizador.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);
                ViewData.Add("nrUtilizadoresIni", nrUtilizadoresIni);
                ViewData.Add("nrUtilizadoresNext", nrUtilizadoresNext);
                Models.UtilizadorFiltro filtro = new UtilizadorFiltro();
                filtro.PerfilPesquisa = Regras.Enum.Perfil.Morador;
                ViewData.Add("Filtro", filtro);
                return View(Pesquisa(filtro, 0, nrUtilizadoresIni));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        public PartialViewResult _Lista(Models.UtilizadorFiltro filtro, int count) {
            try {
                if (!Regras.Morador.Permissoes(ControladorSite.Utilizador).Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                return PartialView(Pesquisa(filtro, count, nrUtilizadoresNext));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        public ViewResult Pesquisa(Models.UtilizadorFiltro filtro) {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.Morador.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);
                ViewData.Add("nrUtilizadoresIni", nrUtilizadoresIni);
                ViewData.Add("nrUtilizadoresNext", nrUtilizadoresNext);
                return View("Index", Pesquisa(filtro, 0, nrUtilizadoresIni));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        public PartialViewResult _DetalheVisualizar(long id) {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.Utilizador.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.Utilizador model = new Models.Utilizador(
                    new Regras.Utilizador().Abrir(id, ControladorSite.Utilizador)
                );

                return PartialView("_DetalheVisualizar", model);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        public PartialViewResult _DetalheEditar(long id) {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.Utilizador.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Gravar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.Utilizador model = new Models.Utilizador(
                    new Regras.Utilizador().Abrir(id, ControladorSite.Utilizador)
                );

                return PartialView("_DetalheEditar", model);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        public PartialViewResult _DetalheEditarPassword(long id) {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.Utilizador.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Gravar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.UtilizadorPassword model = new Models.UtilizadorPassword(
                    new Regras.Utilizador().Abrir(id, ControladorSite.Utilizador)
                );

                return PartialView("_DetalheEditarPassword", model);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        [HttpPost]
        public ActionResult GravarDetalhePassword(Models.UtilizadorPassword registo) {
            try {
                if (ModelState.IsValid) {
                    string erro;
                    if (!ValidarActualizacaoPassword(registo.Password, registo.ConfirmarPassword, out erro)) {
                        return JavaScript(String.Format("alert(\"{0}\")", erro));
                    }

                    Regras.Utilizador regras = new Regras.Utilizador();
                    Regras.BD.Utilizador bdModel = registo.ToBDModel();
                    regras.ActualizarPassword(bdModel, ControladorSite.Utilizador);

                    return PartialView("_DetalheVisualizar", new Models.Utilizador(
                        regras.Abrir(bdModel.UtilizadorID, ControladorSite.Utilizador)
                    ));
                } else
                    throw new Regras.Exceptions.DadosIncorrectos();
            } catch (Regras.Exceptions.DadosIncorrectos ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.DadosIncorrectos));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }


        [HttpPost]
        public ActionResult Gravar(Models.Utilizador registo) {
            try {
                if (registo.PerfilID != (int)Regras.Enum.Perfil.Morador &&
                    registo.PerfilID != (int)Regras.Enum.Perfil.Síndico) {
                    ModelState.Remove("Fraccao");
                }
                else { 
                    if (String.IsNullOrEmpty(registo.Fraccao))
                        throw new Regras.Exceptions.FraccaoObrigatoria();
                }

                Regras.Utilizador regras = new Regras.Utilizador();

                if (!registo.ID.HasValue) {
                    if (ModelState.IsValid) {
                        Regras.BD.Utilizador bdModel = registo.ToBDModel();
                        regras.Inserir(bdModel, ControladorSite.Utilizador);
                        List<Models.Utilizador> modelList = new List<Models.Utilizador>();
                        modelList.Add(new Models.Utilizador(
                            regras.Abrir(bdModel.UtilizadorID, ControladorSite.Utilizador)
                        ));
                        return PartialView("_Lista", modelList);
                    } else
                        throw new Regras.Exceptions.DadosIncorrectos();
                } else {

                    ModelState.Remove("Password");
                    ModelState.Remove("ConfirmarPassword");

                    if (ModelState.IsValid) {
                        Regras.BD.Utilizador bdModel = registo.ToBDModel();
                        regras.Actualizar(bdModel, ControladorSite.Utilizador);
                        return PartialView("_DetalheVisualizar", new Models.Utilizador(
                            regras.Abrir(bdModel.UtilizadorID, ControladorSite.Utilizador)
                        ));
                    } else
                        throw new Regras.Exceptions.DadosIncorrectos();
                }

            } catch (Regras.Exceptions.EmailRepetido ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.EmailRepetido));
            } catch (Regras.Exceptions.CondominioSemSindico ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.CondominioSemSindico));
            } catch (Regras.Exceptions.FornecedorSemUtilizador ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.FornecedorSemUtilizador));
            } catch (Regras.Exceptions.EmpresaSemUtilizador ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.EmpresaSemUtilizador));
            } catch (Regras.Exceptions.FraccaoObrigatoria ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", String.Format(Resources.Erro.CampoObrigatorio.Replace("'", ""), Resources.Utilizador.Fraccao)));
            } catch (Regras.Exceptions.DadosIncorrectos ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.DadosIncorrectos));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }


        [HttpDelete]
        public ActionResult Apagar(long id) {
            try {
                if (id > 0) {
                    new Regras.Utilizador().Apagar(id, ControladorSite.Utilizador);
                    return JavaScript("AfterDelete(" + id + ");");
                } else
                    throw new Regras.Exceptions.DadosIncorrectos();
            } 
            catch (Regras.Exceptions.DadosIncorrectos ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.DadosIncorrectos));
            } catch (Regras.Exceptions.CondominioSemSindico ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.CondominioSemSindico));
            } catch (Regras.Exceptions.FornecedorSemUtilizador ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.FornecedorSemUtilizador));
            } catch (Regras.Exceptions.EmpresaSemUtilizador ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.EmpresaSemUtilizador));
            }catch (Regras.Exceptions.TemDependencias) {
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.TemDependencias));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }

        
        #endregion



        #region Recuperar Password

        [AllowAnonymous]
        [HttpGet]
        public ActionResult RecuperarPassword() {
            try {
                return View("RecuperarPassword");
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, null);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult RecuperarPassword(string email) {
            try {

                Regras.Utilizador rUtilizador = new Regras.Utilizador();
                rUtilizador.RecuperarPassword(email);

                return View("RecuperarPasswordSucesso");
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, null);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }

        #endregion


        #region Funcões Auxiliares


        public Regras.UtilizadorAutenticado DadosUtilizadorAutenticado(long id) {
            return new Regras.Utilizador().AbrirUtilizadorAutenticado(id);
        }


        private List<Models.Utilizador> ConstroiLista(long? condominioID, string nome, bool? activo, List<Regras.Enum.Perfil> perfis) {
            List<Models.Utilizador> modelList = new List<Models.Utilizador>();
            IEnumerable<Regras.BD.Utilizador> listaObj = new Regras.Utilizador().Lista(condominioID, nome, activo, perfis);

            foreach (var obj in listaObj)
                modelList.Add(new Models.Utilizador(obj));

            return modelList;
        }


        private List<Models.UtilizadorDropDown> ConstroiListaDropDown(long? condominioID, string nome,
            bool? activo, List<Regras.Enum.Perfil> perfis) {
            List<Models.UtilizadorDropDown> modelList = new List<Models.UtilizadorDropDown>();
            IEnumerable<Regras.BD.Utilizador> listaObj = new Regras.Utilizador().Lista(condominioID, nome, activo, perfis);

            foreach (var obj in listaObj)
                modelList.Add(new Models.UtilizadorDropDown(obj));

            return modelList;
        }


        public SelectList ConstroiDropDownAdministracao(List<Regras.Enum.Perfil> perfis) {
            List<UtilizadorDropDown> lista = ConstroiListaDropDown(null, null, true, perfis);
            if (lista.Count == 1)
                return new SelectList(lista, "ID", "Nome", lista[0].ID);
            else
                return new SelectList(lista, "ID", "Nome");
        }


        public SelectList ConstroiDropDown(List<Regras.Enum.Perfil> perfis) {
            List<UtilizadorDropDown> lista = ConstroiListaDropDown(ControladorSite.Utilizador.CondominioID.Value, null, true, perfis);
            if (lista.Count == 1)
                return new SelectList(lista, "ID", "Descricao", lista[0].ID);
            else
                return new SelectList(lista, "ID", "Descricao");
        }


        public SelectList ConstroiDropDownEstados() {
            Dictionary<Regras.UtilizadorEstado, string> estadoUtilizador = new Dictionary<Regras.UtilizadorEstado, string>();
            estadoUtilizador.Add(Regras.UtilizadorEstado.Activo, Resources.Utilizador.Activo);
            estadoUtilizador.Add(Regras.UtilizadorEstado.Inactivo, Resources.Utilizador.Inactivo);
            return new SelectList(                                                                                
                estadoUtilizador.Select(x => new { ID = x.Key, Name = x.Value }).ToList(),
                "ID",
                "Name"
            );
        }


        public List<Models.MosaicoMorador> ConstroiMosaicoMoradores() {
            try {
                IEnumerable<Regras.BD.Utilizador> users = Regras.Utilizador.ConstroiMosaico(15, ControladorSite.Utilizador);

                List<Models.MosaicoMorador> modelList = new List<Models.MosaicoMorador>();
                foreach (var obj in users) {
                    modelList.Add(new Models.MosaicoMorador(obj));
                }

                return modelList;
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, null);
                return null;
            }
        }


        private IEnumerable<Models.Utilizador> Pesquisa(Models.UtilizadorFiltro filtro, int skip, int take) {
            List<Regras.BD.Utilizador> listaObj = new List<Regras.BD.Utilizador>();

            listaObj.AddRange(new Regras.Utilizador().Pesquisa(
                filtro.EstadoPesquisa,
                filtro.TermoPesquisa,
                filtro.PerfilPesquisa,
                filtro.CondominioPesquisa,
                filtro.EmpresaPesquisa,
                filtro.FornecedorPesquisa,
                skip,
                take,
                ControladorSite.Utilizador
            ));

            List<Models.Utilizador> modelList = new List<Models.Utilizador>();
            listaObj.ForEach(obj => modelList.Add(new Models.Utilizador(obj)));

            return modelList;
        }



        private bool ValidarActualizacaoPassword(string password, string confirmarPassword, out string erro) {
            erro = string.Empty;

            if (!password.Equals(confirmarPassword)) {
                erro = Resources.Erro.PasswordsIncorrectas;
            } else if (password.Length < Util.PasswordMinLength) {
                erro = String.Format(Resources.Erro.TamanhoMinimo, Resources.Utilizador.Password, Util.PasswordMinLength);
            }

            return erro == string.Empty;
        }


        #endregion


        #region Menu de Utilizador

        public static IEnumerable<OpcaoMenu> ConstroiMenu(bool menuInterior) {

            List<OpcaoMenu> opcoes = ObterOpcoesMenuPrincipal(!menuInterior);

            if (!menuInterior)
                return opcoes.Where(x => !x.Designacao.Equals(Resources.Menu.Comunicados));

            return opcoes;
        }


        public static IEnumerable<OpcaoMenu> ConstroiMenuTopo() {
            List<OpcaoMenu> opcoes = new List<OpcaoMenu>();

            if (ControladorSite.Utilizador.Perfil == Regras.Enum.Perfil.CondoClub ||
                ControladorSite.Utilizador.Perfil == Regras.Enum.Perfil.Empresa) {
                AdicionaOpcaoMenu(opcoes, "Utilizador", "AlterarContextoVisualizar", Resources.Utilizador.SeleccionarCondominio,
                    string.Empty, "orange1-button");
            }

            if (ControladorSite.Utilizador.Impersonating) {
                AdicionaOpcaoMenu(opcoes, "Utilizador", "AlterarContextoSair", Resources.Utilizador.SairCondominio,
                    string.Empty, "orange1-button");
            }

            AdicionarOpcaoEdicaoInstituicao(opcoes);

            if (ControladorSite.Utilizador.Perfil != Regras.Enum.Perfil.Consulta) {
                AdicionaOpcaoMenu(opcoes, "Utilizador", "Perfil", Resources.Utilizador.Perfil, string.Empty, "orange2-button", string.Empty, new { @redirect = true });
            }

            AdicionaOpcaoMenu(opcoes, "Utilizador", "Sair", Resources.Geral.Sair, string.Empty, "orange2-button");

            return opcoes;
        }


        private static void AdicionarOpcaoEdicaoInstituicao(List<OpcaoMenu> opcoes) {
            if (ControladorSite.Utilizador.Perfil == Regras.Enum.Perfil.Empresa &&
                ControladorSite.Utilizador.EmpresaID.HasValue) {
                AdicionaOpcaoMenu(opcoes, "Empresa", "Perfil", Resources.Utilizador.PerfilEmpresa,
                    string.Empty, "orange2-button", string.Empty, new { @redirect = true });
            } else if (ControladorSite.Utilizador.Perfil == Regras.Enum.Perfil.Fornecedor &&
                  ControladorSite.Utilizador.FornecedorID.HasValue) {
                AdicionaOpcaoMenu(opcoes, "Fornecedor", "Perfil", Resources.Utilizador.PerfilFornecedor,
                    string.Empty, "orange2-button", string.Empty, new { @redirect = true });
            } else if (ControladorSite.Utilizador.CondominioID.HasValue &&
                  ControladorSite.Utilizador.Perfil == Regras.Enum.Perfil.Síndico &&
                  (
                      ControladorSite.Utilizador.PerfilOriginal == Regras.Enum.Perfil.Síndico ||
                      ControladorSite.Utilizador.PerfilOriginal == Regras.Enum.Perfil.CondoClub
                  )
              ) {
                AdicionaOpcaoMenu(opcoes, "Condominio", "Perfil", Resources.Utilizador.PerfilCondominio,
                    string.Empty, "orange2-button", string.Empty, new { @redirect = true });
            }
        }


        private static List<OpcaoMenu> ObterOpcoesMenuPrincipal(bool carregaNumeroNovidades) {
            List<OpcaoMenu> opcoes = new List<OpcaoMenu>();
            int numeroMensagensNovas = 0;
            int numeroQuestionariosNovos = 0;
            if (carregaNumeroNovidades) {
                numeroMensagensNovas = Regras.Mensagem.NumeroMensagensNovas(ControladorSite.Utilizador);
                numeroQuestionariosNovos = Regras.Questionario.NumeroQuestionariosNaoRespondidos(ControladorSite.Utilizador);
            }
            switch (ControladorSite.Utilizador.Perfil) {
                case CondoClub.Regras.Enum.Perfil.CondoClub:
                    AdicionaOpcaoMenu(opcoes, "Comunicado", "Index", Resources.Menu.Comunicados, Resources.Menu.ComunicadosDescricao, "announcements");
                    AdicionaOpcaoMenu(opcoes, "Mensagem", "Index", Resources.Menu.Mensagens, Resources.Menu.MensagensDescricao, "messages", "color1", null, numeroMensagensNovas);
                    AdicionaOpcaoMenu(opcoes, "Condominio", "Index", Resources.Menu.Condominios, Resources.Menu.CondominiosDescricao, "condominium", "color4");
                    AdicionaOpcaoMenu(opcoes, "Fornecedor", "Index", Resources.Menu.Fornecedores, Resources.Menu.FornecedoresDescricao, "suppliers", "color6");
                    AdicionaOpcaoMenu(opcoes, "Empresa", "Index", Resources.Menu.Empresas, Resources.Menu.EmpresasDescricao, "companies", "color5");
                    AdicionaOpcaoMenu(opcoes, "Publicidade", "Index", Resources.Menu.Publicidade, Resources.Menu.PublicidadeDescricao, "advertising", "color7");
                    AdicionaOpcaoMenu(opcoes, "Pagamento", "Index", Resources.Menu.Pagamentos, Resources.Menu.PagamentosDescricao, "payments", "color10");
                    AdicionaOpcaoMenu(opcoes, "Estatistica", "Index", Resources.Menu.Estatisticas, Resources.Menu.EstatisticasDescricao, "statistics", "color8");
                    AdicionaOpcaoMenu(opcoes, "Utilizador", "Index", Resources.Menu.Utilizadores, Resources.Menu.UtilizadoresDescricao, "residents", "color9");
                    AdicionaOpcaoMenu(opcoes, "PrecoCondominio", "Index", Resources.Menu.Precos, Resources.Menu.PrecosDescricao, "price", "color2", null, 0, new string[] {"PrecoFornecedor", "PrecoPublicidade"});
                    break;
                case CondoClub.Regras.Enum.Perfil.Empresa:
                    AdicionaOpcaoMenu(opcoes, "Comunicado", "Index", Resources.Menu.Comunicados, Resources.Menu.ComunicadosDescricao, "announcements");
                    AdicionaOpcaoMenu(opcoes, "Mensagem", "Index", Resources.Menu.Mensagens, Resources.Menu.MensagensDescricao, "messages", "color1", null, numeroMensagensNovas);
                    AdicionaOpcaoMenu(opcoes, "Condominio", "Index", Resources.Menu.Condominios, Resources.Menu.CondominiosDescricao, "condominium", "color4");
                    AdicionaOpcaoMenu(opcoes, "Servico", "Index", Resources.Menu.Servicos, Resources.Menu.ServicosDescricao, "services", "color3");
                    AdicionaOpcaoMenu(opcoes, "Pagamento", "Index", Resources.Menu.Pagamentos, Resources.Menu.PagamentosDescricao, "payments", "color10");
                    AdicionaOpcaoMenu(opcoes, "Estatistica", "Index", Resources.Menu.Estatisticas, Resources.Menu.EstatisticasDescricao, "statistics", "color8");
                    break;
                case CondoClub.Regras.Enum.Perfil.Síndico:
                    AdicionaOpcaoMenu(opcoes, "Comunicado", "Index", Resources.Menu.Comunicados, Resources.Menu.ComunicadosDescricao, "announcements");
                    AdicionaOpcaoMenu(opcoes, "Mensagem", "Index", Resources.Menu.Mensagens, Resources.Menu.MensagensDescricao, "messages", "color1", null, numeroMensagensNovas);
                    AdicionaOpcaoMenu(opcoes, "Agenda", "Index", Resources.Menu.Agenda, Resources.Menu.AgendaDescricao, "agenda", "color2");
                    AdicionaOpcaoMenu(opcoes, "Servico", "Index", Resources.Menu.Servicos, Resources.Menu.ServicosDescricao, "services", "color3");
                    AdicionaOpcaoMenu(opcoes, "Questionario", "Index", Resources.Menu.Questionarios, Resources.Menu.QuestionariosDescricao, "survey", "color4", null, numeroQuestionariosNovos);
                    AdicionaOpcaoMenu(opcoes, "Calendario", "Index", Resources.Menu.Calendario, Resources.Menu.CalendarioDescricao, "calendar", "color5");
                    AdicionaOpcaoMenu(opcoes, "Arquivo", "Index", Resources.Menu.Arquivo, Resources.Menu.ArquivoDescricao, "files", "color6");
                    //AdicionaOpcaoMenu(opcoes, "Veiculo", "Index", Resources.Menu.Veiculos, Resources.Menu.VeiculosDescricao, "vehicles", "color7");
                    AdicionaOpcaoMenu(opcoes, "Funcionario", "Index", Resources.Menu.Funcionarios, Resources.Menu.FuncionariosDescricao, "staff", "color8");
                    AdicionaOpcaoMenu(opcoes, "Pagamento", "Index", Resources.Menu.Pagamentos, Resources.Menu.PagamentosDescricao, "payments", "color10");
                    AdicionaOpcaoMenu(opcoes, "Morador", "Index", Resources.Menu.Moradores, Resources.Menu.MoradoresDescricao, "residents", "color9");
                    break;
                case CondoClub.Regras.Enum.Perfil.Morador:
                    AdicionaOpcaoMenu(opcoes, "Comunicado", "Index", Resources.Menu.Comunicados, Resources.Menu.ComunicadosDescricao, "announcements");
                    AdicionaOpcaoMenu(opcoes, "Mensagem", "Index", Resources.Menu.Mensagens, Resources.Menu.MensagensDescricao, "messages", "color1", null, numeroMensagensNovas);
                    AdicionaOpcaoMenu(opcoes, "Agenda", "Index", Resources.Menu.Agenda, Resources.Menu.AgendaDescricao, "agenda", "color2");
                    AdicionaOpcaoMenu(opcoes, "Servico", "Index", Resources.Menu.Servicos, Resources.Menu.ServicosDescricao, "services", "color3");
                    AdicionaOpcaoMenu(opcoes, "Questionario", "Index", Resources.Menu.Questionarios, Resources.Menu.QuestionariosDescricao, "survey", "color4", null, numeroQuestionariosNovos);
                    AdicionaOpcaoMenu(opcoes, "Calendario", "Index", Resources.Menu.Calendario, Resources.Menu.CalendarioDescricao, "calendar", "color5");
                    AdicionaOpcaoMenu(opcoes, "Arquivo", "Index", Resources.Menu.Arquivo, Resources.Menu.ArquivoDescricao, "files", "color6");
                    //AdicionaOpcaoMenu(opcoes, "Veiculo", "Index", Resources.Menu.Veiculos, Resources.Menu.VeiculosDescricao, "vehicles", "color7");
                    AdicionaOpcaoMenu(opcoes, "Funcionario", "Index", Resources.Menu.Funcionarios, Resources.Menu.Funcionarios, "staff", "color8");
                    break;
                case CondoClub.Regras.Enum.Perfil.Portaria:
                    AdicionaOpcaoMenu(opcoes, "Comunicado", "Index", Resources.Menu.Comunicados, Resources.Menu.ComunicadosDescricao, "announcements");
                    AdicionaOpcaoMenu(opcoes, "Mensagem", "Index", Resources.Menu.Mensagens, Resources.Menu.MensagensDescricao, "messages", "color1", null, numeroMensagensNovas);
                    AdicionaOpcaoMenu(opcoes, "Agenda", "Index", Resources.Menu.Agenda, Resources.Menu.AgendaDescricao, "agenda", "color2");
                    AdicionaOpcaoMenu(opcoes, "Arquivo", "Index", Resources.Menu.Arquivo, Resources.Menu.ArquivoDescricao, "files", "color6");
                    //AdicionaOpcaoMenu(opcoes, "Veiculo", "Index", Resources.Menu.Veiculos, Resources.Menu.VeiculosDescricao, "vehicles", "color7");
                    AdicionaOpcaoMenu(opcoes, "Funcionario", "Index", Resources.Menu.Funcionarios, Resources.Menu.Funcionarios, "staff", "color8");
                    break;
                case CondoClub.Regras.Enum.Perfil.Consulta:
                    AdicionaOpcaoMenu(opcoes, "Comunicado", "Index", Resources.Menu.Comunicados, Resources.Menu.ComunicadosDescricao, "announcements");
                    AdicionaOpcaoMenu(opcoes, "Agenda", "Index", Resources.Menu.Agenda, Resources.Menu.AgendaDescricao, "agenda", "color2");
                    AdicionaOpcaoMenu(opcoes, "Servico", "Index", Resources.Menu.Servicos, Resources.Menu.ServicosDescricao, "services", "color3");
                    AdicionaOpcaoMenu(opcoes, "Questionario", "Index", Resources.Menu.Questionarios, Resources.Menu.QuestionariosDescricao, "survey", "color4");
                    AdicionaOpcaoMenu(opcoes, "Calendario", "Index", Resources.Menu.Calendario, Resources.Menu.CalendarioDescricao, "calendar", "color5");
                    AdicionaOpcaoMenu(opcoes, "Arquivo", "Index", Resources.Menu.Arquivo, Resources.Menu.ArquivoDescricao, "files", "color6");
                    break;
                case CondoClub.Regras.Enum.Perfil.Fornecedor:
                    AdicionaOpcaoMenu(opcoes, "Mensagem", "Index", Resources.Menu.Mensagens, Resources.Menu.MensagensDescricao, "messages", "color1", null, numeroMensagensNovas);
                    AdicionaOpcaoMenu(opcoes, "Publicidade", "Index", Resources.Menu.Publicidade, Resources.Menu.PublicidadeDescricao, "advertising", "color7");
                    AdicionaOpcaoMenu(opcoes, "Pagamento", "Index", Resources.Menu.Pagamentos, Resources.Menu.PagamentosDescricao, "payments", "color10");
                    AdicionaOpcaoMenu(opcoes, "Estatistica", "Index", Resources.Menu.Estatisticas, Resources.Menu.EstatisticasDescricao, "statistics", "color8");
                    AdicionaOpcaoMenu(opcoes, "Fornecedor", "Classificacao", Resources.Menu.ClassificacaoServico, Resources.Menu.ClassificacaoServicoDescricao, "classification", "color7");
                    break;
            }

            return opcoes;
        }


        private static void AdicionaOpcaoMenu(List<OpcaoMenu> opcoes, string controlador, string accao, string designacao,
            string descricao, string cssClass, string cssClassCor = "", object routeValues = null, int numeroNovidades = 0, string[] controllersItemSelected = null) {
            if (!opcoes.Any(o => o.Controlador == controlador && o.Accao == accao)) {
                opcoes.Add(new OpcaoMenu() {
                    Controlador = controlador,
                    Accao = accao,
                    Designacao = designacao,
                    Descricao = descricao,
                    CssClass = cssClass,
                    CssClassCor = cssClassCor,
                    RouteValues = routeValues,
                    NumeroNovidades = numeroNovidades > 0 ? numeroNovidades.ToString() : String.Empty,
                    ControllersItemSelected = controllersItemSelected != null ? controllersItemSelected : new string[]{ }
                });
            }
        }


        public class OpcaoMenu {
            public string Controlador { get; set; }
            public string Accao { get; set; }
            public string Designacao { get; set; }
            public string Descricao { get; set; }
            public string CssClass { get; set; }
            public string CssClassCor { get; set; }
            public string NumeroNovidades { get; set; }
            public object RouteValues { get; set; }
            public string[] ControllersItemSelected { get; set; }
        }

        #endregion
    }
}