using System;
using System.Collections.Generic;

namespace CondoClub.Web.Models
{

    public class Agenda
    {
        public Agenda() { }

        public Agenda(Regras.BD.Agenda obj)
        {
            ID = obj.AgendaID;
            CondominioID = obj.CondominioID;
            Designacao = obj.Designacao;
            Telefone = obj.Telefone;
            Email = obj.Email;
            URL = obj.URL;
            Endereco = obj.Endereco;
            Cidade = obj.Cidade;
            CodigoPostal = obj.CodigoPostal;
            Localidade = obj.Localidade;
            Estado = obj.Estado;
            PaisID = obj.PaisID;
            Permissoes = Regras.Agenda.Permissoes(ControladorSite.Utilizador, obj);
        }


        public long? ID { get; set; }

        public long? CondominioID { get; set; }

        [DisplayLocalizado(typeof(Resources.Agenda), "Designacao")]
        [RequiredLocalizado(typeof(Resources.Agenda), "Designacao")]
        [MaxStringLocalizado(200, typeof(Resources.Agenda), "Designacao")]
        public string Designacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Agenda), "Telefone")]
        [RequiredLocalizado(typeof(Resources.Agenda), "Telefone")]
        [MaxStringLocalizado(20, typeof(Resources.Agenda), "Telefone")]
        public string Telefone { get; set; }

        [DisplayLocalizado(typeof(Resources.Agenda), "Email")]
        [FormatoEmailLocalizado(typeof(Resources.Agenda), "Email")]
        [MaxStringLocalizado(200, typeof(Resources.Agenda), "Email")]
        public string Email { get; set; }

        [DisplayLocalizado(typeof(Resources.Agenda), "URL")]
        [MaxStringLocalizado(200, typeof(Resources.Agenda), "URL")]
        public string URL { get; set; }

        [DisplayLocalizado(typeof(Resources.Agenda), "Endereco")]
        [MaxStringLocalizado(400, typeof(Resources.Agenda), "Endereco")]
        public string Endereco { get; set; }

        [DisplayLocalizado(typeof(Resources.Agenda), "Cidade")]
        [MaxStringLocalizado(80, typeof(Resources.Agenda), "Cidade")]
        public string Cidade { get; set; }

        [DisplayLocalizado(typeof(Resources.Agenda), "CodigoPostal")]
        [MaxStringLocalizado(20, typeof(Resources.Agenda), "CodigoPostal")]
        public string CodigoPostal { get; set; }

        [DisplayLocalizado(typeof(Resources.Agenda), "Localidade")]
        [MaxStringLocalizado(80, typeof(Resources.Agenda), "Localidade")]
        public string Localidade { get; set; }

        [DisplayLocalizado(typeof(Resources.Agenda), "Estado")]
        [MaxStringLocalizado(80, typeof(Resources.Agenda), "Estado")]
        public string Estado { get; set; }

        public int? PaisID { get; set; }

        public List<Regras.Enum.Permissao> Permissoes { get; set; }


        public Regras.BD.Agenda ToBDModel()
        {
            Regras.BD.Agenda obj = new Regras.BD.Agenda();
            obj.AgendaID = (ID != null ? Convert.ToInt64(ID) : obj.AgendaID);
            obj.CondominioID = (CondominioID != null ? CondominioID.Value : 0);
            obj.Designacao = Designacao;
            obj.Telefone = Telefone;
            obj.Email = Email;
            obj.URL = URL;
            obj.Endereco = Endereco;
            obj.Cidade = Cidade;
            obj.CodigoPostal = CodigoPostal;
            obj.Localidade = Localidade;
            obj.Estado = Estado;
            return obj;

        }

    }

}