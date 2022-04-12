using System;
using System.IO;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;

namespace CondoClub.Regras {

    public class Ficheiro {

        private _Base<BD.Ficheiro> _base = new _Base<BD.Ficheiro>();

        const int _comunicadoImageWidth = 806;
        const int _comunicadoImageHeight = 644;
        const int _formImageWidth = 265;
        const int _formImageHeight = 265;
        const int _publicidadeImageWidth = 654;
        const int _publicidadeImageHeight = 654;
        const string _formatoImagemSuportado = "png,jpg,jpeg";

        #region Abrir

        public BD.Ficheiro Abrir(long id) {
            return _base.Abrir(id);
        }

        internal BD.Ficheiro Abrir(long id, BD.Context ctx){
            return _base.Abrir(id, ctx);
        }

        public Regras.BD.Ficheiro AbrirConteudo(long ficheiroID) {
            using (BD.Context ctx = new BD.Context()) {
                return ctx.Ficheiro.Include("FicheiroConteudo").Where(f => f.FicheiroID == ficheiroID).FirstOrDefault();
            }
        }

        #endregion

        #region Actualizar

        public void Actualizar(BD.Ficheiro ficheiro) {
            _base.Actualizar(ficheiro);
        }

        internal void Actualizar(BD.Ficheiro ficheiro, BD.Context ctx) {
            _base.Actualizar(ficheiro, ctx);
        }

        #endregion

        #region Apagar

        internal void Apagar(long ficheiroID, BD.Context ctx) {
            _base.Apagar(ficheiroID, ctx);
        }

        #endregion

        #region inserir

        public void Inserir(List<BD.Ficheiro> ficheiros, Enum.DimensaoFicheiro dimensao) {
            using (BD.Context ctx = new BD.Context()) {
                foreach (BD.Ficheiro ficheiro in ficheiros) {
                    ficheiro.Temporario = true;
                    if (dimensao == Enum.DimensaoFicheiro.ImagemComunicado)
                        InserirImagem(ficheiro, _comunicadoImageWidth, _comunicadoImageHeight, ctx);
                    else if (dimensao == Enum.DimensaoFicheiro.ImagemFormulario)
                        InserirImagem(ficheiro, _formImageWidth, _formImageHeight, ctx);
                    else if (dimensao == Enum.DimensaoFicheiro.ImagemPublicidade)
                        InserirImagem(ficheiro, _publicidadeImageWidth, _publicidadeImageHeight, ctx);
                    else
                        Inserir(ficheiro, ctx);
                }
                ctx.SaveChanges();
            }
        }


        internal void InserirImagem(BD.Ficheiro obj, int? maxWidth, int? maxHeight, BD.Context ctx) {
            if (!SuportaFormatoImagem(obj.Extensao.ToLower()))
                throw new Exceptions.FormatoImagemInvalido();

            ImageFormat imageFormat = obj.Extensao.ToLower() == "png" ? ImageFormat.Png : ImageFormat.Jpeg;

            using (Stream ficheiroConteudo = new MemoryStream(obj.FicheiroConteudo.Conteudo)) {
                if (maxWidth.HasValue && maxHeight.HasValue) {
                    using (Stream ficheiroConteudoResized = Util.ResizeStream(ficheiroConteudo, imageFormat,
                        maxWidth.Value, maxHeight.Value)) {
                        obj.FicheiroConteudo.Conteudo = LerConteudoStream(ficheiroConteudoResized);
                    }
                } else {
                    obj.FicheiroConteudo.Conteudo = LerConteudoStream(ficheiroConteudo);
                }
            }

            Inserir(obj, ctx);
        }


        internal void Inserir(CondoClub.Regras.BD.Ficheiro obj, CondoClub.Regras.BD.Context ctx) {
            obj.Temporario = true;
            _base.Inserir(obj, ctx);
        }


        public void Inserir(BD.Ficheiro obj) {
            obj.Temporario = true;
            _base.Inserir(obj);
        }

        #endregion

        #region Permissões


