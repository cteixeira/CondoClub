using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CondoClub.Web.Controllers {
    [Authorize]
    public class RecursoController : Controller {

        #region Constantes

        private const int nrReservasIni = 30;
        private const int nrReservasNext = 10;

        private static Dictionary<int, string> DiasSemana = new Dictionary<int, string> { 
            { (int)DayOfWeek.Sunday + 1, Resources.Calendario.DiaSemana1 },
            { (int)DayOfWeek.Monday + 1, Resources.Calendario.DiaSemana2 },
            { (int)DayOfWeek.Tuesday + 1, Resources.Calendario.DiaSemana3 },
            { (int)DayOfWeek.Wednesday + 1, Resources.Calendario.DiaSemana4 },
            { (int)DayOfWeek.Thursday + 1, Resources.Calendario.DiaSemana5 },
            { (int)DayOfWeek.Friday + 1, Resources.Calendario.DiaSemana6 },
            { (int)DayOfWeek.Saturday + 1, Resources.Calendario.DiaSemana7 },
        };

        private static Dictionary<Regras.Enum.EstadoRecursoReserva, string> EstadoReserva = new Dictionary<Regras.Enum.EstadoRecursoReserva, string> { 
            { Regras.Enum.EstadoRecursoReserva.Reservado, Resources.Recurso.EstadoReservaReservado },
            { Regras.Enum.EstadoRecursoReserva.PendenteAprovacao, Resources.Recurso.EstadoReservaPendenteAprovacao },
            { Regras.Enum.EstadoRecursoReserva.NaoAprovado, Resources.Recurso.EstadoReservaReprovado }
        };

        #endregion

        #region Acções
        
        public ActionResult Index() {
            try {

                List<Regras.Enum.Permissao> permissoes = Regras.Recurso.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);

                return View(ConstroiLista());

            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }

        public PartialViewResult _DetalheVisualizar(long id) {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.Recurso.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.Recurso model = new Models.Recurso(
                    new Regras.Recurso().Abrir(id, ControladorSite.Utilizador)
                );

                return PartialView("_DetalheVisualizar", model);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }

        public PartialViewResult _DetalheEditar(long id) {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.Recurso.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Gravar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.Recurso model = new Models.Recurso(
                    new Regras.Recurso().Abrir(id, ControladorSite.Utilizador)
                );

                return PartialView("_DetalheEditar", model);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }

        [HttpPost]
        public ActionResult Gravar(Models.Recurso registo, bool? confirmar) {
            try {
                if (ModelState.IsValid) {
                    if (!registo.ID.HasValue) {
                        Regras.BD.Recurso bdModel = registo.ToBDModel();
                        new Regras.Recurso().Inserir(bdModel, ControladorSite.Utilizador);
                        List<Models.Recurso> modelList = new List<Models.Recurso>();
                        modelList.Add(new Models.Recurso(bdModel));
                        return PartialView("_Lista", modelList);
                    } else {
                        Regras.Recurso rRecurso = new Regras.Recurso();

                        Regras.BD.Recurso bdModel = registo.ToBDModel();
                        Regras.ActualizarRecursoResult res = rRecurso.Actualizar(bdModel, ControladorSite.Utilizador, confirmar.HasValue ? confirmar.Value : false);

                        if (res == Regras.ActualizarRecursoResult.AvisoMaxSlotsReservaIntervaloReserva) {
                            return JavaScript(String.Format("ConfirmarActualizarRecurso({0}, '{1}')", registo.ID, Resources.Recurso.AvisoMaxSlotsReservaIntervaloReserva));
                        }

                        return PartialView("_DetalheVisualizar", new Models.Recurso(rRecurso.Abrir(bdModel.RecursoID, ControladorSite.Utilizador)));
                    }
                } else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            } catch (Regras.Exceptions.NomeRepetido) {
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.NomeRepetido));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }

        [HttpDelete]
        public ActionResult Apagar(long id) {
            try {
                if (id > 0) {
                    new Regras.Recurso().Apagar(id, ControladorSite.Utilizador);
                    return JavaScript("AfterDelete(" + id + ");");
                } else
                    throw new Exception(Resources.Erro.DadosIncorrectos);

            } catch (Regras.Exceptions.RecursoComReservas) {
                return JavaScript(String.Format("alert('{0}')", Resources.Recurso.ErroRecursoComReservas));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }

        public PartialViewResult _DetalheHorarioEditar(long id) {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.Recurso.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Gravar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.RecursoHorario model = new Models.RecursoHorario(
                    new Regras.Recurso().AbrirHorario(id, ControladorSite.Utilizador)
                );

                return PartialView("_DetalheHorarioEditar", model);

            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }

        public PartialViewResult _DetalheHorarioVisualizar(long id) {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.Recurso.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Gravar))
                    throw new Exception(Resources.Erro.Acesso);

                Models.RecursoHorario model = new Models.RecursoHorario(
                    new Regras.Recurso().AbrirHorario(id, ControladorSite.Utilizador)
                );

                return PartialView("_DetalheHorarioVisualizar", model);

            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }

        public ActionResult _DetalheHorarioGravar(Models.RecursoHorario recursoHorario, bool? confirmar) {

            try {
                if (ModelState.IsValid) {
                    if (!recursoHorario.RecursoHorarioID.HasValue) {

                        Regras.BD.RecursoHorario bdModel = recursoHorario.ToBDModel();
                        Regras.Recurso rRecurso = new Regras.Recurso();
                        Regras.InserirRecursoHorarioResult res = rRecurso.InserirRecursoHorario(bdModel, ControladorSite.Utilizador);

                        if (res == Regras.InserirRecursoHorarioResult.ErroSuperior24Horas) {
                            return JavaScript(String.Format("alert('{0}')", Resources.Recurso.ErroSuperior24Horas));
                        }

                        if (res == Regras.InserirRecursoHorarioResult.ErroColisaoSlots) {
                            return JavaScript(String.Format("alert('{0}')", Resources.Recurso.ErroColisaoSlots));
                        }

                        Regras.BD.Recurso recurso = rRecurso.Abrir(bdModel.RecursoID, ControladorSite.Utilizador);
                        return PartialView("_ListaHorario", new Models.Recurso(recurso));

                    } else {
                        Regras.BD.RecursoHorario bdModel = recursoHorario.ToBDModel();
                        
                        Regras.ActualizarRecursoHorarioResult res = new Regras.Recurso().ActualizarRecursoHorario(bdModel, ControladorSite.Utilizador, confirmar.HasValue ? confirmar.Value : false);

                        if (res == Regras.ActualizarRecursoHorarioResult.ErroSuperior24Horas) {
                            return JavaScript(String.Format("alert('{0}')", Resources.Recurso.ErroSuperior24Horas));
                        }

                        if (res == Regras.ActualizarRecursoHorarioResult.ErroColisaoSlots) {
                            return JavaScript(String.Format("alert('{0}')", Resources.Recurso.ErroColisaoSlots));
                        }

                        if (res == Regras.ActualizarRecursoHorarioResult.AvisoTodasReservasCanceladas) {
                            return JavaScript(String.Format("ConfirmarActualizarRecursoHorario({0}, '{1}')", recursoHorario.RecursoHorarioID, Resources.Recurso.AvisoTodasReservasCanceladas));
                        }

                        return PartialView("_DetalheHorarioVisualizar", new Models.RecursoHorario(bdModel));
                    }
                } else
                    throw new Exception(Resources.Erro.DadosIncorrectos);
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }

        public ActionResult _DetalheHorarioApagar(long id) {
            try {
                if (id > 0) {
                    new Regras.Recurso().ApagarRecursoHorario(id, ControladorSite.Utilizador);
                    return JavaScript("AfterDeleteRecursoHorario(" + id + ");");
                } else
                    throw new Exception(Resources.Erro.DadosIncorrectos);

            } catch (Regras.Exceptions.RecursoComReservas) {
                return JavaScript(String.Format("alert('{0}')", Resources.Recurso.ErroRecursoComReservas));
            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(String.Format("alert('{0}')", Resources.Erro.Geral));
            }
        }

        #region Lista Reservas

        public ActionResult ListaReserva() {
            try {

                List<Regras.Enum.Permissao> permissoes = Regras.Recurso.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);
                ViewData.Add("nrReservasIni", nrReservasIni);
                ViewData.Add("nrReservasNext", nrReservasNext);

                return View(ConstroiListaReservas(new Models.ListaReservaFiltro(), 0, nrReservasIni));

            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }

        public PartialViewResult _ListaReserva(Models.ListaReservaFiltro filtro, int? count) {
            try {

                List<Regras.Enum.Permissao> permissoes = Regras.Recurso.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);

                int skip = count.HasValue ? count.Value : 0;
                //se o count é 0 então é uma nova pesquisa, carregar o numero de registos iniciais
                int take = (!count.HasValue || count.Value == 0) ? nrReservasIni : nrReservasNext;

                return PartialView("_ListaReserva", ConstroiListaReservas(filtro, skip, take));


            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }

        #endregion

        #endregion

        #region Funções Auxiliares

        public IEnumerable<Models.Recurso> ConstroiLista() {

            IEnumerable<Regras.BD.Recurso> listaObj = Regras.Recurso.Lista(ControladorSite.Utilizador);

            List<Models.Recurso> modelList = new List<Models.Recurso>();
            foreach (var obj in listaObj) {
                modelList.Add(new Models.Recurso(obj));
            }

            return modelList.OrderBy(x => x.Designacao);

        }

        public SelectList ConstroiDropDown(bool? recursoActivo, long? recursoSeleccionado) {
            List<Models.Recurso> modelList = new List<Models.Recurso>();
            IEnumerable<Regras.BD.Recurso> recursos = Regras.Recurso.Lista(ControladorSite.Utilizador, recursoActivo).OrderBy(r => r.Designacao);

            foreach (Regras.BD.Recurso r in recursos) {
                modelList.Add(new Models.Recurso(r));
            }

            return new SelectList(modelList, "ID", "Designacao", recursoSeleccionado);
        }

        public SelectList ConstroiDropDownDiasSemana() {
            return new SelectList(DiasSemana.Select(o => new { ID = o.Key, Designacao = o.Value }), "ID", "Designacao");
        }

        public SelectList ConstroiDropDownEstadoReserva() {
            return new SelectList(EstadoReserva.Select(o => new { ID = o.Key, Designacao = o.Value }), "ID", "Designacao");
        }

        public IEnumerable<Models.RecursoReserva> ConstroiListaReservas(Models.ListaReservaFiltro filtro, int skip, int take) {

            IEnumerable<Regras.BD.RecursoReserva> listaObj = Regras.Recurso.ListaRecursoReserva(ControladorSite.Utilizador.CondominioID.Value, filtro.RecursoID,
                                                                    filtro.MoradorID, filtro.DataInicio, filtro.DataFim, filtro.Estado, skip, take, ControladorSite.Utilizador);

            List<Models.RecursoReserva> modelList = new List<Models.RecursoReserva>();
            foreach (var obj in listaObj) {
                modelList.Add(new Models.RecursoReserva(obj));
            }

            return modelList;

        }

        #endregion

    }
}
