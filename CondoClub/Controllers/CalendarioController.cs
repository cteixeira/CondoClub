using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CondoClub.Web.Controllers {

    [Authorize]
    public class CalendarioController : Controller {

        #region Acções

        [HttpGet]
        public ActionResult Index(string id) {
            try {

                List<Regras.RecursoPermissao> permissoes = Regras.Recurso.PermissoesCalendario(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.RecursoPermissao.VisualizarCalendario))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);

                Models.CalendarioIndexModel model = new Models.CalendarioIndexModel();

                if (!String.IsNullOrEmpty(id)) {
                    
                    model.ApresentaLista = false;

                    long reservaID = Convert.ToInt64(Regras.Util.Decifra(Regras.Util.UrlDecode(id)));
                    Regras.BD.RecursoReserva reserva = Regras.Recurso.AbrirReserva(reservaID, ControladorSite.Utilizador);

                    model.DetalheDiaMes = ConstroiDetalhe(reserva.RecursoID, reserva.DataHoraInicio);

                } else {

                    model.ApresentaLista = true;

                    int mes = DateTime.Now.Month;
                    int ano = DateTime.Now.Year;

                    Regras.BD.Recurso r = Regras.Recurso.Lista(ControladorSite.Utilizador, true).OrderBy(rr => rr.Designacao).FirstOrDefault();

                    if (r != null) {
                        model.ListaMes = ConstroiLista(r.RecursoID, mes, ano);
                    } else { 
                        model.ListaMes = new Models.CalendarioListaMes();
                    }
                }

                return View(model);

            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }

        [HttpPost]
        public PartialViewResult _Lista(long recurso, int mes, int ano) {
            try {

                List<Regras.RecursoPermissao> permissoes = Regras.Recurso.PermissoesCalendario(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.RecursoPermissao.VisualizarCalendario))
                    throw new Exception(Resources.Erro.Acesso);

                Models.CalendarioListaMes ret = ConstroiLista(recurso, mes, ano);

                return PartialView(ret);

            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return null;
            }
        }

        [HttpPost]
        public PartialViewResult _Detalhe(long recurso, DateTime Data) {
            try {

                List<Regras.RecursoPermissao> permissoes = Regras.Recurso.PermissoesCalendario(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.RecursoPermissao.VisualizarCalendario))
                    throw new Exception(Resources.Erro.Acesso);

                Models.CalendarioDetalheDia ret = ConstroiDetalhe(recurso, Data);

                return PartialView(ret);

            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return null;
            }
        }

        [HttpPost]
        public ActionResult _Reservar(long recurso, DateTime inicio) {
            try {

                Regras.Recurso rRecurso = new Regras.Recurso();
                Regras.ReservarRecursoResult res = rRecurso.Reservar(recurso, inicio, ControladorSite.Utilizador);

                if (res == Regras.ReservarRecursoResult.ErroDiasMinimosAprovacao) {
                    Regras.Recurso rRegras = new Regras.Recurso();
                    Regras.BD.Recurso obj = rRecurso.Abrir(recurso, ControladorSite.Utilizador);

                    return JavaScript(String.Format("alert('{0}')",
                        String.Format(Resources.Calendario.ErroDiasMinimosAprovacao, obj.DiasMinAprovacao)));
                }

                if (res == Regras.ReservarRecursoResult.ErroMaxSlotsReservarIntervaloReserva) {
                    Regras.Recurso rRegras = new Regras.Recurso();
                    Regras.BD.Recurso obj = rRecurso.Abrir(recurso, ControladorSite.Utilizador);

                    return JavaScript(String.Format("alert('{0}')",
                        String.Format(Resources.Calendario.ErroMaxSlotsReservarIntervaloReserva, obj.MaxSlotsReserva, obj.IntervaloReserva)));
                }

                Models.CalendarioDetalheDia ret = ConstroiDetalhe(recurso, inicio);
                return PartialView("_Detalhe", ret);

            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return null;
            }
        }

        [HttpPost]
        public PartialViewResult _CancelarReserva(long recurso, long reserva, DateTime Data) {
            try {

                Regras.Recurso rRecurso = new Regras.Recurso();
                rRecurso.CancelarReserva(recurso, reserva, ControladorSite.Utilizador);

                Models.CalendarioDetalheDia ret = ConstroiDetalhe(recurso, Data);
                return PartialView("_Detalhe", ret);

            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return null;
            }
        }

        [HttpPost]
        public PartialViewResult _AprovarReserva(long recurso, long reserva, DateTime Data) {
            try {

                Regras.Recurso rRecurso = new Regras.Recurso();
                rRecurso.AprovarReserva(recurso, reserva, ControladorSite.Utilizador);

                Models.CalendarioDetalheDia ret = ConstroiDetalhe(recurso, Data);
                return PartialView("_Detalhe", ret);

            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return null;
            }
        }

        [HttpPost]
        public PartialViewResult _ReprovarReserva(long recurso, long reserva, DateTime Data, string comentario) {
            try {

                Regras.Recurso rRecurso = new Regras.Recurso();
                rRecurso.ReprovarReserva(recurso, reserva, comentario, ControladorSite.Utilizador);

                Models.CalendarioDetalheDia ret = ConstroiDetalhe(recurso, Data);
                return PartialView("_Detalhe", ret);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return null;
            }
        }

        #endregion

        #region Funcões Auxiliares

        private Models.CalendarioListaMes ConstroiLista(long recursoID, int mes, int ano) {

            Regras.Recurso rRecurso = new Regras.Recurso();
            IEnumerable<Regras.CalendarioDia> cDias = rRecurso.ConstroiCalendarioMes(mes, ano, recursoID, ControladorSite.Utilizador);
            Models.CalendarioListaMes ret = new Models.CalendarioListaMes();
            ret.Dias = new List<Models.CalendarioDia>();

            foreach (Regras.CalendarioDia dia in cDias) {
                ret.Dias.Add(new Models.CalendarioDia(dia));
            }

            ret.Mes = mes;
            ret.Ano = ano;
            ViewData.Add("RecursoSeleccionadoID", recursoID);
            return ret;

        }

        private Models.CalendarioDetalheDia ConstroiDetalhe(long recursoID, DateTime Data) {

            Regras.Recurso rRecurso = new Regras.Recurso();
            IEnumerable<Regras.CalendarioSlot> cSlots = rRecurso.ConstroiCalendarioDia(Data, recursoID, ControladorSite.Utilizador);
            Models.CalendarioDetalheDia ret = new Models.CalendarioDetalheDia();
            ret.Slots = new List<Models.CalendarioSlot>();

            foreach (Regras.CalendarioSlot slot in cSlots) {
                ret.Slots.Add(new Models.CalendarioSlot(slot));
            }
            ret.Data = Data;
            ViewData.Add("RecursoSeleccionadoID", recursoID);

            return ret;

        }

        #endregion

    }

}