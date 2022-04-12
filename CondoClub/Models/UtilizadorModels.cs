using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CondoClub.Web.Models {


    public class Login {

        public Login()
        {
            RememberMe = true;
        }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Email")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "Email")]
        public string Email { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Password")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Recordar")]
        public bool RememberMe { get; set; }
    }


    public class LoginAdministrativo {

        public LoginAdministrativo() {
            try {
                ListaUtilizadores = new Controllers.UtilizadorController().ConstroiDropDownAdministracao(null);
            }
            catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, null);
            }
        }

        [RequiredLocalizado(typeof(Resources.Utilizador), "SeleccioneUtilizador")]
        [DisplayLocalizado(typeof(Resources.Utilizador), "SeleccioneUtilizador")]
        public long? UtilizadorSeleccionado { get; set; }

        public IEnumerable<SelectListItem> ListaUtilizadores { get; set; }

    }


    public class Utilizador 
    {
        public Utilizador()
        {
            Activo = true;
        }

        public Utilizador(Regras.BD.Utilizador obj) 
        {
            ID = obj.UtilizadorID;
            PerfilID = obj.PerfilUtilizadorID;
            PerfilDesignacao = obj.PerfilUtilizador.Designacao;
            Email = obj.Email;
            Nome = obj.Nome;
            Fraccao = obj.Fraccao;
            AvatarID = obj.AvatarID;
            Activo = obj.Activo;
            EmpresaID = obj.EmpresaID;
            CondominioID = obj.CondominioID;
            FornecedorID = obj.FornecedorID;

            if (EmpresaID.HasValue)
                NomeInstituicao = obj.Empresa.Nome;
            else if (CondominioID.HasValue)
                NomeInstituicao = obj.Condominio.Nome;
            else if (FornecedorID.HasValue)
                NomeInstituicao = obj.Fornecedor.Nome;

            Permissoes = Regras.Morador.Permissoes(ControladorSite.Utilizador, obj);
        }


        public long? ID { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "PerfilUtilizador")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "PerfilUtilizador")]
        public int PerfilID { get; set; }

        public string PerfilDesignacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Email")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "Email")]
        [FormatoEmailLocalizado(typeof(Resources.Utilizador), "Email")]
        [MaxStringLocalizado(200, typeof(Resources.Utilizador), "Email")]
        [DataType(DataType.Text)]
        public string Email { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Nome")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "Nome")]
        [MaxStringLocalizado(200, typeof(Resources.Utilizador), "Nome")]
        public string Nome { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Fraccao")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "Fraccao")]
        [MaxStringLocalizado(50, typeof(Resources.Utilizador), "Fraccao")]
        public string Fraccao { get; set; }

        public long? AvatarID { get; set; }
        
        [DisplayLocalizado(typeof(Resources.Utilizador), "Activo")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "Activo")]
        public bool Activo { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Empresa")]
        public long? EmpresaID { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Condominio")]
        public long? CondominioID { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Fornecedor")]
        public long? FornecedorID { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "NomeInstituicao")]
        public string NomeInstituicao { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Password")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "Password")]
        [MaxStringLocalizado(50, typeof(Resources.Utilizador), "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "ConfirmarPassword")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "ConfirmarPassword")]
        [MaxStringLocalizado(50, typeof(Resources.Utilizador), "ConfirmarPassword")]
        [DataType(DataType.Password)]
        public string ConfirmarPassword { get; set; }

        public List<Regras.Enum.Permissao> Permissoes { get; set; }


        public virtual Regras.BD.Utilizador ToBDModel()
        {
            Regras.BD.Utilizador obj = new Regras.BD.Utilizador();
            obj.UtilizadorID = (ID != null ? Convert.ToInt64(ID) : obj.UtilizadorID);
            obj.PerfilUtilizadorID = Convert.ToInt32(PerfilID);
            obj.Email = Email;
            obj.Nome = Nome;
            obj.AvatarID = AvatarID;
            obj.Activo = Activo;
            obj.EmpresaID = EmpresaID;
            obj.CondominioID = CondominioID;
            obj.Fraccao = Fraccao;
            obj.FornecedorID = FornecedorID;
            obj.Password = Password;

            return obj;
        }
    }


    public class UtilizadorDropDown
    {
        public UtilizadorDropDown(Regras.BD.Utilizador obj)
        {
            ID = obj.UtilizadorID;
            PerfilID = obj.PerfilUtilizadorID;
            Nome = obj.Nome;
            Fraccao = obj.Fraccao;
            Descricao = Nome + (!string.IsNullOrEmpty(Fraccao) ? " (" + Fraccao + ")" : string.Empty);

        }


        public long? ID { get; set; }

        public int PerfilID { get; set; }

        public string Nome { get; set; }

        public string Fraccao { get; set; }

        public string Descricao { get; set; }


        public Regras.BD.Utilizador ToBDModel()
        {
            Regras.BD.Utilizador obj = new Regras.BD.Utilizador();
            obj.UtilizadorID = (ID != null ? Convert.ToInt64(ID) : obj.UtilizadorID);
            obj.PerfilUtilizadorID = Convert.ToInt32(PerfilID);
            obj.Nome = Nome;
            obj.Fraccao = Fraccao;

            return obj;
        }
    }


    public class UtilizadorPassword
    {
        public UtilizadorPassword() 
        {
            Permissoes = Regras.Utilizador.Permissoes(ControladorSite.Utilizador);
        }

        public UtilizadorPassword(Regras.BD.Utilizador obj) 
        {
            ID = obj.UtilizadorID;
            Password = obj.Password;
            Permissoes = Regras.Utilizador.Permissoes(ControladorSite.Utilizador, obj);
        }


        public long? ID { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "PasswordActual")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "PasswordActual")]
        [MaxStringLocalizado(50, typeof(Resources.Utilizador), "PasswordActual")]
        [DataType(DataType.Password)]
        public string PasswordActual { get; set; }


        [DisplayLocalizado(typeof(Resources.Utilizador), "Password")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "Password")]
        [MaxStringLocalizado(50, typeof(Resources.Utilizador), "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "ConfirmarPassword")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "ConfirmarPassword")]
        [MaxStringLocalizado(50, typeof(Resources.Utilizador), "ConfirmarPassword")]
        [DataType(DataType.Password)]
        public string ConfirmarPassword { get; set; }

        public List<Regras.Enum.Permissao> Permissoes { get; set; }


        public Regras.BD.Utilizador ToBDModel()
        {
            return new Regras.BD.Utilizador(){
                UtilizadorID = (ID != null ? Convert.ToInt64(ID) : 0),
                Password = Password
            };
        }
    }


    public class MosaicoMorador {

        public MosaicoMorador(Regras.BD.Utilizador obj) {
            AvatarID = obj.AvatarID;
        }

        public long? AvatarID { get; set; }    

    }

    public class UtilizadorFiltro {

        public UtilizadorFiltro() {
        }
        
        public string TermoPesquisa { get; set; }
        public Regras.UtilizadorEstado? EstadoPesquisa { get; set; }
        public Regras.Enum.Perfil? PerfilPesquisa { get; set; }
        public int? PerfilPesquisaInt { 
            get { return (int?)PerfilPesquisa;} 
            set{
                if (value.HasValue) {
                    PerfilPesquisa = (Regras.Enum.Perfil)value;
                }    
            }
        }
        public int? CondominioPesquisa { get; set; }
        public int? EmpresaPesquisa { get; set; }
        public int? FornecedorPesquisa { get; set; }

    }

}
