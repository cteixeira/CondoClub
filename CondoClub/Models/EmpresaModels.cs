using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CondoClub.Web.Models
{

    public class EmpresaDropDown
    {
        public EmpresaDropDown(Regras.BD.Empresa obj)
        {
            ID = obj.EmpresaID;
            Nome = obj.Nome;
        }


        public long? ID { get; set; }

        public string Nome { get; set; }


        public Regras.BD.Empresa ToBDModel()
        {
            Regras.BD.Empresa obj = new Regras.BD.Empresa();
            obj.EmpresaID = (ID != null ? Convert.ToInt64(ID) : obj.EmpresaID);
            obj.Nome = Nome;

            return obj;
        }
    }


    public class Empresa
    {
        public Empresa ()
        {
            Activo = false;
            Permissoes = Regras.Empresa.Permissoes(ControladorSite.Utilizador);
        }

        public Empresa(Regras.BD.Empresa obj)
        {
            ID = obj.EmpresaID;
            Nome = obj.Nome;
            Contribuinte = obj.Contribuinte;
            AvatarID = obj.AvatarID;
            Endereco = obj.Endereco;
            Localidade = obj.Localidade;
            Cidade = obj.Cidade;
            CodigoPostal = obj.CodigoPostal;
            Estado = obj.Estado;
            PaisID = obj.PaisID;
            Activo = obj.Activo;
            DataActivacao = obj.DataActivacao;
            Latitude = obj.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            Longitude = obj.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            Permissoes = Regras.Empresa.Permissoes(ControladorSite.Utilizador, obj);

            PaisDesignacao = obj.Pais.Designacao;

            Utilizadores = new List<EmpresaUtilizador>();
            if (obj.Utilizador.IsLoaded)
            {
                foreach (Regras.BD.Utilizador ut in obj.Utilizador)
                {
                    Utilizadores.Add(new EmpresaUtilizador()
                    {
                        Nome = ut.Nome,
                        Email = ut.Email,
                        Activo = ut.Activo
                    });
                }
            }
        }


        public long? ID { get; set; }

        [DisplayLocalizado(typeof(Resources.Empresa), "Nome")]
        [RequiredLocalizado(typeof(Resources.Empresa), "Nome")]
        [MaxStringLocalizado(200, typeof(Resources.Empresa), "Nome")]
        public string Nome { get; set; }

        [DisplayLocalizado(typeof(Resources.Empresa), "Contribuinte")]
        [RequiredLocalizado(typeof(Resources.Empresa), "Contribuinte")]
        [MaxStringLocalizado(20, typeof(Resources.Empresa), "Contribuinte")]
        public string Contribuinte { get; set; }

        public long? AvatarID { get; set; }

        [DisplayLocalizado(typeof(Resources.Empresa), "Endereco")]
        [RequiredLocalizado(typeof(Resources.Empresa), "Endereco")]
        [MaxStringLocalizado(400, typeof(Resources.Empresa), "Endereco")]
        public string Endereco { get; set; }

        [DisplayLocalizado(typeof(Resources.Empresa), "Localidade")]
        [MaxStringLocalizado(80, typeof(Resources.Empresa), "Localidade")]
        public string Localidade { get; set; }

        [DisplayLocalizado(typeof(Resources.Empresa), "Cidade")]
        [RequiredLocalizado(typeof(Resources.Empresa), "Cidade")]
        [MaxStringLocalizado(80, typeof(Resources.Empresa), "Cidade")]
        public string Cidade { get; set; }

        [DisplayLocalizado(typeof(Resources.Empresa), "CodigoPostal")]
        [RequiredLocalizado(typeof(Resources.Empresa), "CodigoPostal")]
        [MaxStringLocalizado(20, typeof(Resources.Empresa), "CodigoPostal")]
        public string CodigoPostal { get; set; }

        [DisplayLocalizado(typeof(Resources.Empresa), "Estado")]
        [MaxStringLocalizado(80, typeof(Resources.Empresa), "Estado")]
        public string Estado { get; set; }

        [DisplayLocalizado(typeof(Resources.Empresa), "Pais")]
        [RequiredLocalizado(typeof(Resources.Empresa), "Pais")]
        public int? PaisID { get; set; }

        [DisplayLocalizado(typeof(Resources.Empresa), "Activo")]
        public bool Activo { get; set; }

        [DisplayLocalizado(typeof(Resources.Empresa), "DataActivacao")]
        public DateTime? DataActivacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Empresa), "Latitude")]
        [RequiredLocalizado(typeof(Resources.Empresa), "Latitude")]
        [MaxStringLocalizado(40, typeof(Resources.Empresa), "Latitude")]
        public string Latitude { get; set; }

        [DisplayLocalizado(typeof(Resources.Empresa), "Longitude")]
        [RequiredLocalizado(typeof(Resources.Empresa), "Longitude")]
        [MaxStringLocalizado(40, typeof(Resources.Empresa), "Longitude")]
        public string Longitude { get; set; }

        public List<Regras.EmpresaPermissao> Permissoes { get; set; }

        /*Visualização*/
        public string PaisDesignacao { get; set; }
        public List<EmpresaUtilizador> Utilizadores { get; set; }

        public Regras.BD.Empresa ToBDModel()
        {
            return new Regras.BD.Empresa()
            {
                EmpresaID = (this.ID != null ? Convert.ToInt64(this.ID) : 0),
                Nome = this.Nome,
                Contribuinte = this.Contribuinte,
                AvatarID = this.AvatarID,
                Endereco = this.Endereco,
                Localidade = this.Localidade,
                Cidade = this.Cidade,
                CodigoPostal = this.CodigoPostal,
                Estado = this.Estado,
                PaisID = (this.PaisID.HasValue ? this.PaisID.Value : 0),
                Activo = this.Activo,
                DataActivacao = this.DataActivacao,
                Latitude = Convert.ToDouble(this.Latitude, System.Globalization.CultureInfo.InvariantCulture),
                Longitude = Convert.ToDouble(this.Longitude, System.Globalization.CultureInfo.InvariantCulture),
            };
        }
    }


    public class EmpresaConvite
    {
        public EmpresaConvite()
        {
            Assunto = Resources.Empresa.ConviteAssuntoDefault;
            Mensagem = Resources.Empresa.ConviteMensagemDefault;
        }

        [DisplayLocalizado(typeof(Resources.Empresa), "Destinatarios")]
        [RequiredLocalizado(typeof(Resources.Empresa), "Destinatarios")]
        public string Destinatarios { get; set; }

        [DisplayLocalizado(typeof(Resources.Empresa), "Assunto")]
        [RequiredLocalizado(typeof(Resources.Empresa), "Assunto")]
        public string Assunto { get; set; }

        [DisplayLocalizado(typeof(Resources.Empresa), "Mensagem")]
        [RequiredLocalizado(typeof(Resources.Empresa), "Mensagem")]
        public string Mensagem { get; set; }
    }


    public class EmpresaUtilizador
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public bool Activo { get; set; }
    }
}