using System;
using System.Collections.Generic;

namespace CondoClub.Web.Models {

    public class Pagamento {

        public Pagamento(Regras.RegistoPagamento obj) {
            ID = obj.ID;
            Origem = obj.Origem;
            Designacao = obj.Designacao;
            Inicio = obj.Inicio;
            Fim = obj.Fim;
            Valor = obj.Valor;
            Pago = obj.Pago;
            DataEmissao = obj.DataEmissao;
            FormaPagamento = obj.FormaPagamento;
            ReferenciaPagamento = obj.ReferenciaPagamento;
            DataPagamento = obj.DataPagamento;
            NomeUtilizadorPagamento = obj.UtilizadorPagamento != null ? obj.UtilizadorPagamento.Nome : string.Empty;
            CondominioDesignacao = obj.Condominio != null ? obj.Condominio.Nome : string.Empty;
            FornecedorDesignacao = obj.Fornecedor != null ? obj.Fornecedor.Nome : string.Empty;
            Permissoes = Regras.Pagamento.Permissoes(ControladorSite.Utilizador, obj);
        }

        public long ID { get; set; }
        public Regras.Enum.OrigemPagamento Origem { get; set; }
        
        public string Designacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Pagamento), "Inicio")]
        public DateTime Inicio { get; set; }

        [DisplayLocalizado(typeof(Resources.Pagamento), "Fim")]
        public DateTime Fim { get; set; }

        [DisplayLocalizado(typeof(Resources.Pagamento), "Valor")]
        public decimal? Valor { get; set; }

        public bool Pago { get; set; }

        [DisplayLocalizado(typeof(Resources.Pagamento), "DataEmissao")]
        public DateTime? DataEmissao { get; set; }

        public Regras.Enum.FormaPagamento FormaPagamento { get; set; }
        [DisplayLocalizado(typeof(Resources.Pagamento), "FormaPagamento")]
        public string FormaPagamentoDesignacao {
            get { 
                if (FormaPagamento == Regras.Enum.FormaPagamento.Boleto) {
                    return Resources.Pagamento.Boleto;    
                }
                return Resources.Pagamento.CartaoCredito;    
            } 
        }

        [DisplayLocalizado(typeof(Resources.Pagamento), "ReferenciaPagamento")]
        public string ReferenciaPagamento { get; set; }

        [DisplayLocalizado(typeof(Resources.Pagamento), "DataPagamento")]
        public DateTime? DataPagamento { get; set; }

        [DisplayLocalizado(typeof(Resources.Pagamento), "UtilizadorPagamento")]
        public string NomeUtilizadorPagamento { get; set; }

        [DisplayLocalizado(typeof(Resources.Pagamento), "Condominio")]
        public string CondominioDesignacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Pagamento), "Fonecedor")]
        public string FornecedorDesignacao { get; set; }

        public string IdCifrado { 
            get {
                return Regras.Util.UrlEncode(Regras.Util.Cifra(String.Concat(Origem.ToString(), "_", ID.ToString())));
            } 
        }

        public IEnumerable<Regras.PagamentoPermissao> Permissoes { get; set; }

    }

    public class PagamentoValidar {

        public PagamentoValidar() {

        }
        
        public PagamentoValidar(Regras.RegistoPagamento obj) {
            ID = obj.ID;
            Origem = obj.Origem;
            Designacao = obj.Designacao;
            Valor = obj.Valor;
            DataEmissao = obj.DataEmissao;
        }

        public long ID { get; set; }
        public Regras.Enum.OrigemPagamento Origem { get; set; }

        public string Designacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Pagamento), "Valor")]
        public decimal? Valor { get; set; }

        [DisplayLocalizado(typeof(Resources.Pagamento), "DataEmissao")]
        public DateTime? DataEmissao { get; set; }

        [RequiredLocalizado(typeof(Resources.Pagamento), "TipoCartao")]
        [DisplayLocalizado(typeof(Resources.Pagamento), "TipoCartao")]
        public Regras.Enum.TipoCartaoCredito? TipoCartao { get; set; }

        public string IdCifrado {
            get {
                return Regras.Util.UrlEncode(Regras.Util.Cifra(String.Concat(Origem.ToString(), "_", ID.ToString())));
            }
        }

    }

    public class PagamentoFinalizar {

        public PagamentoFinalizar() {

        }

        public PagamentoFinalizar(Regras.RegistoPagamento obj) {
            ID = obj.ID;
            Origem = obj.Origem;
            Designacao = obj.Designacao;
            Valor = obj.Valor;
            DataEmissao = obj.DataEmissao;
            Pago = obj.Pago;
            DataPagamento = obj.DataPagamento;
        }

        public long ID { get; set; }
        public Regras.Enum.OrigemPagamento Origem { get; set; }

        public string Designacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Pagamento), "Valor")]
        public decimal? Valor { get; set; }

        [DisplayLocalizado(typeof(Resources.Pagamento), "DataEmissao")]
        public DateTime? DataEmissao { get; set; }

        [DisplayLocalizado(typeof(Resources.Pagamento), "Pago")]
        public bool Pago { get; set; }

        [DisplayLocalizado(typeof(Resources.Pagamento), "DataPagamento")]
        public DateTime? DataPagamento { get; set; }

        public string IdCifrado {
            get {
                return Regras.Util.UrlEncode(Regras.Util.Cifra(String.Concat(Origem.ToString(), "_", ID.ToString())));
            }
        }

    }

}