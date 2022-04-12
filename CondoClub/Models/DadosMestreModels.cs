using System;

namespace CondoClub.Web.Models.DadosMestre {


    public class _Base {

        public int? ID { get; set; }

        [DisplayLocalizado(typeof(Resources.Geral), "Designacao")]
        [RequiredLocalizado(typeof(Resources.Geral), "Designacao")]
        public string Designacao { get; set; }

    }


    public class Pais : _Base {
        public Pais(Regras.BD.Pais obj) {
            ID = obj.PaisID;
            Designacao = obj.Designacao;
        }
    }


    public class PerfilUtilizador : _Base {
        public PerfilUtilizador(Regras.BD.PerfilUtilizador obj) {
            ID = obj.PerfilUtilizadorID;
            Designacao = obj.Designacao;
        }
    }


    public class TabelaLocalizada : _Base {
        public TabelaLocalizada(Regras.BD.TabelaLocalizada obj) {
            ID = obj.TabelaLocalizadaID;
            Designacao = obj.Designacao;
        }
    }


    public class Categoria : _Base {

        public Categoria() { }

        public Categoria(Regras.BD.Categoria obj) {
            ID = obj.CategoriaID;
            Designacao = obj.Designacao;
        }

        public Regras.BD.Categoria ToBDModel() {
            Regras.BD.Categoria obj = new Regras.BD.Categoria();
            obj.CategoriaID = (ID != null ? Convert.ToInt32(ID) : obj.CategoriaID);
            obj.Designacao = Designacao;
            return obj;
        }

    }


    public class ExtratoSocial : _Base
    {
        public ExtratoSocial(Regras.BD.ExtratoSocial obj)
        {
            ID = obj.ExtratoSocialID;
            Designacao = obj.Designacao;
        }
    }


    public class OpcaoPagamento : _Base
    {
        public OpcaoPagamento(Regras.BD.OpcaoPagamento obj)
        {
            ID = obj.OpcaoPagamentoID;
            Designacao = obj.Designacao;
        }
    }


    public class FormaPagamento : _Base 
    {
        public FormaPagamento(Regras.BD.FormaPagamento obj)
        {
            ID = obj.FormaPagamentoID;
            Designacao = obj.Designacao;
        }
    }


    public class ZonaPublicidade : _Base {
        public ZonaPublicidade(Regras.BD.ZonaPublicidade obj) {
            ID = obj.ZonaPublicidadeID;
            Designacao = obj.Designacao;
        }
    }

}
