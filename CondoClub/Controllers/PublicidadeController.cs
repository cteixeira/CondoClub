using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CondoClub.Web.Controllers {

    [Authorize]
    public class PublicidadeController : Controller {

        #region Constantes

        private const int nrPublicidadesIni = 30;
        private const int nrPublicidadesNext = 10;

        private const int nrPublicidadesTopo = 3;
        private const int nrPublicidadesLateral = 3;

        #endregion

        #region Acções

        public ActionResult Index() {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.Publicidade.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);
                ViewData.Add("nrPagamentosIni", nrPublicidadesIni);
                ViewData.Add("nrPagamentosNext", nrPublicidadesNext);

                return View(ConstroiLista(null, null, 0, nrPublicidadesIni));

            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }

        public PartialViewResult _Lista(string termoPesquisa, bool? Aprovado, int? count) {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.Pagamento.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                int skip = count.HasValue ? count.Value : 0;
                //se o count é 0 então é uma nova pesquisa, carregar o numero de registos iniciais
                int take = (!count.HasValue || count.Value == 0) ? nrPublicidadesIni : nrPublicidadesNext;

                return PartialView("_Lista", ConstroiLista(termoPesquisa, Aprovado, skip, take));

            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }

        public PartialViewResult _DetalheVisualizar(long id) {
            try {

                Models.Publicidade model = new Models.Publicidade(
                    new Regras.Publicidade().Abrir(id, ControladorSite.Utilizador)
                );

                return PartialView("_DetalheVisualizar", model);

            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }

        public PartialViewResult _DetalheEditar(long id) {
            try {

                Models.Publicidade model = new Models.Publicidade(
                    new Regras.Publicidade().Abrir(id, ControladorSite.Utilizador)
                );

                if (!model.Permissoes.Contains(Regras.PublicidadePermissao.Gravar)) {
                    return PartialView("_DetalheVisualizar", model);
                }

                return PartialView("_DetalheEditar", model);

            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }

        [HttpPost]
        public ActionResult Gravar(Models.Publicidade registo) {
            try {
                if (ModelState.IsValid) {

                    if (registo.ZonaID == (int)Regras.Enum.ZonaPublicidade.Topo) {
                        if (!registo.ImagemID.HasValue) {
                            return JavaScript(String.Format("alert('{0}')", Resources.Publicidade.ErroImagemObrigatoriaPublicidadeTopo));
                        }
                    } else { 
                        if(!registo.ImagemID.HasValue && String.IsNullOrEmpty(registo.Texto)) {
                            return JavaScript(String.Format("alert('{0}')", Resources.Publicidade.ErroImagemTextoObrigatorioPublicidadeLateral));
                        }
                    }

                    Regras.Publicidade regras = new Regras.Publicidade();

                    if (!registo.IdPublicidade.HasValue) {
                        Regras.BD.Publicidade bdModel = registo.ToBDModel();
                        regras.Inserir(bdModel, ControladorSite.Utilizador);
                        List<Models.Publicidade> modelList = new List<Models.Publicidade>();
                        modelList.Add(new Models.Publicidade(
                            regras.Abrir(bdModel.PublicidadeID, ControladorSite.Utilizador)
                        ));
                        return PartialView("_Lista", modelList);
                    } else {
                        Regras.BD.Publicidade bdModel = registo.ToBDModel();
                        regras.Actualizar(bdModel, ControladorSite.Utilizador);
                        return PartialView("_DetalheVisualizar", new Models.Publicidade(
                            regras.Abrir(bdModel.PublicidadeID, ControladorSite.Utilizador)
                        ));
                    }
                } else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            } catch (Regras.Exceptions.DesignacaoRepetida) {
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.DesignacaoRepetida));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }

        [HttpDelete]
        public ActionResult Apagar(long id) {
            try {
                if (id > 0) {
                    new Regras.Publicidade().Apagar(id, ControladorSite.Utilizador);
                    return JavaScript("AfterDelete(" + id + ");");
                } else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }

        [HttpPost]
        public ActionResult Duplicar(long id) {
            try {
                if (id > 0) {
                    Regras.BD.Publicidade pDuplicar = new Regras.Publicidade().Duplicar(id, ControladorSite.Utilizador);
                    Models.Publicidade model = new Models.Publicidade(pDuplicar);
                    return PartialView("_Detalhe", model);
                } else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }

        public ActionResult _CalcularPreco(long? PublicidadeID, int ZonaID, DateTime Inicio, DateTime Fim) {
            try {

                decimal preco = Regras.Publicidade.PrecoPublicidade(PublicidadeID, ZonaID, Inicio, Fim, ControladorSite.Utilizador);
                return PartialView("DisplayTemplates/FormataMoeda", preco);

            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }

        public ActionResult Click(string id) {
            try {
                if (!String.IsNullOrEmpty(id)) {

                    long idDecifrado = long.Parse(Regras.Util.Decifra(Regras.Util.UrlDecode(id)));

                    Regras.Publicidade rPublicidade = new Regras.Publicidade();
                    rPublicidade.RegistaVisualizacao(idDecifrado, ControladorSite.Utilizador);
                    Regras.BD.Fornecedor f = new Regras.Fornecedor().AbrirPorPublicidade(idDecifrado, ControladorSite.Utilizador);
                    if (f != null) {
                        return RedirectToAction("Fornecedor", "Servico", new { nome = f.Nome });
                    }
                    return View("Erro", new Exception(Resources.Erro.Geral));
                } else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }

        #endregion

        #region selecção de publicidade a apresenta

        public static IEnumerable<Models.PublicidadeVisualizar> ConstroiPublicidadeTopo() {
            IEnumerable<Regras.BD.Publicidade> publicidades = Regras.Publicidade.ListaPublicidadeMostrar(ControladorSite.Utilizador, Regras.Enum.ZonaPublicidade.Topo, nrPublicidadesTopo);
            List<Models.PublicidadeVisualizar> modelList = new List<Models.PublicidadeVisualizar>();
            foreach (var obj in publicidades) {
                modelList.Add(new Models.PublicidadeVisualizar(obj));
            }

            return modelList;
        }

        public static IEnumerable<Models.PublicidadeVisualizar> ConstroiPublicidadeLateral() {
            IEnumerable<Regras.BD.Publicidade> publicidades = Regras.Publicidade.ListaPublicidadeMostrar(ControladorSite.Utilizador, Regras.Enum.ZonaPublicidade.Lateral, nrPublicidadesLateral);
            List<Models.PublicidadeVisualizar> modelList = new List<Models.PublicidadeVisualizar>();
            foreach (var obj in publicidades) {
                modelList.Add(new Models.PublicidadeVisualizar(obj));
            }

            return modelList;
        }

        #endregion

        #region Funções Auxiliares

        public IEnumerable<Models.Publicidade> ConstroiLista(string termoPesquisa, bool? Aprovado, int skip, int take) {


            IEnumerable<Regras.BD.Publicidade> publicidades = Regras.Publicidade.Lista(termoPesquisa, Aprovado, skip, take, ControladorSite.Utilizador);

            List<Models.Publicidade> modelList = new List<Models.Publicidade>();
            foreach (var obj in publicidades) {
                modelList.Add(new Models.Publicidade(obj));
            }

            return modelList;
        }

        public SelectList ConstroiDropDownEstadoPublicidade() {
            Dictionary<bool, string> estadoPublicidade = new Dictionary<bool, string>();
            estadoPublicidade.Add(true, Resources.Publicidade.Aprovado);
            estadoPublicidade.Add(false, Resources.Publicidade.NaoAprovado);
            return new SelectList(
                estadoPublicidade.Select(x => new { ID = x.Key, Name = x.Value }).ToList(),
                "ID",
                "Name"
            );
        }

        #endregion

    }

}
