using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CondoClub.Web.Models
{

    public class Veiculo
    {

        public Veiculo() 
        {
            Permissoes = Regras.Veiculo.Permissoes(ControladorSite.Utilizador);
        }

        public Veiculo(Regras.BD.Veiculo obj)
        {
            ID = obj.VeiculoID;
            CondominioID = obj.CondominioID;
            MoradorID = obj.MoradorID;

            if(MoradorID.HasValue)
            {
                Regras.BD.Utilizador ut = new Regras.Utilizador().Abrir(MoradorID.Value);
                MoradorNome = ut.Nome + (!string.IsNullOrEmpty(ut.Fraccao) ? " (" + ut.Fraccao + ")" : string.Empty);
            }
            else
                MoradorNome = null;
            
            FotoID = obj.FotoID;
            Marca = obj.Marca;
            Modelo = obj.Modelo;
            Matricula = obj.Matricula;
            Permissoes = Regras.Veiculo.Permissoes(ControladorSite.Utilizador, obj);
        }


        public long? ID { get; set; }

        public long? CondominioID { get; set; }

        [DisplayLocalizado(typeof(Resources.Veiculo), "Morador")]
        [RequiredLocalizado(typeof(Resources.Veiculo), "Morador")]
        public long? MoradorID { get; set; }

        public string MoradorNome { get; set; }

        public long? FotoID { get; set; }

        [DisplayLocalizado(typeof(Resources.Veiculo), "Marca")]
        [RequiredLocalizado(typeof(Resources.Veiculo), "Marca")]
        [MaxStringLocalizado(40, typeof(Resources.Veiculo), "Marca")]
        public string Marca { get; set; }

        [DisplayLocalizado(typeof(Resources.Veiculo), "Modelo")]
        [RequiredLocalizado(typeof(Resources.Veiculo), "Modelo")]
        [MaxStringLocalizado(40, typeof(Resources.Veiculo), "Modelo")]
        public string Modelo { get; set; }

        [DisplayLocalizado(typeof(Resources.Veiculo), "Matricula")]
        [RequiredLocalizado(typeof(Resources.Veiculo), "Matricula")]
        [MaxStringLocalizado(10, typeof(Resources.Veiculo), "Matricula")]
        public string Matricula { get; set; }

        public List<Regras.Enum.Permissao> Permissoes { get; set; }


        public Regras.BD.Veiculo ToBDModel()
        {
            Regras.BD.Veiculo obj = new Regras.BD.Veiculo();
            obj.VeiculoID = (ID != null ? Convert.ToInt64(ID) : obj.VeiculoID);
            obj.CondominioID = (CondominioID != null ? CondominioID.Value : 0);
            obj.MoradorID = (MoradorID != null ? MoradorID.Value : 0);
            obj.FotoID = FotoID;
            obj.Marca = Marca;
            obj.Modelo = Modelo;
            obj.Matricula = Matricula;
            return obj;

        }

    }

}