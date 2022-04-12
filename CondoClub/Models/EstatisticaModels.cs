using System;
using System.Collections.Generic;

namespace CondoClub.Web.Models.Estatistica {

    public class Pesquisa {

        public Pesquisa() {
            Inicio = DateTime.Now.AddDays(-7).Date;
            Fim = DateTime.Now.Date;
            TipoEstatistica = Regras.Enum.TipoEstatistica.NovoComunicado;
        }

        [DisplayLocalizado(typeof(Resources.Estatistica), "Inicio")]
        [RequiredLocalizado(typeof(Resources.Estatistica), "Inicio")]
        public DateTime Inicio { get; set; }

        [DisplayLocalizado(typeof(Resources.Estatistica), "Fim")]
        [RequiredLocalizado(typeof(Resources.Estatistica), "Fim")]
        public DateTime Fim { get; set; }

        [DisplayLocalizado(typeof(Resources.Estatistica), "TipoEstatistica")]
        [RequiredLocalizado(typeof(Resources.Estatistica), "TipoEstatistica")]
        public Regras.Enum.TipoEstatistica TipoEstatistica { get; set; }

        [DisplayLocalizado(typeof(Resources.Estatistica), "Empresa")]
        public long? EmpresaID { get; set; }

        public IEnumerable<Regras.PagamentoPermissao> Permissoes { get; set; }

    }


    public class Categoria {

        public Categoria(Regras.Estatistica.Categoria obj) {
            Designacao = obj.Designacao;
            Grupo1 = obj.Grupo1;
            Grupo2 = obj.Grupo2;
            Data = obj.Data.ToString("yyyy-MM-dd");
            Total = obj.Total;
        }

        public string Designacao { get; set; }

        public string Grupo1 { get; set; }

        public string Grupo2 { get; set; }

        public string Data { get; set; }
        
        public long Total { get; set; }

    }

}