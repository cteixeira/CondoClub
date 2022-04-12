using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CondoClub.Web.Models {

    public class Publicidade {

        public Publicidade() {
            Permissoes = new List<Regras.PublicidadePermissao>();
            Inicio = DateTime.Now.AddDays(1);
            Fim = DateTime.Now.AddDays(16);
            ZonaID = (Int32)Regras.Enum.ZonaPublicidade.Topo;
        }

        public Publicidade(Regras.BD.Publicidade obj) {
            if (obj.PublicidadeID > 0) {
                IdPublicidade = obj.PublicidadeID;
            } else {
                IdPublicidade = null;
            }
            FornecedorDesignacao = obj.Fornecedor.Nome;
            Titulo = obj.Titulo;
            DataCriacao = obj.DataCriacao;
            ZonaID = obj.ZonaPublicidadeID;
            ZonaDesignacao = obj.ZonaPublicidade.Designacao;
            ImagemID = obj.ImagemID;
            Texto = obj.Texto;
            Url = obj.URL;
            //RaioAccao = obj.RaioAccao;
            Inicio = obj.Inicio;
            Fim = obj.Fim;
            Permissoes = Regras.Publicidade.Permissoes(ControladorSite.Utilizador, obj);
            Aprovado = obj.Aprovado;
            DataAprovacao = obj.DataAprovacao;
            Pago = obj.Pago;
            if (obj.Valor.HasValue) {
                Preco = obj.Valor.Value;
            }
        }

        public long? IdPublicidade { get; set; }

        public string HeaderLista {
            get {

                if (ControladorSite.Utilizador.Perfil == Regras.Enum.Perfil.CondoClub) {
                    return String.Format("{0} - {1}", FornecedorDesignacao, Titulo);
                }

                return Titulo;

            }
        }

        [DisplayLocalizado(typeof(Resources.Publicidade), "Fornecedor")]
        public string FornecedorDesignacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Publicidade), "Titulo")]
        [RequiredLocalizado(typeof(Resources.Publicidade), "Titulo")]
        [MaxStringLocalizado(25, typeof(Resources.Publicidade), "Titulo")]
        public string Titulo { get; set; }
        public DateTime DataCriacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Publicidade), "Zona")]
        [RequiredLocalizado(typeof(Resources.Publicidade), "Zona")]
        public int ZonaID { get; set; }
        public string ZonaDesignacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Publicidade), "Texto")]
        [MaxStringLocalizado(100, typeof(Resources.Publicidade), "Texto")]
        public string Texto { get; set; }

        [DisplayLocalizado(typeof(Resources.Publicidade), "Imagem")]
        public long? ImagemID { get; set; }


        [DisplayLocalizado(typeof(Resources.Publicidade), "Url")]
        public string Url { get; set; }

        //[DisplayLocalizado(typeof(Resources.Publicidade), "RaioAccao")]
        //[FormatoNumeroInteiroLocalizado(typeof(Resources.Publicidade), "RaioAccao")]
        //[RangeLocalizado(1, Int32.MaxValue, typeof(Resources.Publicidade), "RaioAccao")]
        //[RequiredLocalizado(typeof(Resources.Publicidade), "RaioAccao")]
        //public int? RaioAccao { get; set; }

        
        [DisplayLocalizado(typeof(Resources.Publicidade), "Inicio")]
        [RequiredLocalizado(typeof(Resources.Publicidade), "Inicio")]
        [FormatoDataLocalizado(typeof(Resources.Publicidade), "Inicio")]
        [IsDateAfterTodayLocalizado(false, typeof(Resources.Publicidade), "Inicio")]
        public DateTime? Inicio { get; set; }

        [DisplayLocalizado(typeof(Resources.Publicidade), "Fim")]
        [RequiredLocalizado(typeof(Resources.Publicidade), "Fim")]
        [FormatoDataLocalizado(typeof(Resources.Publicidade), "Fim")]
        [IsDateAfterLocalizado("Inicio", true, typeof(Resources.Publicidade), "Inicio", "Fim")]
        public DateTime? Fim { get; set; }

        [DisplayLocalizado(typeof(Resources.Publicidade), "Aprovado")]
        public bool Aprovado { get; set; }

        [DisplayLocalizado(typeof(Resources.Publicidade), "DataAprovacao")]
        public DateTime? DataAprovacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Publicidade), "Pago")]
        public bool Pago { get; set; }

        [DisplayLocalizado(typeof(Resources.Publicidade), "Preco")]
        public decimal Preco { get; private set; }

        public IEnumerable<Regras.PublicidadePermissao> Permissoes { get; set; }

        public Regras.BD.Publicidade ToBDModel() {
            return new Regras.BD.Publicidade() {

                PublicidadeID = IdPublicidade.HasValue ? IdPublicidade.Value : 0,
                ZonaPublicidadeID = ZonaID,
                Titulo = Titulo,
                Texto = Texto,
                ImagemID = ImagemID,
                URL = Url,
                //RaioAccao = RaioAccao.Value,
                Inicio = Inicio.Value,
                Fim = Fim.Value,
                Aprovado = Aprovado
            };
        }

    }

    public class PublicidadeVisualizar {

        public PublicidadeVisualizar(Regras.BD.Publicidade obj) {
            ID = obj.PublicidadeID;
            Titulo = obj.Titulo;
            ImagemID = obj.ImagemID;
            Texto = obj.Texto;
            Url = obj.URL;
            ZonaPublicidade = (Regras.Enum.ZonaPublicidade)obj.ZonaPublicidadeID;
        }

        public string Titulo { get; set; }

        public long ID { get; set; }
        public string IDCifrado {
            get {
                return Regras.Util.UrlEncode(CondoClub.Regras.Util.Cifra(ID.ToString()));
            }
        }

        public long? ImagemID { get; set; }

        public string Texto { get; set; }

        public string Url { get; set; }

        public Regras.Enum.ZonaPublicidade ZonaPublicidade { get; set; }

    }

}