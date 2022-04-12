using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Runtime.Caching;
using CondoClub.Regras.Exceptions;

namespace CondoClub.Regras {

    public enum RecursoPermissao {
        VisualizarCalendario,
        ReservarSlot,
        AprovarReserva,
        VisualizarSlotReservadoPor,
        CancelarReserva,
        VisualizarEditarRecursos
    }

    public enum ReservarRecursoResult {
        Ok,
        ErroDiasMinimosAprovacao,
        ErroMaxSlotsReservarIntervaloReserva
    }

    public enum ActualizarRecursoResult {
        Ok,
        AvisoMaxSlotsReservaIntervaloReserva
    }

    public enum InserirRecursoHorarioResult {
        Ok,
        ErroSuperior24Horas,
        ErroColisaoSlots
    }

    public enum ActualizarRecursoHorarioResult {
        Ok,
        ErroSuperior24Horas,
        ErroColisaoSlots,
        AvisoTodasReservasCanceladas
    }

    public enum ApagarRecursoHorarioResult {
        Ok,
        AvisoTodasReservasCanceladas
    }

    public class Recurso {

        private _Base<BD.Recurso> _base = new _Base<BD.Recurso>();

        #region constantes

        private float racioDiaQuaseCompleto = 0.49f;

        #endregion

        #region seleccionar

        public static IEnumerable<BD.Recurso> Lista(UtilizadorAutenticado utilizador) {

            if (utilizador == null) {
                throw new ArgumentNullException("utilzador");
            }

            if (!utilizador.CondominioID.HasValue) {
                throw new ArgumentException("O parametro CondominioID tem de estar preenchido");
            }

            using (BD.Context ctx = new BD.Context()) {
                return ctx.Recurso.Include("RecursoHorario").Where(r => r.CondominioID == utilizador.CondominioID.Value).ToList();
            }

        }

        public static IEnumerable<BD.Recurso> Lista(UtilizadorAutenticado utilizador, bool? activo) {

            if (utilizador == null) {
                throw new ArgumentNullException("utilizador");
            }

            if (!utilizador.CondominioID.HasValue) {
                throw new ArgumentException("O parametro CondominioID tem de estar preenchido");
            }

            using (BD.Context ctx = new BD.Context()) {
                return ctx.Recurso.Where(r => r.CondominioID == utilizador.CondominioID.Value && (!activo.HasValue || r.Activo == activo.Value)).ToList();
            }
        }

        public static BD.RecursoReserva AbrirReserva(long reservaID, UtilizadorAutenticado utilizador) {

            if (utilizador == null) {
                throw new ArgumentNullException("utilzador");
            }

            using (BD.Context ctx = new BD.Context()) {
                BD.RecursoReserva reserva = ctx.RecursoReserva.Where(rr => rr.RecursoReservaID == reservaID).FirstOrDefault();

                //apenas o utilizador da reserva e o sindico do condominio podem abrir a reserva
                if (reserva.Utilizador.UtilizadorID != utilizador.ID && !(utilizador.Perfil == Enum.Perfil.Síndico && utilizador.CondominioID == reserva.Recurso.CondominioID)) {
                    throw new SemPermissao();
                }

                return reserva;
            }
        }

        public BD.Recurso Abrir(long id, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {
                BD.Recurso obj = ctx.Recurso.Include("RecursoHorario").FirstOrDefault(r => r.RecursoID == id);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exceptions.SemPermissao();

                return obj;
            }
        }

        public BD.RecursoHorario AbrirHorario(long id, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {
                BD.RecursoHorario obj = ctx.RecursoHorario.Include("Recurso").FirstOrDefault(rh => rh.RecursoHorarioID == id);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj.Recurso).Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exceptions.SemPermissao();

                return obj;
            }
        }

        public static IEnumerable<BD.RecursoReserva> ListaRecursoReserva(long CondominioID, long? RecursoID, long? MoradorID, DateTime? DataInicio, DateTime? DataFim, 
               Enum.EstadoRecursoReserva? Estado, int skip,int take, UtilizadorAutenticado utilizador) {

            if (utilizador == null) {
                throw new ArgumentNullException("utilzador");
            }

            if (!utilizador.CondominioID.HasValue) {
                throw new ArgumentException("O parametro CondominioID tem de estar preenchido");
            }

            using (BD.Context ctx = new BD.Context()) {
                return ctx.RecursoReserva.
                        Include("Recurso").
                        Include("Utilizador").
                        Where(rr => rr.Recurso.CondominioID == CondominioID &&
                                    (!RecursoID.HasValue || rr.RecursoID == RecursoID) &&
                                    (!MoradorID.HasValue || rr.Utilizador.UtilizadorID == MoradorID.Value) &&
                                    (!DataInicio.HasValue || EntityFunctions.TruncateTime(rr.DataHoraInicio) >= DataInicio.Value) &&
                                    (!DataFim.HasValue || EntityFunctions.TruncateTime(rr.DataHoraInicio) <= DataFim.Value) &&
                                    (!Estado.HasValue || 
                                        (Estado.Value == Enum.EstadoRecursoReserva.Reservado && rr.Aprovado.HasValue && rr.Aprovado.Value) || 
                                        (Estado.Value == Enum.EstadoRecursoReserva.PendenteAprovacao && !rr.Aprovado.HasValue) ||
                                        (Estado.Value == Enum.EstadoRecursoReserva.NaoAprovado && rr.Aprovado.HasValue && ! rr.Aprovado.Value))).

                        OrderBy(rr => rr.DataHoraInicio).
                        Skip(skip).
                        Take(take).ToList();
            }

        }

