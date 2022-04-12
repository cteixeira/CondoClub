using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CondoClub.Web.Models
{

    public class Ficheiro
    {
        public Ficheiro(Regras.BD.Ficheiro obj)
        {
            this.ID = obj.FicheiroID;
            //this.IDCifrado = CondoClub.Regras.Util.UrlEncode(CondoClub.Regras.Util.Cifra(this.ID.ToString()));
            this.Nome = obj.Nome;
            this.Extensao = obj.Extensao;
            this.DataHora = obj.DataHora;
            this.UtilizadorID = obj.UtilizadorID;
        }


        public long? ID { get; set; }

        //public string IDCifrado { get; set; }

        public string Nome { get; set; }

        public string Extensao { get; set; }

        public byte[] Conteudo { get; set; }

        public DateTime DataHora { get; set; }

        public long? UtilizadorID { get; set; }


        public Regras.BD.Ficheiro ToBDModel()
        {
            Regras.BD.Ficheiro obj = new Regras.BD.Ficheiro();

            obj.FicheiroID = (this.ID != null ? Convert.ToInt64(this.ID) : obj.FicheiroID);
            obj.Nome = this.Nome;
            obj.Extensao = this.Extensao;
            obj.DataHora = this.DataHora;
            obj.UtilizadorID = (this.UtilizadorID != null ? Convert.ToInt64(this.UtilizadorID) : obj.UtilizadorID);

            return obj;
        }
    }

}