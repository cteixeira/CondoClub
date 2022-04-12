using System;
using System.Collections.Generic;
using System.Web;

namespace CondoClub.Web.Models {

    public class Comunicado {

        public Comunicado() { }

        public Comunicado(Regras.BD.Comunicado obj) {
            ID = obj.ComunicadoID;
            RemetenteID = obj.RemetenteID;

            //se o perfil do utilizador autenticado for empresa ou condoclub, colocar o nome da empresa ou condominio na designação do remetente
            if (Util.ConcatenarComNomeCondominio(obj.Utilizador.UtilizadorID, (Regras.Enum.Perfil)obj.Utilizador.PerfilUtilizadorID)) {
                RemetenteNome = String.Concat(obj.Utilizador.Nome, " (", obj.Utilizador.Condominio.Nome, ")");
            } else if (Util.ConcatenarComNomeEmpresa(obj.Utilizador.UtilizadorID, (Regras.Enum.Perfil)obj.Utilizador.PerfilUtilizadorID)) {
                RemetenteNome = String.Concat(obj.Utilizador.Nome, " (", obj.Utilizador.Empresa.Nome, ")");
            } else {
                RemetenteNome = obj.Utilizador.Nome;
            }

            RemetenteAvatar = obj.Utilizador.AvatarID;
            DataHora = obj.DataHora;
            Texto = Util.ConvertUrlsToLinks(obj.Texto).Replace(Environment.NewLine, "<br/>");
            Video = obj.Video;
            ImagemID = obj.ImagemID;

            Permissoes = Regras.Comunicado.Permissoes(ControladorSite.Utilizador, obj);

            Comentarios = new List<ComunicadoComentario>();
            if (obj.ComunicadoComentario.IsLoaded) {
                foreach (var item in obj.ComunicadoComentario) {
                    if (item.Gosto != null) {
                        if (item.Gosto.Value)
                            totalGostos++;
                        else
                            totalGostos--;

                        if (ControladorSite.Utilizador.ID == item.ComentadorID)
                            Gosto = item.Gosto.Value;
                    }
                    else if (!String.IsNullOrEmpty(item.Comentario)) {
                        totalComentarios++;
                        Comentarios.Add(new ComunicadoComentario(item));
                    }
                }
            }
        }

        public long? ID { get; set; }

        public long RemetenteID { get; set; }
        
        public string RemetenteNome { get; set; }

        public long? RemetenteAvatar { get; set; }

        public long? EmpresaID { get; set; }

        public long? CondominioID { get; set; }

        [RequiredLocalizado(typeof(Resources.Comunicado), "Texto")]
        public string Texto { get; set; }

        public string Video { get; set; }

        public long? ImagemID { get; set; }

        public DateTime DataHora { get; set; }

        public int totalGostos { get; set; }
        
        public int totalComentarios { get; set; }

        public bool Gosto { get; set; }

        public List<ComunicadoComentario> Comentarios { get; set; }

        public List<Regras.Enum.Permissao> Permissoes { get; set; }

        public Regras.BD.Comunicado ToBDModel() {
            Regras.BD.Comunicado obj = new Regras.BD.Comunicado();
            obj.RemetenteID = ControladorSite.Utilizador.ID;
            obj.EmpresaID = ControladorSite.Utilizador.EmpresaID;
            obj.CondominioID = ControladorSite.Utilizador.CondominioID;
            obj.Texto = Texto;
            obj.Video = !String.IsNullOrEmpty(Video) ? GetYouTubeVideoID(Video) : null;
            obj.ImagemID = ImagemID;
            obj.DataHora = DateTime.Now;
            return obj;
        }

        private string GetYouTubeVideoID(string url) {
            try {
                var uri = new Uri(url);
                return HttpUtility.ParseQueryString(uri.Query).Get("v");
            }
            catch {
                return null;
            }
        }

    }


    public class ComunicadoComentario { 
    
        public ComunicadoComentario() { }

        public ComunicadoComentario(Regras.BD.ComunicadoComentario obj) {
            ID = obj.ComunicadoComentarioID;
            ComentadorID = obj.ComentadorID;

            //se o perfil do utilizador autenticado for empresa ou condoclub, colocar o nome da empresa ou condominio na designação do remetente
            if (Util.ConcatenarComNomeCondominio(obj.Utilizador.UtilizadorID, (Regras.Enum.Perfil)obj.Utilizador.PerfilUtilizadorID)) {
                ComentadorNome = String.Concat(obj.Utilizador.Nome, " (", obj.Utilizador.Condominio.Nome, ")");
            } else if (Util.ConcatenarComNomeEmpresa(obj.Utilizador.UtilizadorID, (Regras.Enum.Perfil)obj.Utilizador.PerfilUtilizadorID)) {
                ComentadorNome = String.Concat(obj.Utilizador.Nome, " (", obj.Utilizador.Empresa.Nome, ")");
            } else {
                ComentadorNome = obj.Utilizador.Nome;
            }

            ComentadorAvatar = obj.Utilizador.AvatarID;
            Gosto = obj.Gosto;
            Comentario = obj.Comentario;
            DataHora = obj.DataHora;
            
            Permissoes = Regras.Comunicado.PermissoesComentario(ControladorSite.Utilizador, obj);
        }

        public long? ID { get; set; }

        public long ComunicadoID { get; set; }

        public long ComentadorID { get; set; }
        
        public string ComentadorNome { get; set; }

        public long? ComentadorAvatar { get; set; }

        public bool? Gosto { get; set; }

        public string Comentario { get; set; }

        public DateTime DataHora { get; set; }

        public List<Regras.Enum.Permissao> Permissoes { get; set; }

        public Regras.BD.ComunicadoComentario ToBDModel() {
            Regras.BD.ComunicadoComentario obj = new Regras.BD.ComunicadoComentario();
            obj.ComunicadoID = ComunicadoID;
            obj.ComentadorID = ControladorSite.Utilizador.ID;
            obj.Gosto = Gosto;
            obj.Comentario = !String.IsNullOrEmpty(Comentario) ? Comentario.Trim() : null;
            obj.DataHora = DateTime.Now;
            return obj;
        }
    }

}