        public bool PermissaoFicheiroAvatar(long id, UtilizadorAutenticado utilizador) {
            if (utilizador != null && (utilizador.Perfil == Enum.Perfil.CondoClub ||
                utilizador.PerfilOriginal == Enum.Perfil.CondoClub)) {
                return true;
            }

            using (BD.Context ctx = new BD.Context()) {
                bool utilizadorIgualNULL = utilizador == null;
                long? utilizadorID = utilizador != null ? utilizador.ID : (long?)null;

                BD.Ficheiro ficheiro = ctx.Ficheiro.FirstOrDefault(f => f.FicheiroID == id && f.Temporario &&
                    (
                        (utilizadorIgualNULL && !f.UtilizadorID.HasValue) ||
                        (!utilizadorIgualNULL && f.UtilizadorID.HasValue && f.UtilizadorID.Value == utilizadorID.Value)
                    )
                );

                if (ficheiro != null)
                    return true;

                return ctx.Utilizador.FirstOrDefault(u => u.AvatarID.HasValue && u.AvatarID.Value == id) != null ||
                    ctx.Condominio.FirstOrDefault(c => c.AvatarID.HasValue && c.AvatarID.Value == id) != null ||
                    ctx.Fornecedor.FirstOrDefault(f => f.AvatarID.HasValue && f.AvatarID.Value == id) != null ||
                    ctx.Empresa.FirstOrDefault(e => e.AvatarID.HasValue && e.AvatarID.Value == id) != null ||
                    ctx.Funcionario.FirstOrDefault(f => f.FotoID.HasValue && f.FotoID.Value == id) != null ||
                    ctx.Veiculo.FirstOrDefault(v => v.FotoID.HasValue && v.FotoID.Value == id) != null;
            }
        }


        public bool PermissaoFicheiroComunicado(long id, UtilizadorAutenticado utilizador) {
            if (utilizador != null && (utilizador.Perfil == Enum.Perfil.CondoClub ||
                utilizador.PerfilOriginal == Enum.Perfil.CondoClub)) {
                return true;
            }

            using (BD.Context ctx = new BD.Context()) {
                BD.Ficheiro ficheiro = ctx.Ficheiro.FirstOrDefault(f => f.FicheiroID == id && f.Temporario && f.UtilizadorID == utilizador.ID);

                if (ficheiro != null)
                    return true;

                return ctx.Comunicado.FirstOrDefault(
                    c => c.ImagemID.HasValue && c.ImagemID.Value == id &&
                    (
                        (!c.CondominioID.HasValue || c.CondominioID == utilizador.CondominioID) ||
                        (!c.EmpresaID.HasValue || c.Condominio.EmpresaID == utilizador.EmpresaID)
                    )
                ) != null;
            }
        }


        public bool PermissaoFicheiroArquivo(long id, UtilizadorAutenticado utilizador) {
            if (utilizador != null && (utilizador.Perfil == Enum.Perfil.CondoClub ||
                utilizador.PerfilOriginal == Enum.Perfil.CondoClub)) {
                return true;
            }

            if (utilizador.Perfil == Enum.Perfil.Fornecedor) {
                return false;
            }

            using (BD.Context ctx = new BD.Context()) {

                BD.ArquivoFicheiro file = ctx.ArquivoFicheiro.FirstOrDefault(af => af.FicheiroID == id);
                if (file == null) {
                    return false;
                }
                if (utilizador != null && utilizador.Perfil == Enum.Perfil.Empresa) {
                    return ctx.Condominio.Where(c => c.EmpresaID.Value == utilizador.EmpresaID.Value).Select(c => c.CondominioID).Contains(file.CondominioID);
                }

                return file.CondominioID == utilizador.CondominioID.Value;

            }
        }


        public bool PermissaoFicheiroPublicidade(long id, UtilizadorAutenticado utilizador) {
            
            if (utilizador == null) {
                return false;
            }

            using (BD.Context ctx = new BD.Context()) {

                //tem permissão se ficheiro estiver associado a uma publicidade, ou estiver como temporário e carregado pelo utilizador
                return ctx.Ficheiro.FirstOrDefault(f => 
                    (f.FicheiroID == id &&
                        (!f.Temporario && f.Publicidade.Count() > 0 || 
                        f.Temporario && f.Utilizador.UtilizadorID == utilizador.ID))
                    ) != null;
            }
        }
     

        #endregion

        #region Métodos auxiliares

        public static bool SuportaFormatoImagem(string extensao) {
            return _formatoImagemSuportado.Contains(extensao.ToLower());
        }

        private byte[] LerConteudoStream(Stream stream) {
            byte[] conteudo = new byte[stream.Length];
            stream.Read(conteudo, 0, (int)stream.Length);
            return conteudo;
        }

        #endregion

    }

}