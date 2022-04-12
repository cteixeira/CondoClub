using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CondoClub.Web.Models
{

    public class Condominio
    {
        public Condominio()
        {
            Activo = false;
            Permissoes = Regras.Condominio.Permissoes(ControladorSite.Utilizador);
            ExtratoSocialID = (int)Regras.Enum.ExtratoSocial.Media;
        }

        public Condominio(Regras.BD.Condominio obj)
        {
            ID = obj.CondominioID;
            EmpresaID = obj.EmpresaID;
            Nome = obj.Nome;
            Contribuinte = obj.Contribuinte;
            OpcaoPagamentoID = obj.OpcaoPagamentoID;
            FormaPagamentoID = obj.FormaPagamentoID;
            Fraccoes = obj.Fraccoes;
            ExtratoSocialID = obj.ExtratoSocialID;
            AvatarID = obj.AvatarID;
            Endereco = obj.Endereco;
            Localidade = obj.Localidade;
            Cidade = obj.Cidade;
            CodigoPostal = obj.CodigoPostal;
            Estado = obj.Estado;
            PaisID = obj.PaisID;
            Latitude = obj.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            Longitude = obj.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            Activo = obj.Activo;
            DataActivacao = obj.DataActivacao;
            Permissoes = Regras.Condominio.Permissoes(ControladorSite.Utilizador, obj);

            EmpresaNome = (obj.Empresa != null ? obj.Empresa.Nome : null);
            ExtratoSocialDesignacao = (obj.ExtratoSocial != null ? obj.ExtratoSocial.Designacao : null);
            OpcaoPagamentoDesignacao = obj.OpcaoPagamento.Designacao;
            FormaPagamentoDesignacao = obj.FormaPagamento.Designacao;
            PaisDesignacao = obj.Pais.Designacao;

            Utilizadores = new List<CondominioUtilizador>();
            if (obj.Utilizador.IsLoaded)
            {
                obj.Utilizador.Where(u => u.PerfilUtilizadorID == (int)Regras.Enum.Perfil.Síndico).ToList();
                //Só mostra os síndicos
                foreach (Regras.BD.Utilizador ut in obj.Utilizador.Where(u => u.PerfilUtilizadorID == (int)Regras.Enum.Perfil.Síndico))
                {
                    Utilizadores.Add(new CondominioUtilizador()
                    {
                        Nome = ut.Nome,
                        Fraccao = ut.Fraccao,
                        Activo = ut.Activo
                    });
                }
            }
        }


        public long? ID { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "Empresa")]
        public long? EmpresaID { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "Nome")]
        [RequiredLocalizado(typeof(Resources.Condominio), "Nome")]
        [MaxStringLocalizado(200, typeof(Resources.Condominio), "Nome")]
        public string Nome { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "Contribuinte")]
        [RequiredLocalizado(typeof(Resources.Condominio), "Contribuinte")]
        [MaxStringLocalizado(20, typeof(Resources.Condominio), "Contribuinte")]
        public string Contribuinte { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "OpcaoPagamento")]
        [RequiredLocalizado(typeof(Resources.Condominio), "OpcaoPagamento")]
        public int? OpcaoPagamentoID { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "FormaPagamento")]
        [RequiredLocalizado(typeof(Resources.Condominio), "FormaPagamento")]
        public int? FormaPagamentoID { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "Fraccoes")]
        public short? Fraccoes { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "ExtratoSocial")]
        [RequiredLocalizado(typeof(Resources.Condominio), "ExtratoSocial")]
        public int? ExtratoSocialID { get; set; }

        public long? AvatarID { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "Endereco")]
        [RequiredLocalizado(typeof(Resources.Condominio), "Endereco")]
        [MaxStringLocalizado(400, typeof(Resources.Condominio), "Endereco")]
        public string Endereco { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "Localidade")]
        [MaxStringLocalizado(80, typeof(Resources.Condominio), "Localidade")]
        public string Localidade { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "Cidade")]
        [RequiredLocalizado(typeof(Resources.Condominio), "Cidade")]
        [MaxStringLocalizado(80, typeof(Resources.Condominio), "Cidade")]
        public string Cidade { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "CodigoPostal")]
        [RequiredLocalizado(typeof(Resources.Condominio), "CodigoPostal")]
        [MaxStringLocalizado(20, typeof(Resources.Condominio), "CodigoPostal")]
        public string CodigoPostal { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "Estado")]
        [MaxStringLocalizado(80, typeof(Resources.Condominio), "Estado")]
        public string Estado { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "Pais")]
        [RequiredLocalizado(typeof(Resources.Condominio), "Pais")]
        public int? PaisID { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "Latitude")]
        [RequiredLocalizado(typeof(Resources.Condominio), "Latitude")]
        [MaxStringLocalizado(40, typeof(Resources.Condominio), "Latitude")]
        public string Latitude { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "Longitude")]
        [RequiredLocalizado(typeof(Resources.Condominio), "Longitude")]
        [MaxStringLocalizado(40, typeof(Resources.Condominio), "Longitude")]
        public string Longitude { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "Activo")]
        [RequiredLocalizado(typeof(Resources.Condominio), "Activo")]
        public bool Activo { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "DataActivacao")]
        public DateTime? DataActivacao { get; set; }

        public List<Regras.CondominioPermissao> Permissoes { get; set; }

        /*Visualização*/
        public string EmpresaNome { get; set; }
        public string ExtratoSocialDesignacao { get; set; }
        public string OpcaoPagamentoDesignacao { get; set; }
        public string FormaPagamentoDesignacao { get; set; }
        public string PaisDesignacao { get; set; }
        public List<CondominioUtilizador> Utilizadores { get; set; }

        public Regras.BD.Condominio ToBDModel()
        {
            return new Regras.BD.Condominio()
            {
                CondominioID = (this.ID.HasValue ? Convert.ToInt64(this.ID) : 0),
                Nome = this.Nome,
                Contribuinte = this.Contribuinte,
                OpcaoPagamentoID = (this.OpcaoPagamentoID.HasValue ? this.OpcaoPagamentoID.Value : 0),
                FormaPagamentoID = (this.FormaPagamentoID.HasValue ? this.FormaPagamentoID.Value : 0),
                Fraccoes = this.Fraccoes,
                ExtratoSocialID = (this.ExtratoSocialID.HasValue ? this.ExtratoSocialID.Value : 0),
                EmpresaID = this.EmpresaID,
                AvatarID = this.AvatarID,
                Endereco = this.Endereco,
                Localidade = this.Localidade,
                Cidade = this.Cidade,
                CodigoPostal = this.CodigoPostal,
                Estado = this.Estado,
                PaisID = (this.PaisID.HasValue ? this.PaisID.Value : 0),
                Latitude = Convert.ToDouble(this.Latitude, System.Globalization.CultureInfo.InvariantCulture),
                Longitude = Convert.ToDouble(this.Longitude, System.Globalization.CultureInfo.InvariantCulture),
                Activo = this.Activo,
                DataActivacao = this.DataActivacao
            };
        }
    }


    public class CondominioPesquisa
    {
        public CondominioPesquisa() { }

        public CondominioPesquisa(Regras.BD.Condominio obj)
        {
            this.ID = obj.CondominioID;
            this.label = this.Nome = obj.Nome;
        }


        public long? ID { get; set; }

        public string Nome { get; set; }

        public string label { get; set; }
    }


    public class CondominioConvite
    {
        public CondominioConvite() 
        {
            Assunto = Resources.Condominio.ConviteAssuntoDefault;
            Mensagem = Resources.Condominio.ConviteMensagemDefault;
        }

        [DisplayLocalizado(typeof(Resources.Condominio), "Destinatarios")]
        [RequiredLocalizado(typeof(Resources.Condominio), "Destinatarios")]
        public string Destinatarios { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "Assunto")]
        [RequiredLocalizado(typeof(Resources.Condominio), "Assunto")]
        public string Assunto { get; set; }

        [DisplayLocalizado(typeof(Resources.Condominio), "Mensagem")]
        [RequiredLocalizado(typeof(Resources.Condominio), "Mensagem")]
        public string Mensagem { get; set; }
    }


    public class CondominioFiltro
    {
        public CondominioFiltro() { }

        public string TermoPesquisa { get; set; }
        public Regras.CondominioEstado? EstadoPesquisa { get; set; }
        public long? EmpresaPesquisa { get; set; }
    }


    public class CondominioUtilizador
    {
        public string Nome { get; set; }
        public string Fraccao { get; set; }
        public bool Activo { get; set; }
    }
}