using System;
using System.Collections.Generic;
using System.Linq;

namespace CondoClub.Regras {

    public class Localizacao {

        #region Traduções

        public void TraduzirLista(Enum.TabelaLocalizada tabela, Enum.Pais pais, IEnumerable<BD.PerfilUtilizador> lista) {
            if (pais == Enum.Pais.Brasil)
                return;

            int tabelaID = Convert.ToInt32(tabela);
            int paisID = Convert.ToInt32(pais);

            using (BD.Context ctx = new BD.Context()) {
                IEnumerable<BD.Localizacao> loc = ctx.Localizacao.Where(o => 
                    o.TabelaLocalizadaID == tabelaID &&
                    o.PaisID == paisID);

                foreach (var item in lista) {
                    item.Designacao = loc.FirstOrDefault(o => o.Identificador == item.PerfilUtilizadorID).Designacao;
                }
            }
        }


        public void TraduzirItem(Enum.TabelaLocalizada tabela, Enum.Pais pais, int identificador, ref string item) {
            if (pais == Enum.Pais.Brasil)
                return;

            int tabelaID = Convert.ToInt32(tabela);
            int paisID = Convert.ToInt32(pais);

            using (BD.Context ctx = new BD.Context()) {
                item =  ctx.Localizacao.FirstOrDefault(o => 
                    o.TabelaLocalizadaID == tabelaID && 
                    o.PaisID == paisID && 
                    o.Identificador == identificador).Designacao;
            };
        }

        #endregion
    }
}
