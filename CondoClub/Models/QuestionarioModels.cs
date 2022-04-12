using System;
using System.Collections.Generic;
using System.Web;

namespace CondoClub.Web.Models {

    public class Questionario {

        public Questionario() {
            Inicio = DateTime.Now.Date;
            Fim = DateTime.Now.Date.AddDays(7);
        }

        public Questionario(Regras.BD.Questionario obj) {
            ID = obj.QuestionarioID;
            CondominioID = obj.CondominioID;
            Questao = obj.Questao;
            Inicio = obj.Inicio;
            Fim = obj.Fim;
            Opcao1 = obj.Opcao1;
            Opcao2 = obj.Opcao2;
            Opcao3 = obj.Opcao3;
            Opcao4 = obj.Opcao4;
            Opcao5 = obj.Opcao5;
            Opcao6 = obj.Opcao6;
            Opcao7 = obj.Opcao7;
            Opcao8 = obj.Opcao8;

            Permissoes = Regras.Questionario.Permissoes(ControladorSite.Utilizador, obj);

            TotalRespostasPorOpcao = new int[9];
            TotalRespostasPorOpcaoPercentagem = new int[9];
            Respostas = new List<QuestionarioResposta>();

            if (obj.QuestionarioResposta.IsLoaded) {
                foreach (var item in obj.QuestionarioResposta) {
                    Respostas.Add(new QuestionarioResposta(item));
                    
                    if (item.MoradorID == ControladorSite.Utilizador.ID)
                        JaRespondi = true;
                    
                    TotalRespostas++;
                    TotalRespostasPorOpcao[item.OpcaoSeleccionada]++;
                }

                if (TotalRespostas > 0) {
                    for (int i = 1; i < TotalRespostasPorOpcao.Length; i++) {
                        TotalRespostasPorOpcaoPercentagem[i] = (int)((float)TotalRespostasPorOpcao[i] / (float)TotalRespostas * 100);
                    }
                }
            }
        }

        public long? ID { get; set; }

        public long CondominioID { get; set; }

        [DisplayLocalizado(typeof(Resources.Questionario), "Questao")]
        [RequiredLocalizado(typeof(Resources.Questionario), "Questao")]
        [MaxStringLocalizado(200, typeof(Resources.Questionario), "Questao")]
        public string Questao { get; set; }

        [DisplayLocalizado(typeof(Resources.Questionario), "Inicio")]
        [RequiredLocalizado(typeof(Resources.Questionario), "Inicio")]
        [FormatoDataLocalizado(typeof(Resources.Questionario), "Inicio")]
        public DateTime Inicio { get; set; }

        [DisplayLocalizado(typeof(Resources.Questionario), "Fim")]
        [RequiredLocalizado(typeof(Resources.Questionario), "Fim")]
        [FormatoDataLocalizado(typeof(Resources.Questionario), "Fim")]
        public DateTime Fim { get; set; }

        [DisplayLocalizado(typeof(Resources.Questionario), "Opcao1")]
        [RequiredLocalizado(typeof(Resources.Questionario), "Opcao1")]
        [MaxStringLocalizado(200, typeof(Resources.Questionario), "Opcao1")]
        public string Opcao1 { get; set; }

        [DisplayLocalizado(typeof(Resources.Questionario), "Opcao2")]
        [RequiredLocalizado(typeof(Resources.Questionario), "Opcao2")]
        [MaxStringLocalizado(200, typeof(Resources.Questionario), "Opcao2")]
        public string Opcao2 { get; set; }

        [DisplayLocalizado(typeof(Resources.Questionario), "Opcao3")]
        [MaxStringLocalizado(200, typeof(Resources.Questionario), "Opcao3")]
        public string Opcao3 { get; set; }

        [DisplayLocalizado(typeof(Resources.Questionario), "Opcao4")]
        [MaxStringLocalizado(200, typeof(Resources.Questionario), "Opcao4")]
        public string Opcao4 { get; set; }

        [DisplayLocalizado(typeof(Resources.Questionario), "Opcao5")]
        [MaxStringLocalizado(200, typeof(Resources.Questionario), "Opcao5")]
        public string Opcao5 { get; set; }

