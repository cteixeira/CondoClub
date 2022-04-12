using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CondoClub.Web.Controllers {

    [Authorize]
    public class MensagemController : Controller {

        #region Propriedades e Constantes

        private int nrDestinatarios = 100;

        #endregion

        #region Acções

        [HttpGet]
        public ActionResult Index() {
            try {

                List<Regras.MensagemPermissao> permissoes = Regras.Mensagem.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.MensagemPermissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);
                return View(ConstroiListaGrupos());

            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }

        public PartialViewResult _Detalhe(long id) {
            try {
                IEnumerable<Models.Mensagem> msgs = ConstroiDetalhe(id);
                //marcar as mensagens como vistas
                new Regras.Mensagem().ActualizaMensagensVistas(id, ControladorSite.Utilizador);
                return PartialView(msgs);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }

        [HttpPost]
        public ActionResult Gravar(Models.MensagemNova Mensagem) {
            try {
                List<Regras.MensagemPermissao> permissoes = Regras.Mensagem.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.MensagemPermissao.CriarMensagem))
                    throw new Exception(Resources.Erro.Acesso);

                if (ModelState.IsValid) {

                    List<Regras.MensagemDestinatario> dests = new List<Regras.MensagemDestinatario>();
                    foreach (Models.MensagemDestinatario d in Mensagem.Destinatarios) {
                        dests.Add(d.ToBDModel());
                    }

                    new Regras.Mensagem().Inserir(Mensagem.Texto, dests, Mensagem.Ficheiros, ControladorSite.Utilizador);

                    return PartialView("_Lista", ConstroiListaGrupos());
                } else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }

        [HttpPost]
        public ActionResult ResponderMensagem(Models.Mensagem registo) {
            try {

                if (!ModelState.IsValid) {
                    throw new Regras.Exceptions.DadosIncorrectos();
                }

                List<Regras.MensagemPermissao> permissoes = Regras.Mensagem.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.MensagemPermissao.ResponderMensagem))
                    throw new Exception(Resources.Erro.Acesso);

                if (ModelState.IsValid) {
                    Regras.Mensagem mRegras = new Regras.Mensagem();
                    mRegras.InserirResposta(registo.ToBDModel(), ControladorSite.Utilizador);
                    return PartialView("_Detalhe", ConstroiDetalhe(registo.RespostaID.Value));
                } else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            } catch (Regras.Exceptions.DadosIncorrectos) {
                return JavaScript(String.Format("alert('{0}')", Resources.Mensagem.ErroSemTexto));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }

        [HttpGet]
        public JsonResult SeleccionaMensagemDestinatario(string term) {
            try {
                IEnumerable<Regras.MensagemDestinatario> utilizadoresDest = Regras.Mensagem.ListaDestinatarios(term, ControladorSite.Utilizador, nrDestinatarios);
                List<Models.MensagemDestinatario> destinatarios = new List<Models.MensagemDestinatario>();
                foreach (Regras.MensagemDestinatario u in utilizadoresDest) {
                    destinatarios.Add(new Models.MensagemDestinatario(u));
                }

                return Json(destinatarios, JsonRequestBehavior.AllowGet);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return null;
            }
        }

        [HttpGet]
        public ActionResult SeleccionaMensagemFicheiro(string id) {
            try {
                if (!String.IsNullOrEmpty(id)) {
                    long idDecifrado;
                    if (long.TryParse(Regras.Util.Decifra(Regras.Util.UrlDecode(id)), out idDecifrado)) {
                        Regras.BD.Ficheiro ficheiro = new Regras.Mensagem().SeleccionaMensagemFicheiro(idDecifrado, ControladorSite.Utilizador);
                        FileContentResult result = new FileContentResult(ficheiro.FicheiroConteudo.Conteudo, ContentTypeConfig.GetContentType(ficheiro.Extensao));
                        result.FileDownloadName = String.Concat(ficheiro.Nome, ".", ficheiro.Extensao);
                        return result;
                    }
                }
                return null;
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return null;
            }
        }

        #endregion

        #region Funcões Auxiliares

        public List<Models.MensagemGrupo> ConstroiListaGrupos() {

            IEnumerable<Regras.MensagemGrupo> listaObj = Regras.Mensagem.ListaGrupos(ControladorSite.Utilizador);

            List<Models.MensagemGrupo> modelList = new List<Models.MensagemGrupo>();
            foreach (var obj in listaObj) {
                modelList.Add(new Models.MensagemGrupo(obj));
            }

            return modelList;

        }

        public List<Models.Mensagem> ConstroiDetalhe(long MensagemID) {

            IEnumerable<Regras.BD.Mensagem> listaObj = new Regras.Mensagem().DetalheGrupoMensagens(MensagemID, ControladorSite.Utilizador);

            List<Models.Mensagem> modelList = new List<Models.Mensagem>();
            foreach (var obj in listaObj) {
                modelList.Add(new Models.Mensagem(obj));
            }

            return modelList;

        }

        #endregion

    }

}
