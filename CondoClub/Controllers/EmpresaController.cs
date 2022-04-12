using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace CondoClub.Web.Controllers {
    [Authorize]
    public class EmpresaController : Controller {

        private const int nrEmpresasIni = 20;
        private const int nrEmpresasNext = 10;

        #region Acções

        public ActionResult Index() {
            try {
                List<Regras.EmpresaPermissao> permissoes = Regras.Empresa.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.EmpresaPermissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);
                ViewData.Add("nrEmpresasIni", nrEmpresasIni);
                ViewData.Add("nrEmpresasNext", nrEmpresasNext);
                return View(Pesquisa("", null, 0, nrEmpresasIni));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        public PartialViewResult _Lista(string termoPesquisa, Regras.EmpresaEstado? estadoPesquisa, int count) {
            try {
                if (!Regras.Empresa.Permissoes(ControladorSite.Utilizador).Contains(Regras.EmpresaPermissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("nrEmpresasIni", nrEmpresasIni);
                ViewData.Add("nrEmpresasNext", nrEmpresasNext);
                return PartialView(Pesquisa(termoPesquisa.Trim(), estadoPesquisa, count, nrEmpresasNext));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }

        public ViewResult Pesquisa(string termoPesquisa, Regras.EmpresaEstado? estadoPesquisa) {
            try {
                List<Regras.EmpresaPermissao> permissoes = Regras.Empresa.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.EmpresaPermissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);
                ViewData.Add("nrEmpresasIni", nrEmpresasIni);
                ViewData.Add("nrEmpresasNext", nrEmpresasNext);
                return View("Index", Pesquisa(termoPesquisa.Trim(), estadoPesquisa, 0, nrEmpresasIni));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        public PartialViewResult _DetalheVisualizar(long id) {
            try {
                List<Regras.EmpresaPermissao> permissoes = Regras.Empresa.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.EmpresaPermissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.Empresa model = new Models.Empresa(
                    new Regras.Empresa().Abrir(id, ControladorSite.Utilizador)
                );

                return PartialView("_DetalheVisualizar", model);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        public PartialViewResult _DetalheEditar(long id) {
            try {
                List<Regras.EmpresaPermissao> permissoes = Regras.Empresa.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.EmpresaPermissao.Gravar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.Empresa model = new Models.Empresa(
                    new Regras.Empresa().Abrir(id, ControladorSite.Utilizador)
                );

                return PartialView("_DetalheEditar", model);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        [HttpPost]
        public ActionResult Gravar(Models.Empresa registo) {
            try {
                if (string.IsNullOrEmpty(registo.Latitude) || string.IsNullOrEmpty(registo.Longitude)) {
                    return JavaScript(String.Format("alert('{0}')", Resources.Erro.MoradaGoogleMaps));
                }

                if (ModelState.IsValid) {
                    Regras.Empresa regras = new Regras.Empresa();

                    if (!registo.ID.HasValue) {
                        Regras.BD.Empresa bdModel = registo.ToBDModel();
                        regras.Inserir(bdModel, ControladorSite.Utilizador);
                        List<Models.Empresa> modelList = new List<Models.Empresa>();
                        modelList.Add(new Models.Empresa(
                            regras.Abrir(bdModel.EmpresaID, ControladorSite.Utilizador)
                        ));
                        return PartialView("_Lista", modelList);
                    } else {
                        Regras.BD.Empresa bdModel = registo.ToBDModel();
                        regras.Actualizar(bdModel, ControladorSite.Utilizador);
                        return PartialView("_DetalheVisualizar", new Models.Empresa(
                            regras.Abrir(bdModel.EmpresaID, ControladorSite.Utilizador)
                        ));
                    }
                } else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            } catch (Regras.Exceptions.DesignacaoRepetida) {
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.DesignacaoRepetida));
            } catch (Regras.Exceptions.EmpresaSemUtilizador) {
                return JavaScript(String.Format("alert('{0}')", Resources.Empresa.ErroActivarEmpresaSemUtilizador));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }


        [HttpDelete]
        public ActionResult Apagar(long id) {
            try {
                if (id > 0) {
                    new Regras.Empresa().Apagar(id, ControladorSite.Utilizador);
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
        public ActionResult EnviarConvite(Models.EmpresaConvite convite) {
            try {
                List<Regras.EmpresaPermissao> permissoes = Regras.Empresa.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.EmpresaPermissao.EnviarConvites))
                    throw new Exception(Resources.Erro.Acesso);

                string erro;
                if (!Util.ValidarDestinatariosConvites(convite.Destinatarios, out erro)) {
                    return JavaScript(String.Format("alert('{0}')", erro));
                } else {
                    string url = ConfigurationManager.AppSettings["AppURL"] + "/Registo/Empresa";

                    if (!new Regras.Condominio().EnviarConvite(
                        ConfigurationManager.AppSettings["EmailComercial"],
                        convite.Destinatarios.Replace(" ", ""),
                        convite.Assunto,
                        String.Format(convite.Mensagem, url),
                        ControladorSite.Utilizador
                    )) {
                        return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
                    }

                    return JavaScript(String.Format("alert('{0}'); Reset();", Resources.Empresa.ConvitesEnviados));
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

                Regras.BD.Empresa obj = new Regras.Empresa().Abrir(
                    ControladorSite.Utilizador.EmpresaID.Value, ControladorSite.Utilizador);

                if (obj != null)
                    return View("Perfil", new Models.Empresa(obj));
                else
                    return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, null);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        public ActionResult EditarPerfil() {
            try {
                Regras.BD.Empresa obj = new Regras.Empresa().Abrir(
                   ControladorSite.Utilizador.EmpresaID.Value, ControladorSite.Utilizador);

                if (obj != null)
                    return View("EditarPerfil", new Models.Empresa(obj));
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
        public ActionResult GravarPerfil(Models.Empresa registo) {
            try {
                if (ModelState.IsValid) {
                    Regras.Empresa regras = new Regras.Empresa();
                    Regras.BD.Empresa bdModel = registo.ToBDModel();
                    regras.ActualizarPerfil(bdModel, ControladorSite.Utilizador);

                    return View("Perfil", new Models.Empresa(
                        regras.Abrir(bdModel.EmpresaID, ControladorSite.Utilizador)
                    ));
                } else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            } catch (Regras.Exceptions.DadosIncorrectos) {
                ModelState.AddModelError("DadosIncorrectos", Resources.Erro.DadosIncorrectos);
                return View("EditarPerfil", registo);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }

        #endregion


        #region Métodos auxiliares

        public SelectList ConstroiDropDown() {
            List<Regras.BD.Empresa> listaObj = (List<Regras.BD.Empresa>)new Regras.Empresa().Lista(ControladorSite.Utilizador);

            List<Models.EmpresaDropDown> modelList = new List<Models.EmpresaDropDown>();
            listaObj.ForEach(obj => modelList.Add(new Models.EmpresaDropDown(obj)));

            return new SelectList(modelList, "ID", "Nome");
        }


        public SelectList ConstroiDropDownEstados() {
            Dictionary<Regras.EmpresaEstado, string> estadoEmpresa = new Dictionary<Regras.EmpresaEstado, string>();
            estadoEmpresa.Add(Regras.EmpresaEstado.Activo, Resources.Empresa.Activo);
            estadoEmpresa.Add(Regras.EmpresaEstado.Inactivo, Resources.Empresa.Inactivo);
            estadoEmpresa.Add(Regras.EmpresaEstado.PorValidar, Resources.Empresa.PorValidar);
            return new SelectList(
                estadoEmpresa.Select(x => new { ID = x.Key, Name = x.Value }).ToList(),
                "ID",
                "Name"
            );
        }


        private IEnumerable<Models.Empresa> Pesquisa(string termo, Regras.EmpresaEstado? estado, int skip, int take) {
            List<Regras.BD.Empresa> listaObj = new List<Regras.BD.Empresa>();

            long? empresa = null;
            if (ControladorSite.Utilizador.Perfil == Regras.Enum.Perfil.Empresa)
                empresa = ControladorSite.Utilizador.EmpresaID.Value;

            listaObj.AddRange(new Regras.Empresa().Pesquisa(
                empresa,
                estado,
                termo,
                skip,
                take,
                ControladorSite.Utilizador
            ));

            List<Models.Empresa> modelList = new List<Models.Empresa>();
            listaObj.ForEach(obj => modelList.Add(new Models.Empresa(obj)));

            return modelList;
        }


        #endregion

    }
}