        #endregion

        #region inserir/actualizar

        public void Inserir(BD.Recurso recurso, UtilizadorAutenticado utilizador) {

            if (!RecurcoObjValido(recurso)) {
                throw new Exceptions.DadosIncorrectos();
            }

            if (!Permissoes(utilizador).Contains(Enum.Permissao.Gravar)) {
                throw new Exceptions.SemPermissao();
            }

            using (BD.Context ctx = new BD.Context()) {

                recurso.CondominioID = utilizador.CondominioID.Value;

                if (ctx.Recurso.Any(r => r.CondominioID == recurso.CondominioID
                        && r.Designacao.Trim() == recurso.Designacao.Trim())) {
                    throw new Exceptions.NomeRepetido();
                }

                if (recurso.RequerAprovacao && !recurso.DiasMinAprovacao.HasValue) {
                    recurso.DiasMinAprovacao = 1;
                }

                //intervalo de reserva é sempre 24 horas
                recurso.IntervaloReserva = 24;

                _base.Inserir(recurso, ctx);
                ctx.SaveChanges();
            }

        }

        public ActualizarRecursoResult Actualizar(BD.Recurso recurso, UtilizadorAutenticado utilizador, bool confirmar) {

            recurso.CondominioID = utilizador.CondominioID.Value;

            Regras.BD.Recurso original = _base.Abrir(recurso.RecursoID);

            if (original == null)
                throw new Exceptions.DadosIncorrectos();

            if (!Permissoes(utilizador, original).Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (!RecurcoObjValido(recurso)) {
                throw new Exceptions.DadosIncorrectos();
            }

            //intervalo de reserva é sempre 24 horas
            recurso.IntervaloReserva = 24;

            using (BD.Context ctx = new BD.Context()) {

                if (ctx.Recurso.Any(r => r.RecursoID != recurso.RecursoID &&
                        r.CondominioID == recurso.CondominioID &&
                        r.Designacao.Trim() == recurso.Designacao.Trim())) {
                    throw new Exceptions.NomeRepetido();
                }

                BD.Recurso recursoOriginal = ctx.Recurso.Where(r => r.RecursoID == recurso.RecursoID).FirstOrDefault();
                if (recursoOriginal == null) {
                    throw new DadosIncorrectos();
                }

                if (!confirmar) {
                    //validar se os dias minimos de aprovação dão para as reservas em falta de aprovação e validar se já existem reservas para o futuro
                    if (((recurso.MaxSlotsReserva < recursoOriginal.MaxSlotsReserva) ||
                            (recurso.IntervaloReserva > recursoOriginal.IntervaloReserva)) &&
                        ctx.RecursoReserva.Any(rr => rr.DataHoraInicio > DateTime.Now)) {
                        //o utilizador alterou o maxslots de reserva ou intervalo de reserva  para um valor maior e existem reservas para o futuro
                        return ActualizarRecursoResult.AvisoMaxSlotsReservaIntervaloReserva;
                    }
                }

                //como carreguei o objecto original da base de dados, para aplicar as alterações temos de utilizar o método ApplyCurrentValues
                ctx.Recurso.ApplyCurrentValues(recurso);
                ctx.SaveChanges();
                return ActualizarRecursoResult.Ok;

            }
        }

        public InserirRecursoHorarioResult InserirRecursoHorario(BD.RecursoHorario recursoHorario, UtilizadorAutenticado utilizador) {

            if (!RecurcoHorarioObjValido(recursoHorario)) {
                throw new Exceptions.DadosIncorrectos();
            }

            if (!Permissoes(utilizador).Contains(Enum.Permissao.Gravar)) {
                throw new Exceptions.SemPermissao();
            }

            using (BD.Context ctx = new BD.Context()) {

                recursoHorario.Inicio = new DateTime(1900, 1, 1) + recursoHorario.Inicio.TimeOfDay;
                DateTime datafim = recursoHorario.Inicio.AddMinutes(recursoHorario.DuracaoSlot * recursoHorario.NumeroSlots);


                //Hora inicio (num slots * duração slot inferior < 24h)
                if (datafim.Subtract(recursoHorario.Inicio).TotalHours > 24
                       || (datafim.Day > recursoHorario.Inicio.Day && datafim.TimeOfDay > new TimeSpan(0, 0, 1))) {
                    return InserirRecursoHorarioResult.ErroSuperior24Horas;
                }

                //verifica se não existem colisões de horário
                IEnumerable<BD.RecursoHorario> horario = SeleccionarHorarioDiaSemana(recursoHorario.RecursoID, (DayOfWeek)recursoHorario.DiaSemana - 1);
                if (horario.Any(h => (recursoHorario.Inicio.TimeOfDay >= h.Inicio.TimeOfDay &&
                                     recursoHorario.Inicio.TimeOfDay < h.Inicio.AddMinutes(h.NumeroSlots * h.DuracaoSlot).TimeOfDay) ||
                                     (datafim.TimeOfDay > h.Inicio.TimeOfDay &&
                                     datafim.TimeOfDay <= h.Inicio.AddMinutes(h.NumeroSlots * h.DuracaoSlot).TimeOfDay))) {
                    return InserirRecursoHorarioResult.ErroColisaoSlots;
                }

                BD.Recurso recurso = ctx.Recurso.Where(r => r.RecursoID == recursoHorario.RecursoID).FirstOrDefault();

                if (recurso == null) {
                    throw new Exceptions.DadosIncorrectos();
                }

                //validar se podemos editar o recurso associado
                if (!Permissoes(utilizador, recurso).Contains(Enum.Permissao.Gravar)) {
                    throw new Exceptions.SemPermissao();
                }

                ctx.RecursoHorario.AddObject(recursoHorario);
                ctx.SaveChanges();
                //limpar cahce de horário
                LimparHorarioDiaSemanaCache(recursoHorario.RecursoID);
                return InserirRecursoHorarioResult.Ok;
            }
        }

        public ActualizarRecursoHorarioResult ActualizarRecursoHorario(BD.RecursoHorario recursoHorario, UtilizadorAutenticado utilizador, bool confirmar) {
            if (!RecurcoHorarioObjValido(recursoHorario)) {
                throw new Exceptions.DadosIncorrectos();
            }

            using (BD.Context ctx = new BD.Context()) {

                //validar se existem reservas no futuro no horário que se pretende editar

                BD.Recurso recurso = ctx.Recurso.Where(r => r.RecursoID == recursoHorario.RecursoID).FirstOrDefault();

                if (recurso == null) {
                    throw new Exceptions.DadosIncorrectos();
                }

                //validar se podemos editar o recurso associado
                if (!Permissoes(utilizador, recurso).Contains(Enum.Permissao.Gravar)) {
                    throw new Exceptions.SemPermissao();
                }

                //regras de inserção
                recursoHorario.Inicio = new DateTime(1900, 1, 1) + recursoHorario.Inicio.TimeOfDay;
                DateTime datafim = recursoHorario.Inicio.AddMinutes(recursoHorario.DuracaoSlot * recursoHorario.NumeroSlots);

                //Hora inicio (num slots * duração slot inferior < 24h)
                if (datafim.Subtract(recursoHorario.Inicio).TotalHours > 24
                       || (datafim.Day > recursoHorario.Inicio.Day && datafim.TimeOfDay > new TimeSpan(0, 0, 1))) {
                    return ActualizarRecursoHorarioResult.ErroSuperior24Horas;
                }

                //verifica se não existem colisões de horário
                IEnumerable<BD.RecursoHorario> horario = SeleccionarHorarioDiaSemana(recursoHorario.RecursoID, (DayOfWeek)recursoHorario.DiaSemana - 1);
                if (horario.Any(h => recursoHorario.RecursoHorarioID != h.RecursoHorarioID &&
                                    ((recursoHorario.Inicio.TimeOfDay >= h.Inicio.TimeOfDay &&
                                     recursoHorario.Inicio.TimeOfDay < h.Inicio.AddMinutes(h.NumeroSlots * h.DuracaoSlot).TimeOfDay) ||
                                     (datafim.TimeOfDay > h.Inicio.TimeOfDay &&
                                     datafim.TimeOfDay <= h.Inicio.AddMinutes(h.NumeroSlots * h.DuracaoSlot).TimeOfDay)))) {
                    return ActualizarRecursoHorarioResult.ErroColisaoSlots;
                }


                BD.RecursoHorario horarioOriginal = ctx.RecursoHorario.
                                            Where(rh => rh.RecursoHorarioID == recursoHorario.RecursoHorarioID).FirstOrDefault();

                if (horarioOriginal == null) {
                    throw new DadosIncorrectos();
                }

                IEnumerable<BD.RecursoReserva> reservasDiaSemana = ctx.RecursoReserva.
                                                    Where(rr => rr.RecursoID == recursoHorario.RecursoID &&
                                                    rr.DataHoraInicio > DateTime.Now &&
                                                    System.Data.Objects.SqlClient.SqlFunctions.DatePart("dw", rr.DataHoraInicio) == horarioOriginal.DiaSemana);

                IEnumerable<BD.RecursoReserva> reservasPeriodo = reservasDiaSemana.Where(rr => rr.DataHoraInicio.TimeOfDay >= horarioOriginal.Inicio.TimeOfDay &&
                                                                        rr.DataHoraInicio.TimeOfDay < horarioOriginal.Inicio.AddMinutes(horarioOriginal.DuracaoSlot * horarioOriginal.NumeroSlots).TimeOfDay);

                if (horarioOriginal.DuracaoSlot != recursoHorario.DuracaoSlot || horarioOriginal.DiaSemana != recursoHorario.DiaSemana ||
                    horarioOriginal.Inicio.TimeOfDay != recursoHorario.Inicio.TimeOfDay || horarioOriginal.NumeroSlots > recursoHorario.NumeroSlots) {
                    //se alterar a duração do slot, todas as reservas devem de ser canceladas
                    //se o inicio do horário ultrapassar alguma reserva ou se o número de slots diminuir, as reservas que batem fora do periodo têm de ser canceladas
                    if (reservasPeriodo.Count() > 0) {
                        if (!confirmar) {
                            return ActualizarRecursoHorarioResult.AvisoTodasReservasCanceladas;
                        } else {
                            //cancelar todas as reservas no periodo
                            foreach (BD.RecursoReserva r in reservasPeriodo) {
                                ctx.RecursoReserva.DeleteObject(r);
                                Notificacao.Processa(r.RecursoReservaID, Notificacao.Evento.ReservaCancelada, utilizador);
                            }
                        }
                    }
                }

                //como carreguei o objecto original da base de dados, para aplicar as alterações temos de utilizar o método ApplyCurrentValues
                ctx.RecursoHorario.ApplyCurrentValues(recursoHorario);
                ctx.SaveChanges();
                //limpar cahce de horário
                LimparHorarioDiaSemanaCache(recursoHorario.RecursoID);
                return ActualizarRecursoHorarioResult.Ok;
            }
        }

        #endregion

        #region eliminiar

        public void Apagar(long id, UtilizadorAutenticado utilizador) {

            using (BD.Context ctx = new BD.Context()) {

                Regras.BD.Recurso obj = ctx.Recurso.Where(r => r.RecursoID == id).FirstOrDefault();

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Apagar))
                    throw new Exceptions.SemPermissao();

                if (ctx.RecursoReserva.Any(rr => rr.RecursoID == id)) {
                    throw new RecursoComReservas();
                }

                _base.Apagar(obj.RecursoID, ctx);

                ctx.SaveChanges();
            }
        }

        public void ApagarRecursoHorario(long id, UtilizadorAutenticado utilizador) {

            using (BD.Context ctx = new BD.Context()) {

                Regras.BD.RecursoHorario recursoHorario = ctx.RecursoHorario.
                                                                Include("Recurso").
                                                                Where(rh => rh.RecursoHorarioID == id).FirstOrDefault();

                if (recursoHorario == null)
                    throw new Exceptions.DadosIncorrectos();

                //validar se podemos editar o recurso associado
                if (!Permissoes(utilizador, recursoHorario.Recurso).Contains(Enum.Permissao.Apagar))
                    throw new Exceptions.SemPermissao();

                IEnumerable<BD.RecursoReserva> reservasDiaSemana = ctx.RecursoReserva.
                                                                    Where(rr => rr.RecursoID == recursoHorario.RecursoID &&
                                                                    rr.DataHoraInicio > DateTime.Now &&
                                                                    System.Data.Objects.SqlClient.SqlFunctions.DatePart("dw", rr.DataHoraInicio) == recursoHorario.DiaSemana);

                IEnumerable<BD.RecursoReserva> reservasPeriodo = reservasDiaSemana.Where(rr => rr.DataHoraInicio.TimeOfDay >= recursoHorario.Inicio.TimeOfDay &&
                                                                        rr.DataHoraInicio.TimeOfDay < recursoHorario.Inicio.AddMinutes(recursoHorario.DuracaoSlot * recursoHorario.NumeroSlots).TimeOfDay);

                if (reservasPeriodo.Count() > 0) {
                    //cancelar todas as reservas no periodo
                    foreach (BD.RecursoReserva r in reservasPeriodo) {
                        ctx.RecursoReserva.DeleteObject(r);
                        Notificacao.Processa(r.RecursoReservaID, Notificacao.Evento.ReservaCancelada, utilizador);
                    }
                }

                ctx.RecursoHorario.DeleteObject(recursoHorario);
                ctx.SaveChanges();
                //limpar cahce de horário
                LimparHorarioDiaSemanaCache(recursoHorario.RecursoID);

            }
        }

        #endregion

        #region calendario

        #region construção calendário

        public IEnumerable<CalendarioDia> ConstroiCalendarioMes(int mes, int ano, long recursoID, UtilizadorAutenticado utilizador) {

            List<CalendarioDia> res = new List<CalendarioDia>();

            using (BD.Context ctx = new BD.Context()) {
                BD.Recurso recurso = ctx.Recurso.Where(r => r.RecursoID == recursoID).FirstOrDefault();

                if (recurso == null) {
                    throw new RecursoNaoExiste();
                }

                DateTime primeiroDiaMes = new DateTime(ano, mes, 1);
                DateTime ultimoDiaMes = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));
                IEnumerable<BD.RecursoReserva> reservasMes = ctx.RecursoReserva.Include("Utilizador").Where(rr =>
                                                            rr.RecursoID == recursoID &&
                                                            rr.DataHoraInicio >= primeiroDiaMes &&
                                                            rr.DataHoraInicio <= ultimoDiaMes).ToList();

                for (int dia = 1; dia <= DateTime.DaysInMonth(ano, mes); ++dia) {

                    CalendarioDia cDia = new CalendarioDia();
                    cDia.Data = new DateTime(ano, mes, dia);

                    IEnumerable<BD.RecursoReserva> reservasDia = reservasMes.Where(rr =>
                                                rr.RecursoID == recurso.RecursoID &&
                                                rr.DataHoraInicio.Date == cDia.Data.Date).ToList();

                    cDia.Slots = ConstroiCalendarioDia(cDia.Data, recurso, reservasDia, utilizador);
                    cDia.Disponibilidade = DisponibilidadeDia(cDia, recurso, reservasMes);

                    res.Add(cDia);
                }

            }

            return res;

        }

        private RecursoDisponibilidadeDia DisponibilidadeDia(CalendarioDia cDia, BD.Recurso recurso, IEnumerable<BD.RecursoReserva> reservas) {

            //datas anteriores ao dia actual estão indisponiveis
            if (cDia.Data.Date < DateTime.Now.Date) {
                return RecursoDisponibilidadeDia.Nao_Disponivel;
            }

            IEnumerable<BD.RecursoHorario> horarioDiaSemana = SeleccionarHorarioDiaSemana(recurso.RecursoID, cDia.Data.DayOfWeek);
            //datas que o dia da semana não tenha horario definido estao indisponiveis
            if (horarioDiaSemana == null || horarioDiaSemana.Count() == 0) {
                return RecursoDisponibilidadeDia.Nao_Disponivel;
            }

            //datas que não tenham o periodo minimo de aprovacao estao indisponiveis
            if (recurso.DiasMinAprovacao.HasValue && cDia.Data < DateTime.Now.AddDays(recurso.DiasMinAprovacao.Value)) {
                return RecursoDisponibilidadeDia.Nao_Disponivel;
            }

            int numeroSlotsNaoDisponivel = cDia.Slots.Count(s => s.Disponibilidade == RecursoDisponibilidadeSlot.Nao_Disponivel);

            if (numeroSlotsNaoDisponivel == horarioDiaSemana.Sum(h => h.NumeroSlots)) {
                //Já não existem slots para o dia
                return RecursoDisponibilidadeDia.Nao_Disponivel;
            }

            IEnumerable<BD.RecursoReserva> reservasDia = reservas.Where(r =>
                r.DataHoraInicio.Date == cDia.Data.Date && (!r.Recurso.RequerAprovacao ||
                       r.Recurso.RequerAprovacao && (!r.Aprovado.HasValue || r.Aprovado.Value)));

            if (DateTime.Now.Date == cDia.Data.Date) {
                //se estiver no dia actual ver apenas as reservas que ainda não passaram
                reservasDia = reservasDia.Where(r => r.DataHoraInicio.TimeOfDay >= DateTime.Now.TimeOfDay);

            }

            int numeroSlotsDisponiveisDia = horarioDiaSemana.Sum(h => h.NumeroSlots) - numeroSlotsNaoDisponivel;

            //se o número de reservas for igual ao número de slots disponiveis o dia está cheio
            int numeroReservasDia = reservasDia.Sum(r => r.NumeroSlots);
            if (numeroReservasDia >= numeroSlotsDisponiveisDia) {
                return RecursoDisponibilidadeDia.Completo;
            }

            //veririficar se o o número de reservas já atingiu o estado quase_completo
            if (((float)numeroReservasDia / numeroSlotsDisponiveisDia) > racioDiaQuaseCompleto) {
                return RecursoDisponibilidadeDia.Quase_Completo;
            }

            return RecursoDisponibilidadeDia.Disponivel;

        }

        public IEnumerable<CalendarioSlot> ConstroiCalendarioDia(DateTime data, long recursoID, UtilizadorAutenticado utilizador) {

            using (BD.Context ctx = new BD.Context()) {
                BD.Recurso recurso = ctx.Recurso.Where(r => r.RecursoID == recursoID).FirstOrDefault();

                IEnumerable<BD.RecursoReserva> reservasDia = ctx.RecursoReserva.Include("Utilizador").Where(rr =>
                                                rr.RecursoID == recurso.RecursoID &&
                                                EntityFunctions.TruncateTime(rr.DataHoraInicio) == EntityFunctions.TruncateTime(data)).ToList();

                return ConstroiCalendarioDia(data, recurso, reservasDia, utilizador);
            }

        }

        private IEnumerable<CalendarioSlot> ConstroiCalendarioDia(DateTime data, BD.Recurso recurso, IEnumerable<BD.RecursoReserva> reservasDia, UtilizadorAutenticado utilizador) {

            List<CalendarioSlot> res = new List<CalendarioSlot>();

            if (recurso == null) {
                throw new RecursoNaoExiste();
            }

            IEnumerable<BD.RecursoHorario> hConfiguracaoDia = SeleccionarHorarioDiaSemana(recurso.RecursoID, data.DayOfWeek);

            if (hConfiguracaoDia == null || hConfiguracaoDia.Count() == 0) {
                //este dia não tem horário definido
                return res;
            }

            foreach (BD.RecursoHorario horario in hConfiguracaoDia) {
                DateTime horarioInicio = new DateTime(data.Year, data.Month, data.Day, horario.Inicio.Hour, horario.Inicio.Minute, 0);
                for (int i = 0; i < horario.NumeroSlots; ++i) {
                    CalendarioSlot cSlot = new CalendarioSlot();
                    cSlot.Inicio = horarioInicio;
                    horarioInicio = horarioInicio.AddMinutes(horario.DuracaoSlot);
                    cSlot.Fim = horarioInicio;
                    BD.RecursoReserva reserva = SeleccionaReservaSlot(cSlot.Inicio, cSlot.Fim, reservasDia, horario.DuracaoSlot);
                    cSlot.Reserva = reserva;
                    cSlot.Disponibilidade = DisponibilidadeSlot(cSlot, recurso, reservasDia);
                    cSlot.Permissoes = PermissoesCalendario(utilizador, cSlot);
                    if (reserva != null) {
                        cSlot.UtilizadorReservaID = reserva.MoradorID;
                        cSlot.NomeReserva = !String.IsNullOrEmpty(reserva.Utilizador.Fraccao) ? String.Concat(reserva.Utilizador.Nome, " (", reserva.Utilizador.Fraccao, ")") : reserva.Utilizador.Nome;
                    }
                    res.Add(cSlot);
                }
            }


            return res;
        }

        private RecursoDisponibilidadeSlot DisponibilidadeSlot(CalendarioSlot slot, BD.Recurso recurso, IEnumerable<BD.RecursoReserva> reservasDia) {

            if (slot.Inicio < DateTime.Now) {
                return RecursoDisponibilidadeSlot.Nao_Disponivel;
            }

            if (slot.Reserva != null) {
                if (recurso.RequerAprovacao && !slot.Reserva.Aprovado.HasValue) {
                    //slot pendente de aprovação
                    return RecursoDisponibilidadeSlot.PendenteAprovacao;
                }
                if (recurso.RequerAprovacao && slot.Reserva.Aprovado.HasValue && !slot.Reserva.Aprovado.Value) {
                    //tem reserva mas foi cancelada
                    return RecursoDisponibilidadeSlot.Disponivel;
                }
                //slot indisponivel
                return RecursoDisponibilidadeSlot.Reservado;
            }

            //não existe reserva
            return RecursoDisponibilidadeSlot.Disponivel;

        }

        private BD.RecursoReserva SeleccionaReservaSlot(DateTime ReservaInicio, DateTime ReservaFim, IEnumerable<BD.RecursoReserva> reservasDia, short duracaoSlot) {

            return reservasDia.Where(r =>
                   (
                       r.DataHoraInicio == ReservaInicio ||
                       (r.DataHoraInicio < ReservaInicio &&
                       r.DataHoraInicio.AddMinutes(r.NumeroSlots * duracaoSlot) >= ReservaFim)
                    )
                    && (!r.Recurso.RequerAprovacao ||
                       r.Recurso.RequerAprovacao && (!r.Aprovado.HasValue || r.Aprovado.Value))).FirstOrDefault();

        }

        private IEnumerable<BD.RecursoHorario> SeleccionarHorarioDiaSemana(long recursoID, DayOfWeek diaSemana) {

            DateTimeOffset duracaoCache = DateTimeOffset.Now.AddHours(1);

            IEnumerable<BD.RecursoHorario> horarioSemana = null;

            //validar se existe em cache
            string chaveCache = String.Format("Recurso_HorarioDiaSemana_{0}", recursoID.ToString());
            horarioSemana = MemoryCache.Default.Get(chaveCache) as IEnumerable<BD.RecursoHorario>;
            if (horarioSemana == null) {
                using (BD.Context ctx = new BD.Context()) {
                    horarioSemana = ctx.RecursoHorario.Where(rh => rh.RecursoID == recursoID).OrderBy(rh => rh.Inicio).ToList();
                    //guardar em cache
                    MemoryCache.Default.Add(chaveCache, horarioSemana, duracaoCache);
                }
            }

            return horarioSemana.Where(rh =>
                                    rh.RecursoID == recursoID &&
                                    rh.DiaSemana == ((short)diaSemana + 1));

        }

        private void LimparHorarioDiaSemanaCache(long recursoID) {
            string chaveCache = String.Format("Recurso_HorarioDiaSemana_{0}", recursoID.ToString());
            MemoryCache.Default.Remove(chaveCache);
        }

        #endregion

        #region operações sobre calendario

        public ReservarRecursoResult Reservar(long recursoID, DateTime inicio, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {

                BD.Recurso recurso = ctx.Recurso.Where(r => r.RecursoID == recursoID).FirstOrDefault();

                if (recurso == null) {
                    throw new RecursoNaoExiste();
                }

                if (!recurso.Activo) {
                    throw new RecursoInactivo();
                }

                if (inicio < DateTime.Now) {
                    throw new ArgumentException("Não é possível fazer reservas para datas anteriores à data actual");
                }

                //validar se tem permissoes para reservar todos os slots
                IEnumerable<BD.RecursoHorario> hconfiguracao = SeleccionarHorarioDiaSemana(recursoID, inicio.DayOfWeek);

                CalendarioSlot slotReservar = ConstroiCalendarioDia(inicio, recursoID, utilizador).
                    Where(s => s.Inicio == inicio).ToList().FirstOrDefault();

                if (slotReservar == null || !PermissoesCalendario(utilizador, slotReservar).Contains(RecursoPermissao.ReservarSlot)) {
                    throw new Exceptions.SemPermissao();
                }

                //validar max slots reserva e validar intervalo reserva
                DateTime limiteInferior = inicio.AddHours(-recurso.IntervaloReserva);
                DateTime limiteSuperior = inicio.AddHours(recurso.IntervaloReserva);

                IEnumerable<BD.RecursoReserva> reservasUtilizadorRecurso = ctx.RecursoReserva.Where(rr =>
                    rr.RecursoID == recursoID &&
                    rr.MoradorID == utilizador.ID &&
                    rr.DataHoraInicio > limiteInferior &&
                    rr.DataHoraInicio < limiteSuperior
                    && (!rr.Aprovado.HasValue || rr.Aprovado.Value));

                if (reservasUtilizadorRecurso.Sum(r => r.NumeroSlots) >= recurso.MaxSlotsReserva) {
                    return ReservarRecursoResult.ErroMaxSlotsReservarIntervaloReserva;
                }

                //validar dias minimos de aprovacao
                if (recurso.RequerAprovacao && recurso.DiasMinAprovacao.HasValue &&
                        inicio.Date.Subtract(DateTime.Now).TotalDays < recurso.DiasMinAprovacao) {
                    return ReservarRecursoResult.ErroDiasMinimosAprovacao;
                }

                BD.RecursoReserva reserva = new BD.RecursoReserva();
                reserva.DataHoraInicio = inicio;
                reserva.MoradorID = utilizador.ID;
                reserva.NumeroSlots = 1;

                //Verificar se requer aprovação
                bool requerAprovacao = recurso.RequerAprovacao && utilizador.Perfil != Enum.Perfil.Síndico;
                if (!requerAprovacao) {
                    reserva.Aprovado = true; ;
                }

                recurso.RecursoReserva.Add(reserva);
                ctx.SaveChanges();

                if (requerAprovacao) {
                    Notificacao.Processa(reserva.RecursoReservaID, Notificacao.Evento.NovaReservaPendenteAprovacao, utilizador);
                } else {
                    Notificacao.Processa(reserva.RecursoReservaID, Notificacao.Evento.NovaReserva, utilizador);
                }

                return ReservarRecursoResult.Ok;

            }
        }

        public void CancelarReserva(long recursoID, long reservaID, UtilizadorAutenticado utilizador) {

            using (BD.Context ctx = new BD.Context()) {

                BD.RecursoReserva reserva = ctx.RecursoReserva.Where(rr =>
                    rr.RecursoReservaID == reservaID &&
                    rr.RecursoID == recursoID).FirstOrDefault();

                if (reserva == null) {
                    throw new DadosIncorrectos();
                }

                if (!PermissoesCalendario(utilizador, reserva).Contains(RecursoPermissao.CancelarReserva)) {
                    throw new Exceptions.SemPermissao();
                }

                if (utilizador.ID != reserva.MoradorID) {
                    Notificacao.Processa(reserva.RecursoReservaID, Notificacao.Evento.ReservaCancelada, utilizador);
                }

                ctx.RecursoReserva.DeleteObject(reserva);

                ctx.SaveChanges();

            }
        }

        public void AprovarReserva(long recursoID, long reservaID, UtilizadorAutenticado utilizador) {

            using (BD.Context ctx = new BD.Context()) {

                BD.RecursoReserva reserva = ctx.RecursoReserva.Where(rr =>
                    rr.RecursoReservaID == reservaID &&
                    rr.RecursoID == recursoID).FirstOrDefault();

                if (reserva == null) {
                    throw new DadosIncorrectos();
                }

                if (!PermissoesCalendario(utilizador, reserva).Contains(RecursoPermissao.AprovarReserva)) {
                    throw new Exceptions.SemPermissao();
                }

                reserva.Aprovado = true;
                reserva.DataAprovacao = DateTime.Now;
                reserva.UtilizadorAprovacaoID = utilizador.ID;
                ctx.SaveChanges();

                Notificacao.Processa(reserva.RecursoReservaID, Notificacao.Evento.NovaReservaAprovada, utilizador);

            }
        }

        public void ReprovarReserva(long recursoID, long reservaID, string comentario, UtilizadorAutenticado utilizador) {

            using (BD.Context ctx = new BD.Context()) {

                BD.RecursoReserva reserva = ctx.RecursoReserva.Where(rr =>
                    rr.RecursoReservaID == reservaID &&
                    rr.RecursoID == recursoID).FirstOrDefault();

                if (reserva == null) {
                    throw new DadosIncorrectos();
                }

                if (!PermissoesCalendario(utilizador, reserva).Contains(RecursoPermissao.AprovarReserva)) {
                    throw new Exceptions.SemPermissao();
                }

                reserva.Aprovado = false;
                reserva.UtilizadorAprovacaoID = utilizador.ID;
                reserva.Comentario = comentario;
                ctx.SaveChanges();

                Notificacao.Processa(reserva.RecursoReservaID, Notificacao.Evento.NovaReservaReprovada, utilizador);

            }
        }

        #endregion

        #endregion

        # region Permissões

        public static List<Enum.Permissao> Permissoes(UtilizadorAutenticado utilizador) {
            if (utilizador.CondominioID.HasValue) {
                if (utilizador.Perfil == Regras.Enum.Perfil.CondoClub ||
                    utilizador.Perfil == Regras.Enum.Perfil.Empresa ||
                    utilizador.Perfil == Regras.Enum.Perfil.Síndico) {
                    return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar };
                }
            }

            return new List<Enum.Permissao>();
        }

        public static List<Enum.Permissao> Permissoes(UtilizadorAutenticado utilizador, BD.Recurso obj) {
            if (utilizador.CondominioID.HasValue && utilizador.CondominioID.Value == obj.CondominioID) {
                if (utilizador.Perfil == Regras.Enum.Perfil.CondoClub ||
                    utilizador.Perfil == Regras.Enum.Perfil.Empresa ||
                    utilizador.Perfil == Regras.Enum.Perfil.Síndico) {
                    return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar, Enum.Permissao.Apagar };
                }
                if (utilizador.Perfil == Regras.Enum.Perfil.Morador ||
                    utilizador.Perfil == Regras.Enum.Perfil.Portaria) {
                    return new List<Enum.Permissao>() { Enum.Permissao.Visualizar };
                }
            }

            return new List<Enum.Permissao>();
        }

        public static List<RecursoPermissao> PermissoesCalendario(UtilizadorAutenticado utilizador) {

            List<RecursoPermissao> permissoes = new List<RecursoPermissao>();

            if (utilizador.Perfil == Enum.Perfil.Síndico ||
                utilizador.Perfil == Enum.Perfil.Morador ||
                utilizador.Perfil == Enum.Perfil.Consulta) {
                permissoes.Add(RecursoPermissao.VisualizarCalendario);
            }

            if (utilizador.Perfil == Enum.Perfil.Síndico ||
                utilizador.Perfil == Enum.Perfil.Morador) {
                permissoes.Add(RecursoPermissao.ReservarSlot);
            }

            if (utilizador.Perfil == Enum.Perfil.Síndico) {
                permissoes.Add(RecursoPermissao.VisualizarEditarRecursos);
            }

            return permissoes;
        }

        public static List<RecursoPermissao> PermissoesCalendario(UtilizadorAutenticado utilizador, CalendarioSlot cSlot) {

            List<RecursoPermissao> permissoes = new List<RecursoPermissao>();

            if (utilizador.Perfil == Enum.Perfil.Síndico) {
                permissoes.Add(RecursoPermissao.VisualizarSlotReservadoPor);
                if (cSlot.Reserva != null && cSlot.Disponibilidade == RecursoDisponibilidadeSlot.Reservado) {
                    permissoes.Add(RecursoPermissao.CancelarReserva);
                }
            }

            if (cSlot.Reserva != null && utilizador.ID == cSlot.Reserva.Utilizador.UtilizadorID && cSlot.Disponibilidade == RecursoDisponibilidadeSlot.Reservado) {
                permissoes.Add(RecursoPermissao.VisualizarSlotReservadoPor);
                permissoes.Add(RecursoPermissao.CancelarReserva);
            }

            if (cSlot.Reserva != null && utilizador.ID == cSlot.Reserva.Utilizador.UtilizadorID && cSlot.Disponibilidade == RecursoDisponibilidadeSlot.PendenteAprovacao) {
                permissoes.Add(RecursoPermissao.VisualizarSlotReservadoPor);
            }

            if (utilizador.Perfil == Enum.Perfil.Síndico && cSlot.Disponibilidade == RecursoDisponibilidadeSlot.PendenteAprovacao) {
                permissoes.Add(RecursoPermissao.AprovarReserva);
            }
            if ((utilizador.Perfil == Enum.Perfil.Síndico || utilizador.Perfil == Enum.Perfil.Morador) &&
                cSlot.Disponibilidade == RecursoDisponibilidadeSlot.Disponivel) {
                permissoes.Add(RecursoPermissao.ReservarSlot);
            }

            return permissoes;
        }

        public static List<RecursoPermissao> PermissoesCalendario(UtilizadorAutenticado utilizador, BD.RecursoReserva reserva) {
            List<RecursoPermissao> permissoes = new List<RecursoPermissao>();

            if (utilizador.Perfil == Enum.Perfil.Síndico) {
                permissoes.Add(RecursoPermissao.CancelarReserva);
                permissoes.Add(RecursoPermissao.AprovarReserva);
            }

            if (utilizador.Perfil == Enum.Perfil.Morador && reserva.Utilizador.UtilizadorID == utilizador.ID) {
                permissoes.Add(RecursoPermissao.CancelarReserva);
            }

            return permissoes;
        }

        #endregion

        #region Métodos auxiliares

        private bool RecurcoObjValido(BD.Recurso recurso) {
            if (String.IsNullOrEmpty(recurso.Designacao) || recurso.DiasMinAprovacao <= 0 || recurso.MaxSlotsReserva <= 0) {
                return false;
            }
            if (recurso.RequerAprovacao && recurso.DiasMinAprovacao <= 0) {
                return false;
            }
            return true;
        }

        private bool RecurcoHorarioObjValido(BD.RecursoHorario recursoHorario) {
            return (recursoHorario.RecursoID > 0 && recursoHorario.DiaSemana > 0 && recursoHorario.NumeroSlots > 0 && recursoHorario.DuracaoSlot > 0);
        }

        #endregion

    }

    public enum RecursoDisponibilidadeDia {
        Nao_Disponivel,
        Disponivel,
        Quase_Completo,
        Completo
    }

    public enum RecursoDisponibilidadeSlot {
        Nao_Disponivel,
        Disponivel,
        PendenteAprovacao,
        Reservado
    }

    public class CalendarioDia {
        public DateTime Data { get; set; }
        public RecursoDisponibilidadeDia Disponibilidade { get; set; }
        public IEnumerable<CalendarioSlot> Slots { get; set; }
        public int NumeroSlots { get { return Slots.Count(); } }
        public int NumeroSlotsLivres { get { return Slots.Where(s => s.Reserva == null).Count(); } }
    }

    public class CalendarioSlot {

        public BD.RecursoReserva Reserva { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fim { get; set; }
        public long? UtilizadorReservaID { get; set; }
        public string NomeReserva { get; set; }
        public RecursoDisponibilidadeSlot Disponibilidade { get; set; }
        public List<Regras.RecursoPermissao> Permissoes { get; set; }

    }

}
