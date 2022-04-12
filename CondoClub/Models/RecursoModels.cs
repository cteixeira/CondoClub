using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CondoClub.Web.Models {

    public class CalendarioIndexModel {
        public bool ApresentaLista { get; set; }
        public CalendarioListaMes ListaMes { get; set; }
        public CalendarioDetalheDia DetalheDiaMes { get; set; }
    }

    public class CalendarioListaMes {
        public int Mes { get; set; }
        public int Ano { get; set; }
        public List<CalendarioDia> Dias { get; set; }
    }

    public class CalendarioDetalheDia {
        public DateTime Data { get; set; }
        public List<CalendarioSlot> Slots { get; set; }
    }

    public class CalendarioDia {

        public CalendarioDia(Regras.CalendarioDia cDia) {
            Data = cDia.Data;
            if (cDia.Disponibilidade != Regras.RecursoDisponibilidadeDia.Nao_Disponivel &&
                cDia.Slots.Any(s => s.Reserva != null && s.Reserva.MoradorID == ControladorSite.Utilizador.ID)) {
                CssClassDisponibilidade = "blue";
            } else {
                switch (cDia.Disponibilidade) {
                    case CondoClub.Regras.RecursoDisponibilidadeDia.Disponivel:
                        CssClassDisponibilidade = "green";
                        break;
                    case CondoClub.Regras.RecursoDisponibilidadeDia.Quase_Completo:
                        CssClassDisponibilidade = "yellow";
                        break;
                    case CondoClub.Regras.RecursoDisponibilidadeDia.Completo:
                        CssClassDisponibilidade = "red";
                        break;
                    default:
                        CssClassDisponibilidade = "gray";
                        break;
                }
            }
            NumeroSlots = cDia.NumeroSlots;
            NumeroSlotsLivres = cDia.NumeroSlotsLivres;
        }

        public DateTime Data { get; set; }
        public string CssClassDisponibilidade { get; set; }
        public int NumeroSlots { get; set; }
        public int NumeroSlotsLivres { get; set; }
    }

    public class CalendarioSlot {

        public CalendarioSlot(Regras.CalendarioSlot cSlot) {
            if (cSlot.Reserva != null) {
                ReservaID = cSlot.Reserva.RecursoReservaID;
            }
            Inicio = cSlot.Inicio;
            Horario = String.Format("{0} - {1}", cSlot.Inicio.ToString("HH:mm"), cSlot.Fim.ToString("HH:mm"));

            switch (cSlot.Disponibilidade) {
                case CondoClub.Regras.RecursoDisponibilidadeSlot.Nao_Disponivel:
                    CssClassDisponibilidade = "gray";
                    DesignacaoSlot = Resources.Recurso.NaoDisponivel;
                    break;
                case CondoClub.Regras.RecursoDisponibilidadeSlot.Disponivel:
                    CssClassDisponibilidade = "green";
                    DesignacaoSlot = Resources.Recurso.Disponivel;
                    break;
                case CondoClub.Regras.RecursoDisponibilidadeSlot.PendenteAprovacao:
                    CssClassDisponibilidade = cSlot.Reserva != null && cSlot.Reserva.MoradorID == ControladorSite.Utilizador.ID ? "blue" : "red";
                    if (cSlot.Permissoes.Contains(Regras.RecursoPermissao.VisualizarSlotReservadoPor)) {
                        DesignacaoSlot = String.Concat(Resources.Recurso.PendenteAprovacao, " - ", cSlot.NomeReserva);
                    } else {
                        DesignacaoSlot = Resources.Recurso.PendenteAprovacao;
                    }
                    break;
                case CondoClub.Regras.RecursoDisponibilidadeSlot.Reservado:
                    CssClassDisponibilidade = cSlot.Reserva != null && cSlot.Reserva.MoradorID == ControladorSite.Utilizador.ID ? "blue" : "red";
                    if (cSlot.Permissoes.Contains(Regras.RecursoPermissao.VisualizarSlotReservadoPor)) {
                        DesignacaoSlot = cSlot.NomeReserva;
                    } else {
                        DesignacaoSlot = Resources.Recurso.Reservado;
                    }
                    break;
                default:
                    CssClassDisponibilidade = "gray";
                    DesignacaoSlot = Resources.Recurso.NaoDisponivel;
                    break;
            }
            Permissoes = cSlot.Permissoes;
        }

        public long? ReservaID { get; set; }
        public DateTime Inicio { get; set; }
        public string Horario { get; set; }
        public string DesignacaoSlot { get; set; }
        public string CssClassDisponibilidade { get; set; }
        public List<Regras.RecursoPermissao> Permissoes { get; set; }
    }

    public class Recurso {

        public Recurso() {
            Horario = new List<RecursoHorario>();
        }

        public Recurso(Regras.BD.Recurso obj) {
            ID = obj.RecursoID;
            Designacao = obj.Designacao;
            Activo = obj.Activo;
            RequerAprovacao = obj.RequerAprovacao;
            DiasMinimosAprovacao = obj.DiasMinAprovacao;
            MaxSlotsReserva = obj.MaxSlotsReserva;
            //IntervaloReserva = obj.IntervaloReserva;
            Horario = new List<RecursoHorario>();
            if (obj.RecursoHorario.IsLoaded) {
                foreach (Regras.BD.RecursoHorario h in obj.RecursoHorario.OrderBy(o => o.DiaSemana).ThenBy(o => o.Inicio)) {
                    Horario.Add(new RecursoHorario(h));
                }
            }

            Permissoes = Regras.Recurso.Permissoes(ControladorSite.Utilizador, obj);

        }

        public long? ID { get; set; }

        [DisplayLocalizado(typeof(Resources.Recurso), "Designacao")]
        [RequiredLocalizado(typeof(Resources.Recurso), "Designacao")]
        public string Designacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Recurso), "Activo")]
        [RequiredLocalizado(typeof(Resources.Recurso), "Activo")]
        public bool Activo { get; set; }

        [DisplayLocalizado(typeof(Resources.Recurso), "RequerAprovacao")]
        [RequiredLocalizado(typeof(Resources.Recurso), "RequerAprovacao")]
        public bool RequerAprovacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Recurso), "DiasMinimosAprovacao")]
        [FormatoNumeroInteiroLocalizado(typeof(Resources.Recurso), "DiasMinimosAprovacao")]
        [RangeLocalizado(0, Int32.MaxValue, typeof(Resources.Recurso), "MaxSlotsReserva")]
        public short? DiasMinimosAprovacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Recurso), "MaxSlotsReserva")]
        [RequiredLocalizado(typeof(Resources.Recurso), "MaxSlotsReserva")]
        [FormatoNumeroInteiroLocalizado(typeof(Resources.Recurso), "MaxSlotsReserva")]
        [RangeLocalizado(1, Int32.MaxValue, typeof(Resources.Recurso), "MaxSlotsReserva")]
        public short? MaxSlotsReserva { get; set; }

        //[DisplayLocalizado(typeof(Resources.Recurso), "IntervaloReserva")]
        //[RequiredLocalizado(typeof(Resources.Recurso), "IntervaloReserva")]
        //[RangeLocalizado(1, Int32.MaxValue, typeof(Resources.Recurso), "IntervaloReserva")]
        //public short? IntervaloReserva { get; set; }

        public List<RecursoHorario> Horario { get; set; }

        public List<Regras.Enum.Permissao> Permissoes { get; set; }

        public Regras.BD.Recurso ToBDModel() {
            Regras.BD.Recurso bdModel = new Regras.BD.Recurso();
            if (ID.HasValue) {
                bdModel.RecursoID = ID.Value;
            }
            bdModel.CondominioID = ControladorSite.Utilizador.CondominioID.Value;
            bdModel.Designacao = Designacao;
            bdModel.Activo = Activo;
            bdModel.RequerAprovacao = RequerAprovacao;
            bdModel.DiasMinAprovacao = DiasMinimosAprovacao;
            bdModel.MaxSlotsReserva = MaxSlotsReserva.Value;
            //bdModel.IntervaloReserva = IntervaloReserva.Value;
            bdModel.IntervaloReserva = 24;

            return bdModel;

        }

    }

    public class RecursoHorario {

        public RecursoHorario() {

        }

        public RecursoHorario(Regras.BD.RecursoHorario obj) {
            RecursoHorarioID = obj.RecursoHorarioID;
            RecursoID = obj.RecursoID;
            DiaSemana = obj.DiaSemana;
            Inicio = obj.Inicio;
            NumeroSlots = obj.NumeroSlots;
            DuracaoSlot = obj.DuracaoSlot;
        }

        public long? RecursoHorarioID { get; set; }
        public long RecursoID { get; set; }

        [DisplayLocalizado(typeof(Resources.Recurso), "DiaSemana")]
        [RequiredLocalizado(typeof(Resources.Recurso), "DiaSemana")]
        public short? DiaSemana { get; set; }
        public String DiaSemanaDesignacao {
            get { return Resources.Calendario.ResourceManager.GetString(String.Concat("DiaSemana", DiaSemana)); }
        }

        [DisplayLocalizado(typeof(Resources.Recurso), "HoraInicio")]
        [RequiredLocalizado(typeof(Resources.Recurso), "HoraInicio")]
        [FormatoHoraLocalizado(typeof(Resources.Recurso), "HoraInicio")]
        public DateTime? Inicio { get; set; }

        [DisplayLocalizado(typeof(Resources.Recurso), "NumeroSlots")]
        [RequiredLocalizado(typeof(Resources.Recurso), "NumeroSlots")]
        [FormatoNumeroInteiroLocalizado(typeof(Resources.Recurso), "NumeroSlots")]
        public short? NumeroSlots { get; set; }

        [DisplayLocalizado(typeof(Resources.Recurso), "DuracaoSlot")]
        [RequiredLocalizado(typeof(Resources.Recurso), "DuracaoSlot")]
        [RangeLocalizado(1, 1440, typeof(Resources.Recurso), "DuracaoSlot")]
        public short? DuracaoSlot { get; set; }

        public Regras.BD.RecursoHorario ToBDModel() {
            Regras.BD.RecursoHorario bdModel = new Regras.BD.RecursoHorario();
            if (RecursoHorarioID.HasValue) {
                bdModel.RecursoHorarioID = RecursoHorarioID.Value;
            }
            bdModel.RecursoID = RecursoID;
            bdModel.DiaSemana = DiaSemana.Value;
            bdModel.Inicio = Inicio.Value;
            bdModel.NumeroSlots = NumeroSlots.Value;
            bdModel.DuracaoSlot = DuracaoSlot.Value;

            return bdModel;

        }

    }

    public class ListaReservaFiltro {

        public ListaReservaFiltro() {
            DataInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DataFim = DateTime.Now.AddDays(15);
            Estado = Regras.Enum.EstadoRecursoReserva.Reservado;
        }

        public long? CondominioID { get; set; }

        [DisplayLocalizado(typeof(Resources.Recurso), "Recursos")]
        public long? RecursoID { get; set; }

        [DisplayLocalizado(typeof(Resources.Recurso), "Morador")]
        public long? MoradorID { get; set; }

        [DisplayLocalizado(typeof(Resources.Recurso), "DataInicio")]
        public DateTime? DataInicio { get; set; }

        [DisplayLocalizado(typeof(Resources.Recurso), "DataFim")]
        public DateTime? DataFim { get; set; }

        [DisplayLocalizado(typeof(Resources.Recurso), "EstadoReserva")]
        public Regras.Enum.EstadoRecursoReserva? Estado { get; set; }

    }

    public class RecursoReserva {

        public long? ID { get; set; }
        public string RecursoDesignacao { get; set; }
        public string UtilizadorDesignacao { get; set; }
        public DateTime DataInicio { get; set; }


        public RecursoReserva(Regras.BD.RecursoReserva obj) {
            ID = obj.RecursoReservaID;
            DataInicio = obj.DataHoraInicio;
            if (obj.RecursoReference.IsLoaded) {
                RecursoDesignacao = obj.Recurso.Designacao;
            }
            if (obj.UtilizadorReference.IsLoaded) {
                UtilizadorDesignacao = obj.Utilizador.Nome;
            }
        }

    }

}