using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace CondoClub.Web.Controllers
{
    [Authorize]
    public class MoradorController : Controller
    {
        private const int nrMoradoresIni = 20;
        private const int nrMoradoresNext = 10;

        protected enum Estado { Todos, Activos, Inactivos };


        #region Acções

        public ActionResult Index()
        {
            try
            {
                List<Regras.Enum.Permissao> permissoes = Regras.Morador.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);
                ViewData.Add("nrMoradoresIni", nrMoradoresIni);
                ViewData.Add("nrMoradoresNext", nrMoradoresNext);
                return View(Pesquisa("", null, 0, nrMoradoresIni));
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        public PartialViewResult _Lista(string termoPesquisa, Regras.UtilizadorEstado? estadoPesquisa, int count)
        {
            try
            {
                if (!Regras.Morador.Permissoes(ControladorSite.Utilizador).Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                return PartialView(Pesquisa(termoPesquisa, estadoPesquisa, count, nrMoradoresNext));
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        public ViewResult Pesquisa(string termoPesquisa, Regras.UtilizadorEstado? estadoPesquisa)
        {
            try
            {
                List<Regras.Enum.Permissao> permissoes = Regras.Morador.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);
                ViewData.Add("nrMoradoresIni", nrMoradoresIni);
                ViewData.Add("nrMoradoresNext", nrMoradoresNext);
                return View("Index", Pesquisa(termoPesquisa, estadoPesquisa, 0, nrMoradoresIni));
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        public PartialViewResult _DetalheVisualizar(long id)
        {
            try
            {
                List<Regras.Enum.Permissao> permissoes = Regras.Morador.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.Morador model = new Models.Morador(
                    new Regras.Morador().Abrir(id, ControladorSite.Utilizador)
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
                List<Regras.Enum.Permissao> permissoes = Regras.Morador.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Gravar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.Morador model = new Models.Morador(
                    new Regras.Morador().Abrir(id, ControladorSite.Utilizador)
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
        public ActionResult Gravar(Models.Morador registo)
        {
            try
            {
                if (registo.PerfilUtilizadorID != (int)Regras.Enum.Perfil.Portaria && String.IsNullOrEmpty(registo.Fraccao))
                    throw new Regras.Exceptions.FraccaoObrigatoria();

                if (ModelState.IsValid)
                {
                    Regras.Morador regras = new Regras.Morador();

                    if (!registo.ID.HasValue)
                    {
                        if (!registo.Password.Equals(registo.ConfirmarPassword))
                        {
                            return JavaScript(String.Format("alert('{0}')", Resources.Erro.PasswordsIncorrectas));
                        }

                        Regras.BD.Utilizador bdModel = registo.ToBDModel();
                        regras.Inserir(bdModel, ControladorSite.Utilizador);
                        List<Models.Morador> modelList = new List<Models.Morador>();
                        modelList.Add(new Models.Morador(
                            regras.Abrir(bdModel.UtilizadorID, ControladorSite.Utilizador)
                        ));
                        return PartialView("_Lista", modelList);
                    }
                    else
                    {
                        Regras.BD.Utilizador bdModel = registo.ToBDModel();
                        regras.Actualizar(bdModel, ControladorSite.Utilizador);
                        return PartialView("_DetalheVisualizar", new Models.Morador(
                            regras.Abrir(bdModel.UtilizadorID, ControladorSite.Utilizador)
                        ));
                    }
                }
                else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            }
            catch (Regras.Exceptions.EmailRepetido)
            {
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.EmailRepetido));
            }
            catch (Regras.Exceptions.CondominioSemSindico)
            {
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.CondominioSemSindico));
            }
            catch (Regras.Exceptions.FraccaoObrigatoria)
            {
                return JavaScript(String.Format("alert('{0}')", String.Format(Resources.Erro.CampoObrigatorio.Replace("'", ""), 
                    Resources.Utilizador.Fraccao)));
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
                    new Regras.Morador().Apagar(id, ControladorSite.Utilizador);
                    return JavaScript("AfterDelete(" + id + ");");
                }
                else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            }
            catch (Regras.Exceptions.TemDependencias) {
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.TemDependencias));
            } 
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }


        [HttpPost]
        public ActionResult EnviarConvite(Models.MoradorConvite convite)
        {
            try
            {
                string erro;
                if (!Util.ValidarDestinatariosConvites(convite.Destinatarios, out erro))
                {
                    return JavaScript(String.Format("alert('{0}')", erro));
                }
                else
                {
                    string url;
                    if (ControladorSite.Utilizador.CondominioID.HasValue)
                    {
                        url = ConfigurationManager.AppSettings["AppURL"] + "/Registo/Utilizador?cifra=" + Regras.Util.Cifra(
                            Regras.Util.UrlEncode(((int)Regras.Enum.Perfil.Morador) + ";" + ControladorSite.Utilizador.CondominioID.Value)
                        );
                    }
                    else
                        throw new Regras.Exceptions.DadosIncorrectos();

                    if (!new Regras.Morador().EnviarConvite(
                        ConfigurationManager.AppSettings["EmailComercial"],
                        //convite.Destinatarios.Substring(0, convite.Destinatarios.Length - 1).Replace(' ', ','),
                        convite.Destinatarios.Replace(" ", ""),
                        convite.Assunto,
                        String.Format(convite.Mensagem, url),
                        ControladorSite.Utilizador
                    ))
                    {
                        return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
                    }

                    return JavaScript(String.Format("alert('{0}'); Reset();", Resources.Condominio.ConvitesEnviados));
                }
            }
            catch (Regras.Exceptions.DadosIncorrectos)
            {
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.DadosIncorrectos));
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }

        #endregion


        #region Métodos Auxiliares


        private bool? PesquisaCampoActivo(string estado)
        {
            Estado estadoSeleccionado;
            if (Enum.TryParse(estado, out estadoSeleccionado))
            {
                switch (estadoSeleccionado)
                {
                    case Estado.Todos: return null;
                    case Estado.Activos: return true;
                    case Estado.Inactivos: return false;
                }
            }

            return null;
        }


        private IEnumerable<Models.Morador> Pesquisa(string termo, Regras.UtilizadorEstado? estado, int skip, int take)
        {
            List<Regras.BD.Utilizador> listaObj = new List<Regras.BD.Utilizador>();

            listaObj.AddRange(new Regras.Morador().Pesquisa(
                ControladorSite.Utilizador.CondominioID.Value, 
                estado, 
                termo, 
                skip, 
                take, 
                ControladorSite.Utilizador
            ));

            List<Models.Morador> modelList = new List<Models.Morador>();
            listaObj.ForEach(obj => modelList.Add(new Models.Morador(obj)));

            return modelList;
        }

        
        public SelectList ConstroiDropDown(long? condominioID, bool? activo) {
            Regras.Morador rMorador = new Regras.Morador();
            IEnumerable<Regras.BD.Utilizador> moradores = rMorador.Lista(condominioID, activo.HasValue ? activo.Value : true);
            return new SelectList(
                moradores,
                "UtilizadorID",
                "Nome"
            );
        }

        #endregion

    }
}
