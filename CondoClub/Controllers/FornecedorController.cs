using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace CondoClub.Web.Controllers {
    [Authorize]
    public class FornecedorController : Controller {
        private const int nrFornecedoresIni = 20;
        private const int nrFornecedoresNext = 10;

        protected enum Estado { Todos, Activos, Inactivos };


        #region Acções

        public ActionResult Index(bool? porValidar) {
            try {
                List<Regras.Fornecedor.Permissao> permissoes = Regras.Fornecedor.PermissoesBO(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Fornecedor.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);
                ViewData.Add("nrFornecedoresIni", nrFornecedoresIni);
                ViewData.Add("nrFornecedoresNext", nrFornecedoresNext);

                Regras.FornecedorEstado? estadoPesquisa = null;
                if (porValidar != null && porValidar == true)
                    estadoPesquisa = Regras.FornecedorEstado.PorValidar;
                ViewData.Add("EstadoPesquisa", estadoPesquisa);

                return View(Pesquisa("", estadoPesquisa, 0, nrFornecedoresIni));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        public PartialViewResult _Lista(string termoPesquisa, Regras.FornecedorEstado? estadoPesquisa, int count) {
            try {
                if (!Regras.Fornecedor.PermissoesBO(ControladorSite.Utilizador).Contains(Regras.Fornecedor.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("nrFornecedoresIni", nrFornecedoresIni);
                ViewData.Add("nrFornecedoresNext", nrFornecedoresNext);
                return PartialView(Pesquisa(termoPesquisa.Trim(), estadoPesquisa, count, nrFornecedoresNext));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        public ViewResult Pesquisa(string termoPesquisa, Regras.FornecedorEstado? estadoPesquisa) {
            try {
                List<Regras.Fornecedor.Permissao> permissoes = Regras.Fornecedor.PermissoesBO(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Fornecedor.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);
                ViewData.Add("nrFornecedoresIni", nrFornecedoresIni);
                ViewData.Add("nrFornecedoresNext", nrFornecedoresNext);
                return View("Index", Pesquisa(termoPesquisa.Trim(), estadoPesquisa, 0, nrFornecedoresIni));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        public PartialViewResult _DetalheVisualizar(long id) {
            try {
                List<Regras.Fornecedor.Permissao> permissoes = Regras.Fornecedor.PermissoesBO(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Fornecedor.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.Fornecedor model = new Models.Fornecedor(
                    new Regras.Fornecedor().Abrir(id, ControladorSite.Utilizador)
                );

                return PartialView("_DetalheVisualizar", model);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        public PartialViewResult _DetalheEditar(long id) {
            try {
                List<Regras.Fornecedor.Permissao> permissoes = Regras.Fornecedor.PermissoesBO(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Fornecedor.Permissao.Gravar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.Fornecedor model = new Models.Fornecedor(
                    new Regras.Fornecedor().Abrir(id, ControladorSite.Utilizador)
                );

                return PartialView("_DetalheEditar", model);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        public PartialViewResult _DetalheCategoriaVisualizar(long id) {
            try {
                List<Regras.Fornecedor.Permissao> permissoes = Regras.Fornecedor.PermissoesBO(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Fornecedor.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                Regras.Fornecedor regras = new Regras.Fornecedor();
                Regras.BD.FornecedorCategoria obj = regras.FornecedorCategoriaAbrir(id, ControladorSite.Utilizador);

                Models.Fornecedor model = new Models.Fornecedor(
                    regras.Abrir(obj.FornecedorID, ControladorSite.Utilizador)
                );
                return PartialView("_DetalheCategoriaVisualizar",
                   model.Categorias.Where(kvp => kvp.Key.FornecedorCategoriaID.Value == id).First()
                );
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        public PartialViewResult _DetalheCategoriaEditar(long id) {
            try {
                List<Regras.Fornecedor.Permissao> permissoes = Regras.Fornecedor.PermissoesBO(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Fornecedor.Permissao.Gravar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.FornecedorCategoria model = new Models.FornecedorCategoria(
                    new Regras.Fornecedor().FornecedorCategoriaAbrir(id, ControladorSite.Utilizador)
                );

                return PartialView("_DetalheCategoriaEditar", model);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        [HttpPost]
        public ActionResult Gravar(Models.Fornecedor registo) {
            try {
                if (string.IsNullOrEmpty(registo.Latitude) || string.IsNullOrEmpty(registo.Longitude)) {
                    return JavaScript(String.Format("alert('{0}')", Resources.Erro.MoradaGoogleMaps));
                }

                if (ModelState.IsValid) {
                    Regras.Fornecedor regras = new Regras.Fornecedor();

                    if (!registo.ID.HasValue) {
                        Regras.BD.Fornecedor bdModel = registo.ToBDModel();
                        regras.Inserir(bdModel, ControladorSite.Utilizador);
                        List<Models.Fornecedor> modelList = new List<Models.Fornecedor>();
                        modelList.Add(new Models.Fornecedor(
                            regras.Abrir(bdModel.FornecedorID, ControladorSite.Utilizador)
                        ));
                        return PartialView("_Lista", modelList);
                    } else {
                        Regras.BD.Fornecedor bdModel = registo.ToBDModel();
                        regras.Actualizar(bdModel, ControladorSite.Utilizador);
                        return PartialView("_DetalheVisualizar", new Models.Fornecedor(
                            regras.Abrir(bdModel.FornecedorID, ControladorSite.Utilizador)
                        ));
                    }
                } else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            } catch (Regras.Exceptions.DesignacaoRepetida) {
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.DesignacaoRepetida));
            } catch (Regras.Exceptions.FornecedorSemUtilizador) {
                return JavaScript(String.Format("alert('{0}')", Resources.Servico.ErroActivarFornecedorSemUtilizador));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }


        [HttpDelete]
        public ActionResult Apagar(long id) {
            try {
                if (id > 0) {
                    new Regras.Fornecedor().Apagar(id, ControladorSite.Utilizador);
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
        public ActionResult _DetalheCategoriaGravar(Models.FornecedorCategoria registo) {
            try {
                if (ModelState.IsValid) {
                    if (!registo.FornecedorCategoriaID.HasValue) {
                        Regras.Fornecedor regras = new Regras.Fornecedor();
                        Regras.BD.FornecedorCategoria bdModel = registo.ToBDModel();
                        regras.FornecedorCategoriaInserir(bdModel, ControladorSite.Utilizador);

                        Models.Fornecedor model = new Models.Fornecedor(
                            regras.Abrir(registo.FornecedorID.Value, ControladorSite.Utilizador)
                        );
                        return PartialView("_ListaCategoria", model);
                    } else {
                        Regras.Fornecedor regras = new Regras.Fornecedor();
                        Regras.BD.FornecedorCategoria bdModel = registo.ToBDModel();
                        regras.FornecedorCategoriaActualizar(bdModel, ControladorSite.Utilizador);

                        Models.Fornecedor model = new Models.Fornecedor(
                            regras.Abrir(registo.FornecedorID.Value, ControladorSite.Utilizador)
                        );
                        return PartialView("_DetalheCategoriaVisualizar",
                           model.Categorias.Where(kvp => kvp.Key.FornecedorCategoriaID.Value == bdModel.FornecedorCategoriaID).First()
                       );
                    }
                } else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            } catch (Regras.Exceptions.CategoriaRepetida) {
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.CategoriaRepetida));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }


        [HttpDelete]
        public ActionResult _DetalheCategoriaApagar(long id) {
            try {
                if (id > 0) {
                    new Regras.Fornecedor().FornecedorCategoriaApagar(id, ControladorSite.Utilizador);
                    return JavaScript("AfterDeleteFornecedorCategoria(" + id + ");");
                } else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }


        [HttpPost]
        public ActionResult EnviarConvite(Models.FornecedorConvite convite) {
            try {
                List<Regras.Fornecedor.Permissao> permissoes = Regras.Fornecedor.PermissoesBO(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Fornecedor.Permissao.EnviarConvites))
                    throw new Exception(Resources.Erro.Acesso);

                string erro;
                if (!Util.ValidarDestinatariosConvites(convite.Destinatarios, out erro)) {
                    return JavaScript(String.Format("alert('{0}')", erro));
                } else {
                    string url = ConfigurationManager.AppSettings["AppURL"] + "/Registo/Fornecedor";

                    if (!new Regras.Condominio().EnviarConvite(
                        ConfigurationManager.AppSettings["EmailComercial"],
                        convite.Destinatarios.Replace(" ", ""),
                        convite.Assunto,
                        String.Format(convite.Mensagem, url),
                        ControladorSite.Utilizador
                    )) {
                        return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
                    }

                    return JavaScript(String.Format("alert('{0}'); Reset();", Resources.Servico.ConvitesEnviados));
                }
            } catch (Regras.Exceptions.DadosIncorrectos) {
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.DadosIncorrectos));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }


        public ActionResult Classificacao(long? fornecedorID) {
            try {

                long? fId = fornecedorID.HasValue ? fornecedorID.Value : ControladorSite.Utilizador.FornecedorID;

                List<Regras.Enum.Permissao> permissoes = Regras.Fornecedor.PermissoesClassificacaoBO(
                    ControladorSite.Utilizador, fId);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                List<Models.FornecedorClassificacao> model = new List<Models.FornecedorClassificacao>();

                List<Regras.BD.FornecedorClassificacao> objs = new Regras.Fornecedor().ObterClassificacoes(fId.Value,
                    ControladorSite.Utilizador);
                objs.ForEach(obj => model.Add(new Models.FornecedorClassificacao(obj)));

                ViewData.Add("Permissoes", permissoes);
                return View("Classificacao", model);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        [HttpDelete]
        public ActionResult ApagarClassificacao(long id) {
            try {
                new Regras.Fornecedor().ApagarClassificacao(id, ControladorSite.Utilizador);
                return JavaScript("AfterDeleteClassificacao('" + id + "');");
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }


        public ActionResult Perfil(bool redirect) {
            try {
                if (redirect)
                    return JavaScript("location='" + Url.Action("Perfil", new { @redirect = false }) + "'");

                Regras.BD.Fornecedor obj = new Regras.Fornecedor().Abrir(
                    ControladorSite.Utilizador.FornecedorID.Value, ControladorSite.Utilizador);

                if (obj != null)
                    return View("Perfil", new Models.Fornecedor(obj));
                else
                    return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, null);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        public ActionResult EditarPerfil() {
            try {
                Regras.BD.Fornecedor obj = new Regras.Fornecedor().Abrir(
                   ControladorSite.Utilizador.FornecedorID.Value, ControladorSite.Utilizador);

                if (obj != null)
                    return View("EditarPerfil", new Models.Fornecedor(obj));
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
        public ActionResult GravarPerfil(Models.Fornecedor registo) {
            try {
                if (ModelState.IsValid) {
                    if (!ValidarHiddenFields(registo))
                        throw new Regras.Exceptions.SemPermissao();

                    Regras.Fornecedor regras = new Regras.Fornecedor();
                    Regras.BD.Fornecedor bdModel = registo.ToBDModel();
                    regras.ActualizarPerfil(bdModel, ControladorSite.Utilizador);

                    return View("Perfil", new Models.Fornecedor(
                        regras.Abrir(bdModel.FornecedorID, ControladorSite.Utilizador)
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


        #region Métodos auxiliares


        public SelectList ConstroiDropDown() {
            List<Regras.BD.Fornecedor> listaObj = (List<Regras.BD.Fornecedor>)new Regras.Fornecedor().Lista();

            List<Models.FornecedorDropDown> modelList = new List<Models.FornecedorDropDown>();
            listaObj.ForEach(obj => modelList.Add(new Models.FornecedorDropDown(obj)));

            return new SelectList(modelList, "ID", "Nome");
        }


        public SelectList ConstroiDropDownEstados() {
            Dictionary<Regras.FornecedorEstado, string> estadoFornecedor = new Dictionary<Regras.FornecedorEstado, string>();
            estadoFornecedor.Add(Regras.FornecedorEstado.Activo, Resources.Servico.Activo);
            estadoFornecedor.Add(Regras.FornecedorEstado.Inactivo, Resources.Servico.Inactivo);
            estadoFornecedor.Add(Regras.FornecedorEstado.PorValidar, Resources.Servico.PorValidar);
            return new SelectList(
                estadoFornecedor.Select(x => new { ID = x.Key, Name = x.Value }).ToList(),
                "ID",
                "Name"
            );
        }


        private IEnumerable<Models.Fornecedor> Pesquisa(string termo, Regras.FornecedorEstado? estado, int skip, int take) {
            List<Regras.BD.Fornecedor> listaObj = new List<Regras.BD.Fornecedor>();

            if (ControladorSite.Utilizador.Perfil == Regras.Enum.Perfil.CondoClub) {
                listaObj.AddRange(new Regras.Fornecedor().Pesquisa(
                    null,
                    estado,
                    termo,
                    skip,
                    take,
                    ControladorSite.Utilizador
                ));
            } else if (ControladorSite.Utilizador.Perfil == Regras.Enum.Perfil.Empresa) {
                listaObj.AddRange(new Regras.Fornecedor().Pesquisa(
                    ControladorSite.Utilizador.EmpresaID.Value,
                    estado,
                    termo,
                    skip,
                    take,
                    ControladorSite.Utilizador
                ));
            }

            List<Models.Fornecedor> modelList = new List<Models.Fornecedor>();
            listaObj.ForEach(obj => modelList.Add(new Models.Fornecedor(obj)));

            return modelList;
        }


        private bool ValidarHiddenFields(Models.Fornecedor registo) {
            return SecuredValueHashComputer.ValidateValue(registo.ID.ToString(), registo.IDHash) &&
                SecuredValueHashComputer.ValidateValue(registo.RaioAccao.ToString(), registo.RaioAccaoHash) &&
                SecuredValueHashComputer.ValidateValue(registo.Activo.ToString(), registo.ActivoHash) &&
                SecuredValueHashComputer.ValidateValue(registo.DataActivacao.ToString(), registo.DataActivacaoHash) &&
                SecuredValueHashComputer.ValidateValue(registo.Endereco, registo.EnderecoHash) &&
                SecuredValueHashComputer.ValidateValue(registo.Localidade, registo.LocalidadeHash) &&
                SecuredValueHashComputer.ValidateValue(registo.Cidade, registo.CidadeHash) &&
                SecuredValueHashComputer.ValidateValue(registo.CodigoPostal, registo.CodigoPostalHash) &&
                SecuredValueHashComputer.ValidateValue(registo.Estado, registo.EstadoHash) &&
                SecuredValueHashComputer.ValidateValue(registo.PaisID.ToString(), registo.PaisIDHash) &&
                SecuredValueHashComputer.ValidateValue(registo.Latitude, registo.LatitudeHash) &&
                SecuredValueHashComputer.ValidateValue(registo.Longitude, registo.LongitudeHash);
        }


        #endregion

    }
}