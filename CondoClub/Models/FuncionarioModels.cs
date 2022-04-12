using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CondoClub.Web.Models
{

    public class Funcionario
    {
        public Funcionario() 
        {
            Masculino = true;
            Permissoes = Regras.Veiculo.Permissoes(ControladorSite.Utilizador);
        }

        public Funcionario(Regras.BD.Funcionario obj)
        {
            ID = obj.FuncionarioID;
            CondominioID = obj.CondominioID;
            Nome = obj.Nome;
            FotoID = obj.FotoID;
            DataNascimento = obj.DataNascimento;
            Masculino = obj.Masculino;
            Identificacao = obj.Identificacao;
            Funcao = obj.Funcao;
            Horario = obj.Horario;
            Telefone = obj.Telefone;
            Email = obj.Email;
            Permissoes = Regras.Funcionario.Permissoes(ControladorSite.Utilizador, obj);
        }


        public long? ID { get; set; }

        public long? CondominioID { get; set; }

        [DisplayLocalizado(typeof(Resources.Funcionario), "Nome")]
        [RequiredLocalizado(typeof(Resources.Funcionario), "Nome")]
        [MaxStringLocalizado(200, typeof(Resources.Funcionario), "Nome")]
        public string Nome { get; set; }

        public long? FotoID { get; set; }

        [DisplayLocalizado(typeof(Resources.Funcionario), "DataNascimento")]
        [RequiredLocalizado(typeof(Resources.Funcionario), "DataNascimento")]
        [FormatoDataLocalizado(typeof(Resources.Funcionario), "DataNascimento")]
        public DateTime? DataNascimento { get; set; }

        [DisplayLocalizado(typeof(Resources.Funcionario), "Sexo")]
        [RequiredLocalizado(typeof(Resources.Funcionario), "Sexo")]
        public bool Masculino { get; set; }

        [DisplayLocalizado(typeof(Resources.Funcionario), "Identificacao")]
        [RequiredLocalizado(typeof(Resources.Funcionario), "Identificacao")]
        [MaxStringLocalizado(200, typeof(Resources.Funcionario), "Identificacao")]
        public string Identificacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Funcionario), "Funcao")]
        [RequiredLocalizado(typeof(Resources.Funcionario), "Funcao")]
        [MaxStringLocalizado(200, typeof(Resources.Funcionario), "Funcao")]
        public string Funcao { get; set; }

        [DisplayLocalizado(typeof(Resources.Funcionario), "Horario")]
        [RequiredLocalizado(typeof(Resources.Funcionario), "Horario")]
        [MaxStringLocalizado(200, typeof(Resources.Funcionario), "Horario")]
        public string Horario { get; set; }

        [DisplayLocalizado(typeof(Resources.Funcionario), "Telefone")]
        [RequiredLocalizado(typeof(Resources.Funcionario), "Telefone")]
        [MaxStringLocalizado(50, typeof(Resources.Funcionario), "Telefone")]
        public string Telefone { get; set; }

        [DisplayLocalizado(typeof(Resources.Funcionario), "Email")]
        [MaxStringLocalizado(200, typeof(Resources.Funcionario), "Email")]
        [FormatoEmailLocalizado(typeof(Resources.Funcionario), "Email")]
        public string Email { get; set; }

        public List<Regras.Enum.Permissao> Permissoes { get; set; }


        public Regras.BD.Funcionario ToBDModel()
        {
            Regras.BD.Funcionario obj = new Regras.BD.Funcionario();
            obj.FuncionarioID = (ID != null ? Convert.ToInt64(ID) : obj.FuncionarioID);
            obj.CondominioID = (CondominioID != null ? CondominioID.Value : 0);
            obj.Nome = Nome;
            obj.FotoID = FotoID;
            obj.DataNascimento = DataNascimento.HasValue ? DataNascimento.Value : DateTime.Now;
            obj.Masculino = Masculino;
            obj.Identificacao = Identificacao;
            obj.Funcao = Funcao;
            obj.Horario = Horario;
            obj.Telefone = Telefone;
            obj.Email = Email;
            return obj;

        }
    }

}