        [DisplayLocalizado(typeof(Resources.Questionario), "Opcao6")]
        [MaxStringLocalizado(200, typeof(Resources.Questionario), "Opcao6")]
        public string Opcao6 { get; set; }

        [DisplayLocalizado(typeof(Resources.Questionario), "Opcao7")]
        [MaxStringLocalizado(200, typeof(Resources.Questionario), "Opcao7")]
        public string Opcao7 { get; set; }

        [DisplayLocalizado(typeof(Resources.Questionario), "Opcao8")]
        [MaxStringLocalizado(200, typeof(Resources.Questionario), "Opcao8")]
        public string Opcao8 { get; set; }

        public long AutorID { get; set; }

        public bool JaRespondi { get; set; }

        public int TotalRespostas { get; set; }

        public int[] TotalRespostasPorOpcao { get; set; }

        public int[] TotalRespostasPorOpcaoPercentagem { get; set; }

        public List<QuestionarioResposta> Respostas { get; set; }

        public List<Regras.Enum.Permissao> Permissoes { get; set; }

        public Regras.BD.Questionario ToBDModel() {
            Regras.BD.Questionario obj = new Regras.BD.Questionario();
            obj.QuestionarioID = (ID != null ? Convert.ToInt64(ID) : obj.QuestionarioID);
            obj.CondominioID = ControladorSite.Utilizador.CondominioID.Value;
            obj.Questao = Questao;
            obj.Inicio = Inicio;
            obj.Fim = Fim;
            obj.Opcao1 = Opcao1;
            obj.Opcao2 = Opcao2;
            obj.Opcao3 = !String.IsNullOrEmpty(Opcao3) ? Opcao3.Trim() : null;
            obj.Opcao4 = !String.IsNullOrEmpty(Opcao4) ? Opcao4.Trim() : null;
            obj.Opcao5 = !String.IsNullOrEmpty(Opcao5) ? Opcao5.Trim() : null;
            obj.Opcao6 = !String.IsNullOrEmpty(Opcao6) ? Opcao6.Trim() : null;
            obj.Opcao7 = !String.IsNullOrEmpty(Opcao7) ? Opcao7.Trim() : null;
            obj.Opcao8 = !String.IsNullOrEmpty(Opcao8) ? Opcao8.Trim() : null;
            obj.AutorID = ControladorSite.Utilizador.ID;
            return obj;
        }

    }


    public class QuestionarioResposta { 
    
        public QuestionarioResposta() { }

        public QuestionarioResposta(Regras.BD.QuestionarioResposta obj) {
            ID = obj.QuestionarioRespostaID;
            QuestionarioID = obj.QuestionarioID;
            MoradorID = obj.MoradorID;
            MoradorNome = obj.Utilizador.Nome;
            MoradorAvatarID = obj.Utilizador.AvatarID;
            OpcaoSeleccionada = obj.OpcaoSeleccionada;
            OutraOpcao = obj.OutraOpcao;
            DataHora = obj.DataHora;
        }

        public long? ID { get; set; }

        public long QuestionarioID { get; set; }

        public long MoradorID { get; set; }
        
        public string MoradorNome { get; set; }

        public long? MoradorAvatarID { get; set; }

        [DisplayLocalizado(typeof(Resources.Questionario), "OpcaoSeleccionada")]
        [RequiredLocalizado(typeof(Resources.Questionario), "OpcaoSeleccionada")]
        public short OpcaoSeleccionada { get; set; }

        [DisplayLocalizado(typeof(Resources.Questionario), "OutraOpcao")]
        [MaxStringLocalizado(800, typeof(Resources.Questionario), "OutraOpcao")]
        public string OutraOpcao { get; set; }

        public DateTime DataHora { get; set; }

        public Regras.BD.QuestionarioResposta ToBDModel() {
            Regras.BD.QuestionarioResposta obj = new Regras.BD.QuestionarioResposta();
            obj.QuestionarioID = QuestionarioID;
            obj.MoradorID = ControladorSite.Utilizador.ID;
            obj.OpcaoSeleccionada = OpcaoSeleccionada;
            obj.OutraOpcao = !String.IsNullOrEmpty(OutraOpcao) ? OutraOpcao.Trim() : null;
            obj.DataHora = DateTime.Now;
            return obj;
        }
    }

}
