using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CondoClub.Web.Controllers
{
    [Authorize]
    public class RegistoController : Controller
    {
        [AllowAnonymous]
        public ActionResult Condominio(string cifra)
        {
            Models.CondominioRegisto model = TempData["model_retroceder"] as Models.CondominioRegisto;

            try
            {
                TempData.Clear();

                if (model == null)
                {
                    model = new Models.CondominioRegisto();

                    //cifra = <ID da empresa>
                    if (!string.IsNullOrEmpty(cifra))
                    {
                        string parametroDecifrado = Regras.Util.Decifra(Regras.Util.UrlDecode(cifra));

                        long empresaID;
                        if (!long.TryParse(parametroDecifrado, out empresaID))
                            throw new Regras.Exceptions.DadosIncorrectos();
                        else
                            model.EmpresaID = empresaID;
                    }
                }
                
                return View("Condominio", model);
            }
            catch (Regras.Exceptions.DadosIncorrectos ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Condominio", model);
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return RedirectToAction("Index", "Erro");
            }
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegistarCondominio(Models.CondominioRegisto registo)
        {
            try
            {
                if (string.IsNullOrEmpty(registo.Latitude) || string.IsNullOrEmpty(registo.Longitude))
                {
                    ModelState.AddModelError("GoogleMap", Resources.Erro.MoradaGoogleMaps);
                    return View("Condominio", registo);
                }

                if (ModelState.IsValid)
                {
                    Regras.BD.Condominio obj = registo.ToBDModel();
                    new Regras.Condominio().Registar(obj);

                    string cifra = Regras.Util.UrlEncode(Regras.Util.Cifra(
                        ((int)Regras.Enum.Perfil.Síndico) + ";" + obj.CondominioID
                    ));

                    return RedirectToAction("Utilizador", new { @cifra = cifra });
                }
                else
                    throw new Regras.Exceptions.DadosIncorrectos();
            }
            catch (Regras.Exceptions.DadosIncorrectos ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Condominio", registo);
            }
            catch (Regras.Exceptions.DesignacaoRepetida)
            {
                ModelState.AddModelError("DesignacaoRepetida", Resources.Erro.DesignacaoRepetida);
                return View("Condominio", registo);
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return RedirectToAction("Index", "Erro");
            }
        }


        [AllowAnonymous]
        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Utilizador(string cifra)
        {
            try
            {
                // Perfil Morador: cifra = <Perfil do utilizador>;<ID do condominio>
                // Perfil Sindico: cifra = <Perfil do utilizador>;<ID do condominio>
                // Perfil Fornecedor: cifra = <Perfil do utilizador>;<ID do condominio>
                // Perfil Empresa: cifra = <Perfil do utilizador>;<ID da empresa>

                string[] parametrosDecifrados = Regras.Util.Decifra(Regras.Util.UrlDecode(cifra)).Split(';');

                Regras.Enum.Perfil perfil;
                long id;
                if(!Enum.TryParse(parametrosDecifrados[0], out perfil) || !long.TryParse(parametrosDecifrados[1], out id))
                    throw new Regras.Exceptions.DadosIncorrectos();

                Models.UtilizadorRegisto model = new Models.UtilizadorRegisto();
                model.PerfilID = (int)perfil;

                if (perfil == Regras.Enum.Perfil.Síndico || perfil == Regras.Enum.Perfil.Morador)
                {
                    //se for sindico, está no processo de registo de um condominio,
                    //validar que ainda não tem nenhum sindico registado
                    //pode ter acontecido se carregou back na página de sucesso
                    if (perfil == Regras.Enum.Perfil.Síndico) {
                        Regras.Utilizador rUtilizador = new Regras.Utilizador();
                        IEnumerable<Regras.BD.Utilizador> users = rUtilizador.Lista(id, null);
                        if (users != null && users.Count() > 0) {
                            return RedirectToAction("Sucesso");         
                        }
                    }


                    model.CondominioID = id;
                    model.AceitarCondicoesGerais = true;
                }
                else if (perfil == Regras.Enum.Perfil.Empresa)
                {

                    //a empresa apenas pode registar um utilizador
                    //validar que ainda não tem nenhum utilizador da empresa registado
                    //pode ter acontecido se carregou back na página de sucesso
                    Regras.Utilizador rUtilizador = new Regras.Utilizador();
                    IEnumerable<Regras.BD.Utilizador> users = rUtilizador.ListaUtilizadoresEmpresa(id);
                    if (users != null && users.Count() > 0) {
                        return RedirectToAction("Sucesso");
                    }

                    model.EmpresaID = id;
                    model.AceitarCondicoesGerais = true;
                }
                else if (perfil == Regras.Enum.Perfil.Fornecedor)
                {
                    //o fornecedor apenas pode registar um utilizador
                    //validar que ainda não tem nenhum utilizador do fornecedor registado
                    //pode ter acontecido se carregou back na página de sucesso
                    Regras.Utilizador rUtilizador = new Regras.Utilizador();
                    IEnumerable<Regras.BD.Utilizador> users = rUtilizador.ListaUtilizadoresFornecedor(id);
                    if (users != null && users.Count() > 0) {
                        return RedirectToAction("Sucesso");
                    }

                    model.FornecedorID = id;
                    model.AceitarCondicoesGerais = true;
                }

                model.PerfilID = (int)perfil;

                return View("Utilizador", model);
            }
            catch (Regras.Exceptions.DadosIncorrectos ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Condominio", new Models.UtilizadorRegisto());
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return RedirectToAction("Index", "Erro");
            }
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegistarUtilizador(Models.UtilizadorRegisto registo)
        {
            try
            {
                if (registo.PerfilID != (int)CondoClub.Regras.Enum.Perfil.Síndico &&
                    registo.PerfilID != (int)CondoClub.Regras.Enum.Perfil.Morador)
                {
                    ModelState.Remove("Fraccao");
                }

                if (!registo.Password.Equals(registo.ConfirmarPassword))
                {
                    ModelState.AddModelError("Password", Resources.Erro.PasswordsIncorrectas);
                    return View("Utilizador", registo);
                }

                if (registo.Password.Length < Util.PasswordMinLength)
                {
                    ModelState.AddModelError("PasswordPequena", String.Format(Resources.Erro.TamanhoMinimo, "Password", 
                        Util.PasswordMinLength));
                    return View("Utilizador", registo);
                }

                if (ModelState.IsValid)
                {
                    new Regras.Utilizador().Registar(registo.ToBDModel());
                    return RedirectToAction("Sucesso");
                }
                else
                    throw new Regras.Exceptions.DadosIncorrectos();
            }
            catch (Regras.Exceptions.EmailRepetido)
            {
                ModelState.AddModelError("EmailRepetido", Resources.Erro.EmailRepetido);
                return View("Utilizador", registo);
            }
            catch (Regras.Exceptions.DadosIncorrectos ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Utilizador", registo);
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return RedirectToAction("Index", "Erro");
            }
        }


        [AllowAnonymous]
        public ActionResult Empresa()
        {
            Models.EmpresaRegisto model = TempData["model_retroceder"] as Models.EmpresaRegisto;

            try
            {
                TempData.Clear();

                if (model == null)
                    model = new Models.EmpresaRegisto();

                return View("Empresa", model);
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return RedirectToAction("Index", "Erro");
            }
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegistarEmpresa(Models.EmpresaRegisto registo)
        {
            try
            {
                if (string.IsNullOrEmpty(registo.Latitude) || string.IsNullOrEmpty(registo.Longitude))
                {
                    ModelState.AddModelError("GoogleMap", Resources.Erro.MoradaGoogleMaps);
                    return View("Empresa", registo);
                }

                if (ModelState.IsValid)
                {
                    Regras.BD.Empresa obj = registo.ToBDModel();
                    new Regras.Empresa().Registar(obj);

                    string cifra = Regras.Util.UrlEncode(Regras.Util.Cifra(
                        ((int)Regras.Enum.Perfil.Empresa) + ";" + obj.EmpresaID
                    ));

                    return RedirectToAction("Utilizador", new { @cifra = cifra });
                }
                else
                    throw new Regras.Exceptions.DadosIncorrectos();
            }
            catch (Regras.Exceptions.DadosIncorrectos ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Empresa", registo);
            }
            catch (Regras.Exceptions.DesignacaoRepetida)
            {
                ModelState.AddModelError("DesignacaoRepetida", Resources.Erro.DesignacaoRepetida);
                return View("Empresa", registo);
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return RedirectToAction("Index", "Erro");
            }
        }


        [AllowAnonymous]
        public ActionResult Fornecedor()
        {
            Models.FornecedorRegisto model = TempData["model_retroceder"] as Models.FornecedorRegisto;

            try
            {
                TempData.Clear();

                if (model == null)
                    model = new Models.FornecedorRegisto();

                return View("Fornecedor", model);
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return RedirectToAction("Index", "Erro");
            }
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegistarFornecedor(Models.FornecedorRegisto registo)
        {
            try
            {
                if (string.IsNullOrEmpty(registo.Latitude) || string.IsNullOrEmpty(registo.Longitude))
                {
                    ModelState.AddModelError("GoogleMap", Resources.Erro.MoradaGoogleMaps);
                    return View("Fornecedor", registo);
                }

                if (ModelState.IsValid)
                {
                    Regras.Fornecedor regras = new Regras.Fornecedor();

                    Regras.BD.Fornecedor obj = registo.ToBDModel();
                    regras.Registar(obj);

                    string cifra = Regras.Util.UrlEncode(Regras.Util.Cifra(
                        ((int)Regras.Enum.Perfil.Fornecedor) + ";" + obj.FornecedorID
                    ));

                    return RedirectToAction("Utilizador", new { @cifra = cifra });
                }
                else
                    throw new Regras.Exceptions.DadosIncorrectos();
            }
            catch (Regras.Exceptions.DadosIncorrectos ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Fornecedor", registo);
            }
            catch (Regras.Exceptions.DesignacaoRepetida)
            {
                ModelState.AddModelError("DesignacaoRepetida", Resources.Erro.DesignacaoRepetida);
                return View("Fornecedor", registo);
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return RedirectToAction("Index", "Erro");
            }
        }


        [AllowAnonymous]
        public ActionResult Retroceder(string cifra)
        {
            try
            {
                string[] parametrosDecifrados = Regras.Util.Decifra(Regras.Util.UrlDecode(cifra)).Split(';');

                Regras.Enum.Perfil perfil;
                long id;

                if(!Enum.TryParse(parametrosDecifrados[0], out perfil) || !long.TryParse(parametrosDecifrados[1], out id))
                    throw new Regras.Exceptions.DadosIncorrectos();

                return Retroceder(id, perfil);
            }
            catch (Regras.Exceptions.DadosIncorrectos ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return RedirectToAction("Index", "Erro");
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return RedirectToAction("Index", "Erro");
            }
        }


        [AllowAnonymous]
        public ActionResult CondicoesGerais()
        {
            try
            {
                return View("CondicoesGerais");
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return RedirectToAction("Index", "Erro");
            }
        }


        [AllowAnonymous]
        public ActionResult Sucesso()
        {
            try
            {
                return View("Sucesso");
            }
            catch (Exception ex)
            {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return RedirectToAction("Index", "Erro");
            }
        }



        #region Métodos auxiliares

        private ActionResult Retroceder(long id, Regras.Enum.Perfil perfil)
        {
            switch (perfil)
            {
                case CondoClub.Regras.Enum.Perfil.Empresa:
                    return RetrocederEmpresa(id);

                case CondoClub.Regras.Enum.Perfil.Síndico:
                    return RetrocederCondominio(id);

                case CondoClub.Regras.Enum.Perfil.Fornecedor:
                    return RetrocederFornecedor(id);
            }

            throw new Regras.Exceptions.DadosIncorrectos();
        }


        private ActionResult RetrocederCondominio(long id)
        {
            TempData["model_retroceder"] = new Models.CondominioRegisto(
                new Regras.Condominio().RegistarRetroceder(id)
            );

            return RedirectToAction("Condominio", new { @cifra = string.Empty });
        }


        private ActionResult RetrocederEmpresa(long id)
        {
            TempData["model_retroceder"] = new Models.EmpresaRegisto(
                new Regras.Empresa().RegistarRetroceder(id)
            );

            return RedirectToAction("Empresa", new { @cifra = string.Empty });
        }


        private ActionResult RetrocederFornecedor(long id)
        {
            TempData["model_retroceder"] = new Models.FornecedorRegisto(
                new Regras.Fornecedor().RegistarRetroceder(id)
            );

            return RedirectToAction("Fornecedor", new { @cifra = string.Empty });
        }

        #endregion

    }
}
