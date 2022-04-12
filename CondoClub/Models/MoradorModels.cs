using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CondoClub.Web.Models
{
    public class Morador
    {
        public Morador() 
        {
            this.Activo = true;
            Permissoes = Regras.Morador.Permissoes(ControladorSite.Utilizador);
        }

        public Morador(Regras.BD.Utilizador obj) 
        {
            this.ID = obj.UtilizadorID;
            this.PerfilUtilizadorID = obj.PerfilUtilizadorID;
            this.PerfilDesignacao = obj.PerfilUtilizador.Designacao;
            this.Email = obj.Email;
            this.ConfirmarPassword = this.Password = obj.Password;
            this.Nome = obj.Nome;
            this.AvatarID = obj.AvatarID;
            this.Activo = obj.Activo;
            this.CondominioID = obj.CondominioID;
            this.Fraccao = obj.Fraccao;
            Permissoes = Regras.Morador.Permissoes(ControladorSite.Utilizador, obj);
        }


        public long? ID { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "PerfilUtilizador")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "PerfilUtilizador")]
        public int PerfilUtilizadorID { get; set; }

        public string PerfilDesignacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Email")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "Email")]
        [FormatoEmailLocalizado(typeof(Resources.Utilizador), "Email")]
        [MaxStringLocalizado(200, typeof(Resources.Utilizador), "Email")]
        [DataType(DataType.Text)]
        public string Email { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Password")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "Password")]
        [MaxStringLocalizado(200, typeof(Resources.Utilizador), "Email")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "ConfirmarPassword")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "ConfirmarPassword")]
        [MaxStringLocalizado(200, typeof(Resources.Utilizador), "ConfirmarPassword")]
        [DataType(DataType.Password)]
        public string ConfirmarPassword { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Nome")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "Nome")]
        [MaxStringLocalizado(200, typeof(Resources.Utilizador), "Email")]
        public string Nome { get; set; }

        public long? AvatarID { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Activo")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "Activo")]
        [DataType(DataType.Text)]
        public bool Activo { get; set; }

        public long? CondominioID { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Fraccao")]
        [MaxStringLocalizado(50, typeof(Resources.Utilizador), "Fraccao")]
        public string Fraccao { get; set; }

        public List<Regras.Enum.Permissao> Permissoes { get; set; }


        public Regras.BD.Utilizador ToBDModel()
        {
            Regras.BD.Utilizador obj = new Regras.BD.Utilizador();

            obj.UtilizadorID = (this.ID != null ? Convert.ToInt64(this.ID) : obj.UtilizadorID);
            obj.PerfilUtilizadorID = this.PerfilUtilizadorID;
            obj.Email = this.Email;
            obj.Password = this.Password;
            obj.Nome = this.Nome;
            obj.AvatarID = this.AvatarID;
            obj.Activo = this.Activo;
            obj.CondominioID = this.CondominioID;
            obj.Fraccao = this.Fraccao;

            return obj;
        }


    }


    public class MoradorConvite
    {
        public MoradorConvite()
        {
            Assunto = Resources.Morador.ConviteAssuntoDefault;
            Mensagem = Resources.Morador.ConviteMensagemDefault;
        }

        [DisplayLocalizado(typeof(Resources.Morador), "Destinatarios")]
        [RequiredLocalizado(typeof(Resources.Morador), "Destinatarios")]
        public string Destinatarios { get; set; }

        [DisplayLocalizado(typeof(Resources.Morador), "Assunto")]
        [RequiredLocalizado(typeof(Resources.Morador), "Assunto")]
        public string Assunto { get; set; }

        [DisplayLocalizado(typeof(Resources.Morador), "Mensagem")]
        [RequiredLocalizado(typeof(Resources.Morador), "Mensagem")]
        public string Mensagem { get; set; }
    }
}