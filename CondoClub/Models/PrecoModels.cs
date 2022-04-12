using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CondoClub.Web.Models {

    public class PrecoCondominio {

        public PrecoCondominio() {

        }

        public PrecoCondominio(Regras.BD.PrecoCondominio obj) {

            ID = obj.PrecoCondominioID;
            PaisID = obj.PaisID;
            PaisDesignacao = obj.Pais.Designacao;
            OpcaoPagamentoID = obj.OpcaoPagamentoID;
            OpcaoPagamentoDesignacao = obj.OpcaoPagamento.Designacao;
            ExtratoSocialID = obj.ExtratoSocialID;
            ExtratoSocialDesignacao = obj.ExtratoSocial.Designacao;
            FraccoesAte = obj.FraccoesAte;
            Valor = obj.Valor;

            Permissoes = Regras.PrecoCondominio.Permissoes(ControladorSite.Utilizador, obj);

        }

        public int? ID { get; set; }

        [DisplayLocalizado(typeof(Resources.Preco), "Pais")]
        [RequiredLocalizado(typeof(Resources.Preco), "Pais")]
        public int? PaisID { get; set; }
        public string PaisDesignacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Preco), "OpcaoPagamento")]
        [RequiredLocalizado(typeof(Resources.Preco), "OpcaoPagamento")]
        public int? OpcaoPagamentoID { get; set; }
        public string OpcaoPagamentoDesignacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Preco), "ExtractoSocial")]
        [RequiredLocalizado(typeof(Resources.Preco), "ExtractoSocial")]
        public int? ExtratoSocialID { get; set; }
        public string ExtratoSocialDesignacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Preco), "FraccoesAte")]
        [RequiredLocalizado(typeof(Resources.Preco), "FraccoesAte")]
        [FormatoNumeroInteiroLocalizado(typeof(Resources.Preco), "FraccoesAte")]
        [RangeLocalizado(1, int.MaxValue, typeof(Resources.Preco), "FraccoesAte")]
        public long? FraccoesAte { get; set; }

        [DisplayLocalizado(typeof(Resources.Preco), "Valor")]
        [RequiredLocalizado(typeof(Resources.Preco), "Valor")]
        [RangeDoubleLocalizado(0, double.MaxValue, typeof(Resources.Preco), "Valor")]
        public decimal? Valor { get; set; }

        public List<Regras.Enum.Permissao> Permissoes { get; set; }

        public Regras.BD.PrecoCondominio ToBDModel() {
            Regras.BD.PrecoCondominio obj = new Regras.BD.PrecoCondominio();

            if (ID.HasValue) {
                obj.PrecoCondominioID = ID.Value;
            }
            obj.PaisID = PaisID.Value;
            obj.OpcaoPagamentoID = OpcaoPagamentoID.Value;
            obj.ExtratoSocialID = ExtratoSocialID.Value;
            obj.FraccoesAte = FraccoesAte.Value;
            obj.Valor = Valor.Value;

            return obj;
        }

    }

    public class PrecoFornecedor {

        public PrecoFornecedor() {

        }

        public PrecoFornecedor(Regras.BD.PrecoFornecedor obj) {

            ID = obj.PrecoFornecedorID;
            PaisID = obj.PaisID;
            PaisDesignacao = obj.Pais.Designacao;
            OpcaoPagamentoID = obj.OpcaoPagamentoID;
            OpcaoPagamentoDesignacao = obj.OpcaoPagamento.Designacao;
            FraccoesAte = obj.FraccoesAte;
            Valor = obj.Valor;

            Permissoes = Regras.PrecoFornecedor.Permissoes(ControladorSite.Utilizador, obj);

        }

        public int? ID { get; set; }

        [DisplayLocalizado(typeof(Resources.Preco), "Pais")]
        [RequiredLocalizado(typeof(Resources.Preco), "Pais")]
        public int? PaisID { get; set; }
        public string PaisDesignacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Preco), "OpcaoPagamento")]
        [RequiredLocalizado(typeof(Resources.Preco), "OpcaoPagamento")]
        public int? OpcaoPagamentoID { get; set; }
        public string OpcaoPagamentoDesignacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Preco), "FraccoesAte")]
        [RequiredLocalizado(typeof(Resources.Preco), "FraccoesAte")]
        [FormatoNumeroInteiroLocalizado(typeof(Resources.Preco), "FraccoesAte")]
        [RangeLocalizado(1, int.MaxValue, typeof(Resources.Preco), "FraccoesAte")]
        public long? FraccoesAte { get; set; }

        [DisplayLocalizado(typeof(Resources.Preco), "Valor")]
        [RequiredLocalizado(typeof(Resources.Preco), "Valor")]
        [RangeDoubleLocalizado(0, double.MaxValue, typeof(Resources.Preco), "Valor")]
        public decimal? Valor { get; set; }

        public List<Regras.Enum.Permissao> Permissoes { get; set; }

        public Regras.BD.PrecoFornecedor ToBDModel() {
            Regras.BD.PrecoFornecedor obj = new Regras.BD.PrecoFornecedor();

            if (ID.HasValue) {
                obj.PrecoFornecedorID = ID.Value;
            }
            obj.PaisID = PaisID.Value;
            obj.OpcaoPagamentoID = OpcaoPagamentoID.Value;
            obj.FraccoesAte = FraccoesAte.Value;
            obj.Valor = Valor.Value;

            return obj;
        }

    }

    public class PrecoPublicidade {

        public PrecoPublicidade() {

        }

        public PrecoPublicidade(Regras.BD.PrecoPublicidade obj) {

            ID = obj.PrecoPublicidadeID;
            PaisID = obj.PaisID;
            PaisDesignacao = obj.Pais.Designacao;
            ZonaPublicidadeID = obj.ZonaPublicidadeID;
            ZonaPublicidadeDesignacao = obj.ZonaPublicidade.Designacao;
            FraccoesAte = obj.FraccoesAte;
            Valor = obj.Valor;

            Permissoes = Regras.PrecoPublicidade.Permissoes(ControladorSite.Utilizador, obj);

        }

        public int? ID { get; set; }

        [DisplayLocalizado(typeof(Resources.Preco), "Pais")]
        [RequiredLocalizado(typeof(Resources.Preco), "Pais")]
        public int? PaisID { get; set; }
        public string PaisDesignacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Preco), "ZonaPublicidade")]
        [RequiredLocalizado(typeof(Resources.Preco), "ZonaPublicidade")]
        public int? ZonaPublicidadeID { get; set; }
        public string ZonaPublicidadeDesignacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Preco), "FraccoesAte")]
        [RequiredLocalizado(typeof(Resources.Preco), "FraccoesAte")]
        [FormatoNumeroInteiroLocalizado(typeof(Resources.Preco), "FraccoesAte")]
        [RangeLocalizado(1, int.MaxValue, typeof(Resources.Preco), "FraccoesAte")]
        public long? FraccoesAte { get; set; }

        [DisplayLocalizado(typeof(Resources.Preco), "Valor")]
        [RequiredLocalizado(typeof(Resources.Preco), "Valor")]
        [RangeDoubleLocalizado(0, double.MaxValue, typeof(Resources.Preco), "Valor")]
        public decimal? Valor { get; set; }

        public List<Regras.Enum.Permissao> Permissoes { get; set; }

        public Regras.BD.PrecoPublicidade ToBDModel() {
            Regras.BD.PrecoPublicidade obj = new Regras.BD.PrecoPublicidade();

            if (ID.HasValue) {
                obj.PrecoPublicidadeID = ID.Value;
            }
            obj.PaisID = PaisID.Value;
            obj.ZonaPublicidadeID = ZonaPublicidadeID.Value;
            obj.FraccoesAte = FraccoesAte.Value;
            obj.Valor = Valor.Value;

            return obj;
        }

    }

}