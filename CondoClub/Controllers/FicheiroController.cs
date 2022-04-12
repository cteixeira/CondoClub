using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Linq;

namespace CondoClub.Web.Controllers {

    [Authorize]
    public class FicheiroController : Controller {

        #region Campos

        const int _smallImageComunicadoWidth = 470;
        const int _smallImageComunicadoHeight = 376;

        #endregion


        #region Visualizar

        [OutputCache(Duration = Int32.MaxValue, NoStore = false, VaryByParam = "*", Location = System.Web.UI.OutputCacheLocation.ServerAndClient)]
        public ActionResult ImagemComunicado(string id, int? sz) {
            try {
                if (!String.IsNullOrEmpty(id)) {
                    Regras.Ficheiro regras = new Regras.Ficheiro();

                    long idFicheiro = Convert.ToInt64(id);

                    if (!regras.PermissaoFicheiroComunicado(idFicheiro, ControladorSite.Utilizador))
                        throw new Regras.Exceptions.SemPermissao();

                    Regras.BD.Ficheiro ficheiro = regras.AbrirConteudo(idFicheiro);

                    if (ficheiro != null && Regras.Ficheiro.SuportaFormatoImagem(ficheiro.Extensao.ToLower())) {
                        FileContentResult result;

                        if (sz != null && sz == 1) {
                            //Redimensionar imagem
                            ImageFormat imageFormat = ficheiro.Extensao == "png" ? ImageFormat.Png : ImageFormat.Jpeg;

                            byte[] conteudo = null;

                            using (Stream str = Regras.Util.ResizeStream(
                                new MemoryStream(ficheiro.FicheiroConteudo.Conteudo),
                                imageFormat,
                                _smallImageComunicadoWidth,
                                _smallImageComunicadoHeight
                            )) {
                                conteudo = new byte[str.Length];
                                str.Read(conteudo, 0, (int)str.Length);
                            }

                            result = new FileContentResult(conteudo, ContentTypeConfig.GetContentType(ficheiro.Extensao));
                        } else {
                            result = new FileContentResult(ficheiro.FicheiroConteudo.Conteudo,
                                ContentTypeConfig.GetContentType(ficheiro.Extensao));
                        }

                        return result;
                    }
                }
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return new HttpNotFoundResult();
        }


        [OutputCache(Duration = Int32.MaxValue, NoStore = false, VaryByParam = "*", Location = System.Web.UI.OutputCacheLocation.ServerAndClient)]
        public ActionResult ImagemPublicidade(string id) {
            try {
                if (!String.IsNullOrEmpty(id)) {
                    Regras.Ficheiro regras = new Regras.Ficheiro();

                    long idFicheiro = Convert.ToInt64(id);


                    if (!regras.PermissaoFicheiroPublicidade(idFicheiro, ControladorSite.Utilizador))
                        throw new Regras.Exceptions.SemPermissao();

                    Regras.BD.Ficheiro ficheiro = regras.AbrirConteudo(idFicheiro);

                    if (ficheiro != null && Regras.Ficheiro.SuportaFormatoImagem(ficheiro.Extensao.ToLower())) {

                        return new FileContentResult(ficheiro.FicheiroConteudo.Conteudo, ContentTypeConfig.GetContentType(ficheiro.Extensao));

                    }
                }
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return new HttpNotFoundResult();
        }


        [AllowAnonymous]
        [OutputCache(Duration = Int32.MaxValue, NoStore = false, VaryByParam = "*", Location = System.Web.UI.OutputCacheLocation.ServerAndClient)]
        public ActionResult Avatar(long? id, bool thumbnail = true) {
            try {
                if (id != null && id > 0) {
                    Regras.Ficheiro regras = new Regras.Ficheiro();

                    if (!regras.PermissaoFicheiroAvatar(id.Value, ControladorSite.Utilizador))
                        throw new Regras.Exceptions.SemPermissao();

                    Regras.BD.Ficheiro ficheiro = regras.AbrirConteudo(id.Value);
                    if (ficheiro != null) {
                        FileContentResult result = new FileContentResult(ficheiro.FicheiroConteudo.Conteudo, ContentTypeConfig.GetContentType(ficheiro.Extensao));
                        result.FileDownloadName = String.Concat(ficheiro.Nome, ficheiro.Extensao);
                        return result;
                    }
                }
                if (id == 0) {
                    //id do avatar para grupo de mensagens                
                    return new FilePathResult(@"~\Content\images\avatar_multi_50.png", "image/png");
                }

                if (thumbnail) {
                    return new FilePathResult(@"~\Content\images\avatar_50.png", "image/png");
                }

                return new FilePathResult(@"~\Content\images\avatar_300.jpg", "image/jpg");
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return new HttpNotFoundResult();
        }


        public ActionResult Other(string id) {
            try {
                if (id != null) {
                    long idDecifrado = Convert.ToInt64(Regras.Util.Decifra(Regras.Util.UrlDecode(id)));

                    Regras.Ficheiro regras = new Regras.Ficheiro();

                    if (regras.PermissaoFicheiroArquivo(idDecifrado, ControladorSite.Utilizador)) {
                        Regras.BD.Ficheiro ficheiro = regras.AbrirConteudo(idDecifrado);

                        if (ficheiro != null) {
                            return File(
                                ficheiro.FicheiroConteudo.Conteudo,
                                ContentTypeConfig.GetContentType(ficheiro.Extensao),
                                ficheiro.Nome + "." + ficheiro.Extensao
                            );
                        }
                    } else
                        throw new Regras.Exceptions.SemPermissao();
                }
            } catch (Regras.Exceptions.SemPermissao ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }

            //return new HttpNotFoundResult(); Requer uma página estática para o erro 404
            return null;
        }

        #endregion


        #region Gravar

        public JsonResult GravarFicheiros(IEnumerable<HttpPostedFileBase> files) {
            try {
                if (files == null)
                    return Json(new List<Models.Ficheiro>());

                return Json(GravarFicheiros(files, Regras.Enum.DimensaoFicheiro.Indiferente));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return Json(Resources.Erro.Geral);
            }
        }


        public JsonResult GravarImagemComunicado(IEnumerable<HttpPostedFileBase> files) {
            return GravarImagens(files, Regras.Enum.DimensaoFicheiro.ImagemComunicado);
        }


        public JsonResult GravarImagemPublicidade(IEnumerable<HttpPostedFileBase> files) {
            return GravarImagens(files, Regras.Enum.DimensaoFicheiro.ImagemPublicidade);
        }


        [AllowAnonymous]
        public JsonResult GravarImagemForm(IEnumerable<HttpPostedFileBase> files) {
            return GravarImagens(files, Regras.Enum.DimensaoFicheiro.ImagemFormulario);
        }

        #endregion


        #region Métodos auxiliares


        internal JsonResult GravarImagens(IEnumerable<HttpPostedFileBase> files, Regras.Enum.DimensaoFicheiro dimensao) {
            try {
                if (files == null)
                    return Json(new List<Models.Ficheiro>());

                if (files.Count(f => f.ContentLength > Util.FicheiroMaxSize) > 0) {
                    return Json(String.Format(Resources.Erro.FicheiroTamanhoMaximo,
                        Util.ConvertToMB(Util.FicheiroMaxSize)));
                }

                return Json(GravarFicheiros(files, dimensao));
            } catch (Regras.Exceptions.FormatoImagemInvalido ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return Json(Resources.Erro.FormatoImagemInvalido);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return Json(Resources.Erro.Geral);
            }
        }


        internal IList<Models.Ficheiro> GravarFicheiros(IEnumerable<HttpPostedFileBase> files, Regras.Enum.DimensaoFicheiro dimensao) {
            List<Regras.BD.Ficheiro> retorno = new List<Regras.BD.Ficheiro>();

            if (files == null) return null;

            int pos;
            byte[] conteudo;
            foreach (var file in files) {
                pos = file.FileName.LastIndexOf('.');
                conteudo = ObterConteudo(file.InputStream);

                retorno.Add(new Regras.BD.Ficheiro() {
                    Nome = file.FileName.Substring(0, pos),
                    Extensao = file.FileName.Substring(pos + 1),
                    DataHora = DateTime.Now,
                    UtilizadorID = (ControladorSite.Utilizador != null ? ControladorSite.Utilizador.ID : (long?)null),
                    FicheiroConteudo = new Regras.BD.FicheiroConteudo { Conteudo = conteudo },
                    Tamanho = conteudo.Length
                });
            }

            new Regras.Ficheiro().Inserir(retorno, dimensao);

            IList<Models.Ficheiro> ret = new List<Models.Ficheiro>();
            foreach (Regras.BD.Ficheiro f in retorno) {
                Models.Ficheiro newFile = new Models.Ficheiro(f);
                //retirar o conteudo da resposta json
                newFile.Conteudo = null;
                ret.Add(newFile);
            }

            return ret;
        }


        private byte[] ObterConteudo(Stream stream) {
            byte[] conteudo = new byte[stream.Length];

            using (stream) {
                stream.Read(conteudo, 0, (int)stream.Length);
                return conteudo;
            }
        }

        #endregion

    }
}
