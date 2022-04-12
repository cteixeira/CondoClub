using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;

namespace CondoClub.Web {

    public class ControladorSite {

        public static Regras.Enum.Pais Pais {
            get {
                return Regras.Enum.Pais.Brasil;
                //switch (System.Threading.Thread.CurrentThread.CurrentUICulture.Name) {
                //    case "pt-BR":
                //        return Regras.Enum.Pais.Brasil;
                //    case "pt-PT":
                //        return Regras.Enum.Pais.Portugal;
                //    default:
                //        return Regras.Enum.Pais.Brasil; ;
                //}
            }
        }

        public static Regras.UtilizadorAutenticado Utilizador {
            get {
                if (HttpContext.Current.Items["Utilizador"] != null)
                    return (Regras.UtilizadorAutenticado)HttpContext.Current.Items["Utilizador"];
                return null;
            }
            set {
                HttpContext.Current.Items["Utilizador"] = value;
            }
        }

    }
}