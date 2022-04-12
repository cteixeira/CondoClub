using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CondoClub.Web.Models {

    public class MensagemGrupo{

        public MensagemGrupo(Regras.MensagemGrupo obj) {

            Id = obj.MensagemID;
            UtilizadorAvatarID = obj.AvatarID;
            TextoCabecalho = obj.TextoCabecalho;
            //remover o utilizador corrente do texto do cabeçalho
            FiltrarTextoCabecalho();
            Data = obj.DataHora;
            UltimaRespostaRemetente = obj.UtlimaRespostaRemetente;
            UltimaRespostaTexto = obj.UltimaRespostaTexto;
            NumeroMensagensNovas = obj.NrMensagensNovas;

        }

        public long Id { get; set; }
        public long? UtilizadorAvatarID { get; set; }
        public string TextoCabecalho { get; set; }
        public DateTime Data { get; set; }
        public string UltimaRespostaRemetente { get; set; }
        public string UltimaRespostaTexto { get; set; }
        public int NumeroMensagensNovas { get; set; }

        private void FiltrarTextoCabecalho() {
            string[] textoCabecalhoSplited = TextoCabecalho.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            TextoCabecalho = String.Join(", ", textoCabecalhoSplited.Where(s => !s.ToLower().Contains(ControladorSite.Utilizador.Nome.ToLower())));
        }

    }

    public class Mensagem {

        public Mensagem() { }

        public Mensagem(Regras.BD.Mensagem obj) {
            MensagemID = obj.MensagemID;
            RespostaID = obj.RespostaID;
            RemetenteID = obj.Utilizador.UtilizadorID;
            RemetenteNome = Regras.Mensagem.FormataNomeUtilizadorMensagem(obj.Utilizador);
            RemetenteAvatarID = obj.Utilizador.AvatarID;
            DataEnvio = obj.DataHora;
            //Texto = Util.ConvertUrlsToLinks(obj.Texto).Replace(Environment.NewLine, "<br/>");
            Texto = obj.Texto.Replace(Environment.NewLine, "<br/>");
            Ficheiros = new List<Ficheiro>();
            if (obj.MensagemFicheiro != null) {
                foreach (Regras.BD.MensagemFicheiro mf in obj.MensagemFicheiro) {
                    Ficheiros.Add(new Ficheiro(mf.Ficheiro));
                }
            }
            Regras.BD.MensagemDestinatario msgDestinatario = obj.MensagemDestinatario.Where(md => md.DestinatarioID == ControladorSite.Utilizador.ID).FirstOrDefault();
            Visto = msgDestinatario != null ? msgDestinatario.Visto : true;
        }

        public long? MensagemID { get; set; }
        public long? RespostaID { get; set; }
        public long? RemetenteID { get; set; }
        public string RemetenteNome { get; set; }
        public long? RemetenteAvatarID { get; set; }
        public DateTime DataEnvio { get; set; }
        [RequiredLocalizado(typeof(Resources.Mensagem), "Texto")]
        public string Texto { get; set; }
        public List<Ficheiro> Ficheiros { get; set; }
        public bool Visto { get; set; }

        public Regras.BD.Mensagem ToBDModel() {

            Regras.BD.Mensagem obj = new Regras.BD.Mensagem();
            obj.RespostaID = RespostaID;
            obj.RemetenteID = ControladorSite.Utilizador.ID;
            obj.Texto = Texto;
            obj.DataHora = DateTime.Now;

            return obj;
        }

    }

    public class MensagemDestinatario {

        public MensagemDestinatario() {}

        public MensagemDestinatario(Regras.MensagemDestinatario obj) {
            ID = obj.ID;
            Designacao = obj.Designacao;
            AvatarID = obj.AvatarID;

            IsMorador = obj.IsMorador;
            IsSindico = obj.IsSindico;
            IsEmpresa = obj.IsEmpresa;
            IsCondominio = obj.IsCondominio;
            IsCondoClub = obj.IsCondoClub;
        }

        public long ID { get; set; }
        public string Designacao { get; set; }
        public long? AvatarID{ get; set; }

        public bool IsMorador { get; set; }
        public bool IsSindico { get; set; }
        public bool IsEmpresa { get; set; }
        public bool IsCondominio { get; set; }
        public bool IsCondoClub { get; set; }

        public Regras.MensagemDestinatario ToBDModel() {

            Regras.MensagemDestinatario obj = new Regras.MensagemDestinatario();
            obj.ID = ID;
            obj.Designacao = Designacao;
            obj.AvatarID = AvatarID;

            obj.IsMorador = IsMorador;
            obj.IsSindico = IsSindico;
            obj.IsEmpresa = IsEmpresa;
            obj.IsCondominio = IsCondominio;
            obj.IsCondoClub = IsCondoClub;

            return obj;
        }

    }

    public class MensagemNova {

        [RequiredLocalizado(typeof(Resources.Mensagem), "Para")]
        public List<MensagemDestinatario> Destinatarios { get; set; }

        [RequiredLocalizado(typeof(Resources.Mensagem), "Texto")]
        public string Texto { get; set; }

        public List<long> Ficheiros { get; set; }

    }

}