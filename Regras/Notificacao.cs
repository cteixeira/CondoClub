using System;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;

namespace CondoClub.Regras {

    public class Notificacao {

        internal enum Evento {
            NovoComunicado,
            NovaMensagem,
            NovaAgenda,
            NovoQuestionario,
            NovaReserva,
            NovaReservaPendenteAprovacao,
            NovaReservaAprovada,
            NovaReservaReprovada,
            ReservaCancelada,
            NovoArquivo,
            NovoVeiculo,
            NovoFuncionario,
            NovaEmpresa,
            NovaEmpresaActiva,
            NovoCondominio,
            NovoCondominioActivo,
            NovoMorador,
            NovoFornecedor,
            NovoFornecedorActivo,
            NovaPublicidade,
            NovaPublicidadeActiva,
            AvisoPagamentoCondominio,
            AvisoPagamentoFornecedor,
            AvisoPagamentoPublicidade,
            AtrasoPagamentoCondominio,
            AtrasoPagamentoFornecedor,
            AvisoFinalPagamentoCondominio,
            AvisoFinalPagamentoFornecedor,
            InactivacaoCondominio,
            InactivacaoFornecedor,
            PagamentoCondominioEfectuado,
            PagamentoFornecedorEfectuado,
            PagamentoPublicidadeEfectuado,
        }


        internal static void Processa(long id, Evento evento, UtilizadorAutenticado utilizador) {
            try {
                using (BD.Context ctx = new BD.Context()) {
                    switch (evento) {
                        case Evento.NovoComunicado:
                            break;
                        case Evento.NovaMensagem:
                            NovaMensagem(id, utilizador, ctx);
                            break;
                        case Evento.NovaAgenda:
                            NovaAgenda(id, utilizador, ctx);
                            break;
                        case Evento.NovaPublicidade:
                            NovaPublicidade(id, utilizador, ctx);
                            break;
                        case Evento.NovaPublicidadeActiva:
                            NovaPublicidadeActiva(id, utilizador, ctx);
                            break;
                        case Evento.NovoQuestionario:
                            NovoQuestionario(id, utilizador, ctx);
                            break;
                        case Evento.NovaReserva:
                            break;
                        case Evento.NovaReservaPendenteAprovacao:
                            NovaReservaPendenteAprovacao(id, utilizador, ctx);
                            break;
                        case Evento.NovaReservaAprovada:
                            ResultadoReserva(id, Resources.Notificacao.NovaReservaAprovada, utilizador, ctx);
                            break;
                        case Evento.NovaReservaReprovada:
                            ResultadoReserva(id, Resources.Notificacao.NovaReservaReprovada, utilizador, ctx);
                            break;
                        case Evento.ReservaCancelada:
                            ResultadoReserva(id, Resources.Notificacao.ReservaCancelada, utilizador, ctx);
                            break;
                        case Evento.NovoArquivo:
                            NovoArquivo(id, utilizador, ctx);
                            break;
                        case Evento.NovoVeiculo:
                            NovoVeiculo(id, utilizador, ctx);
                            break;
                        case Evento.NovoFuncionario:
                            NovoFuncionário(id, utilizador, ctx);
                            break;
                        case Evento.NovaEmpresa:
                            NovaEmpresa(id, utilizador, ctx);
                            break;
                        case Evento.NovaEmpresaActiva:
                            NovaEmpresaActiva(id, utilizador, ctx);
                            break;
                        case Evento.NovoCondominio:
                            NovoCondominio(id, utilizador, ctx);
                            break;
                        case Evento.NovoCondominioActivo:
                            NovoCondominioActivo(id, utilizador, ctx);
                            break;
                        case Evento.NovoMorador:
                            NovoMorador(id, utilizador, ctx);
                            break;
                        case Evento.NovoFornecedor:
                            NovoFornecedor(id, utilizador, ctx);
                            break;
                        case Evento.NovoFornecedorActivo:
                            NovoFornecedorActivo(id, utilizador, ctx);
                            break;
                        case Evento.AvisoPagamentoCondominio:
                            AvisoPagamentoCondominio(id, utilizador, ctx);
                            break;
                        case Evento.AvisoPagamentoFornecedor:
                            AvisoPagamentoFornecedor(id, utilizador, ctx);
                            break;
                        case Evento.AvisoPagamentoPublicidade:
                            AvisoPagamentoPublicidade(id, utilizador, ctx);
                            break;
                        case Evento.AtrasoPagamentoCondominio:
                            AvisoAtrasoPagamento(Evento.AvisoPagamentoCondominio, id, 
                                Resources.Notificacao.AtrasoPagamento, utilizador, ctx);
                            break;
                        case Evento.AtrasoPagamentoFornecedor:
                            AvisoAtrasoPagamento(Evento.AvisoPagamentoFornecedor, id, 
                                Resources.Notificacao.AtrasoPagamento, utilizador, ctx);
                            break;
                        case Evento.AvisoFinalPagamentoCondominio:
                            AvisoAtrasoPagamento(Evento.AvisoPagamentoCondominio, id, 
                                Resources.Notificacao.AvisoFinalPagamento, utilizador, ctx);
                            break;
                        case Evento.AvisoFinalPagamentoFornecedor:
                            AvisoAtrasoPagamento(Evento.AvisoPagamentoFornecedor, id, 
                                Resources.Notificacao.AvisoFinalPagamento, utilizador, ctx);
                            break;
                        case Evento.InactivacaoCondominio:
                            AvisoAtrasoPagamento(Evento.AvisoPagamentoCondominio, id, 
                                String.Format(Resources.Notificacao.Inactivacao, 
                                ConfigurationManager.AppSettings["EmailSuporte"]), utilizador, ctx);
                            break;
                        case Evento.InactivacaoFornecedor:
                            AvisoAtrasoPagamento(Evento.AvisoPagamentoFornecedor, id, 
                                String.Format(Resources.Notificacao.Inactivacao,
                                ConfigurationManager.AppSettings["EmailSuporte"]), utilizador, ctx);
                            break;
                        case Evento.PagamentoCondominioEfectuado:
                        case Evento.PagamentoFornecedorEfectuado:
                        case Evento.PagamentoPublicidadeEfectuado:
                            PagamentoEfectuado(evento, id, utilizador, ctx);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex){
                Util.TratamentoErro(null, "Notificacao", ex, utilizador);
            }
        }


        #region Controlo de eventos


        private static void NovaMensagem(long id, UtilizadorAutenticado utilizador, BD.Context ctx) {
            //TODO: COLOCAR EM QUEUE DE AZURE E CAPTURAR COM TAREFAS
            //Envia email aos destinatarios
            BD.Mensagem mensagem = ctx.Mensagem.Include("Utilizador").FirstOrDefault(o => o.MensagemID == id);

            string remetente = ConfigurationManager.AppSettings["AppEmail"]; 
            string destinatarios = String.Empty;
            foreach (var item in mensagem.MensagemDestinatario)
                destinatarios = String.Concat(destinatarios, (destinatarios.Length == 0 ? String.Empty: ","), item.Utilizador.Email);

            string texto = String.Concat(
                String.Format(Resources.Notificacao.NovaMensagemTexto, mensagem.Utilizador.Nome),
                "<br/><br/>",
                mensagem.Texto, "<br/><br/><a href='",
                ConfigurationManager.AppSettings["AppURL"], "/Mensagem'>", 
                Resources.Notificacao.VerMensagem , "</a>");

            Util.EnviaEmailAssincrono(remetente, destinatarios, Resources.Notificacao.NovaMensagem, texto, true, true, utilizador);
        }


        private static void NovaAgenda(long id, UtilizadorAutenticado utilizador, BD.Context ctx) {
            //Coloca um novo comunicado no condominio
            BD.Agenda agenda = ctx.Agenda.FirstOrDefault(o => o.AgendaID == id);
            NovoComunicado(
                utilizador.ID,
                agenda.CondominioID,
                String.Format(Resources.Notificacao.NovaAgenda, agenda.Designacao),
                ctx);
        }

        
        private static void NovoQuestionario(long id, UtilizadorAutenticado utilizador, BD.Context ctx) {
            //Coloca um novo comunicado no condominio com data de inicio e fim de questionário
            BD.Questionario questionario = ctx.Questionario.FirstOrDefault(o => o.QuestionarioID == id);
            NovoComunicado(
                utilizador.ID, 
                questionario.CondominioID, 
                String.Format(
                    Resources.Notificacao.NovoQuestionario, 
                    questionario.Questao, 
                    questionario.Inicio.ToShortDateString(), 
                    questionario.Fim.ToShortDateString()), 
                ctx);
        }


        private static void NovaReservaPendenteAprovacao(long id, UtilizadorAutenticado utilizador, BD.Context ctx) {
            //Envia NovaMensagem ao sindico e empresa caso exista
            BD.RecursoReserva reserva = ctx.RecursoReserva.FirstOrDefault(o => o.RecursoReservaID == id);
            string texto = String.Format(
                                Resources.Notificacao.NovaReservaRecurso,
                                reserva.Recurso.Condominio.Nome,
                                Util.UrlEncode(Util.Cifra(id.ToString())),
                                reserva.Recurso.Designacao,
                                String.Concat(reserva.DataHoraInicio.ToShortDateString(), " ", reserva.DataHoraInicio.ToString("HH:mm")));

            List<long> destinatarios = new List<long>();

            IEnumerable<BD.Utilizador> sindicos = ctx.Utilizador.Where(
                                                        o => o.CondominioID == utilizador.CondominioID && 
                                                        o.PerfilUtilizadorID == (int)Enum.Perfil.Síndico);
            foreach (var item in sindicos)
                destinatarios.Add(item.UtilizadorID);

            /*if (reserva.Utilizador.Condominio.EmpresaID != null) {
                IEnumerable<BD.Utilizador> empresa = ctx.Utilizador.Where(
                                                            o => o.EmpresaID == reserva.Utilizador.Condominio.EmpresaID &&
                                                            o.PerfilUtilizadorID == (int)Enum.Perfil.Empresa);
                foreach (var item in empresa)
                    destinatarios.Add(item.UtilizadorID);
            }*/

            if (destinatarios.Count > 0)
                new Mensagem().Inserir(texto, destinatarios, Evento.NovaReserva, id, false, utilizador);
        }


        private static void ResultadoReserva(long id, string texto, UtilizadorAutenticado utilizador, BD.Context ctx) {
            //Enviar mensagem de reprovação ao morador e a todos os destinatários da mensagem original, com comentário caso exista
            BD.RecursoReserva reserva = ctx.RecursoReserva.FirstOrDefault(o => o.RecursoReservaID == id);
            texto = String.Format(texto, reserva.Comentario != null ? reserva.Comentario : String.Empty);
            RespostaMensagem(texto, Evento.NovaReserva, id, utilizador, ctx);
        }


        private static void NovoArquivo(long id, UtilizadorAutenticado utilizador, BD.Context ctx) {
            //Coloca um novo comunicado no condominio
            //Usa cache para não repetir comunicados quando está a colocar muitos ficheiros

            //if (MemoryCache.Default.Get(String.Concat("NovoArquivo_", utilizador.ID)) != null)
            //    return;

            BD.ArquivoFicheiro arquivo = ctx.ArquivoFicheiro.FirstOrDefault(o => o.ArquivoFicheiroID == id);

            string texto = arquivo.ArquivoDirectoria.Nome;
            if (texto.Contains(Regras.ArquivoDirectoria.directorioRaiz))
                texto = String.Format(Resources.Notificacao.NovoFicheiroRaiz, 
                    Util.UrlEncode(Util.Cifra(arquivo.Ficheiro.FicheiroID.ToString())), arquivo.Ficheiro.Nome);
            else
                texto = String.Format(Resources.Notificacao.NovoFicheiro, 
                    Util.UrlEncode(Util.Cifra(arquivo.Ficheiro.FicheiroID.ToString())), arquivo.Ficheiro.Nome, texto);

            NovoComunicado(utilizador.ID, arquivo.CondominioID, texto, ctx);

            //MemoryCache.Default.Add(String.Concat("NovoArquivo_", utilizador.ID), true, DateTimeOffset.Now.AddMinutes(1));
        }


        private static void NovoVeiculo(long id, UtilizadorAutenticado utilizador, BD.Context ctx) {
            //Coloca um novo comunicado no condominio
            BD.Veiculo veiculo = ctx.Veiculo.FirstOrDefault(o => o.VeiculoID == id);
            NovoComunicado(
                utilizador.ID,
                veiculo.CondominioID,
                String.Format(
                    Resources.Notificacao.NovoVeiculo, 
                    veiculo.Matricula, 
                    String.Concat(veiculo.Utilizador.Nome, " (", veiculo.Utilizador.Fraccao, ")")),
                ctx);
        }


        private static void NovoFuncionário(long id, UtilizadorAutenticado utilizador, BD.Context ctx) {
            //Coloca um novo comunicado no condominio
            BD.Funcionario funcionario = ctx.Funcionario.FirstOrDefault(o => o.FuncionarioID == id);
            NovoComunicado(
                utilizador.ID,
                funcionario.CondominioID,
                String.Format(Resources.Notificacao.NovoFuncionario, funcionario.Nome, funcionario.Funcao, funcionario.Horario),
                ctx);
        }

        
        private static void NovaEmpresa(long id, UtilizadorAutenticado utilizador, BD.Context ctx) {
            //Envia nova mensagem ao condoclub
            BD.Empresa empresa = ctx.Empresa.FirstOrDefault(o => o.EmpresaID == id);
            string texto = String.Format(Resources.Notificacao.NovaEmpresa, empresa.Nome);
            NovaMensagemCondoClub(texto, Evento.NovaEmpresa, id, utilizador, ctx);
        }


        private static void NovaEmpresaActiva(long id, UtilizadorAutenticado utilizador, BD.Context ctx) {
            //Responde à mensagem original enviando as boas vindas
            RespostaMensagem(String.Format(Resources.Notificacao.NovoRegistoActivo, ConfigurationManager.AppSettings["AppURL"]),
                Evento.NovaEmpresa, id, utilizador, ctx);
        }


        private static void NovoCondominio(long id, UtilizadorAutenticado utilizador, BD.Context ctx) {
            //Envia nova mensagem ao condoclub
            BD.Condominio condominio = ctx.Condominio.FirstOrDefault(o => o.CondominioID == id);
            string texto = String.Format(Resources.Notificacao.NovoCondominio, condominio.Nome);
            NovaMensagemCondoClub(texto, Evento.NovoCondominio, id, utilizador, ctx);
        }


        private static void NovoCondominioActivo(long id, UtilizadorAutenticado utilizador, BD.Context ctx) {
            //Coloca um comunicado geral de boas-vindas no condominio com links para mail de suporte e tutorial
            BD.Condominio condominio = ctx.Condominio.FirstOrDefault(o => o.CondominioID == id);
            NovoComunicado(
                utilizador.ID,
                condominio.CondominioID,
                String.Format(Resources.Notificacao.ComunicadoCondominioActivo, ConfigurationManager.AppSettings["EmailSuporte"]),
                ctx);

            //Responde à mensagem original enviando as boas vindas
            RespostaMensagem(String.Format(Resources.Notificacao.NovoRegistoActivo, ConfigurationManager.AppSettings["AppURL"]), 
                Evento.NovoCondominio, id, utilizador, ctx);
        }


        private static void NovoMorador(long id, UtilizadorAutenticado utilizador, BD.Context ctx) {
            //Coloca um novo comunicado de boas-vindas no condominio
            BD.Utilizador morador = ctx.Utilizador.FirstOrDefault(o => o.UtilizadorID == id);
            NovoComunicado(
                utilizador.ID,
                morador.CondominioID.Value,
                String.Format(Resources.Notificacao.NovoMorador, morador.Nome, morador.Fraccao),
                ctx);
        }


        private static void NovoFornecedor(long id, UtilizadorAutenticado utilizador, BD.Context ctx) {
            //Envia nova mensagem ao condoclub
            BD.Fornecedor fornecedor = ctx.Fornecedor.FirstOrDefault(o => o.FornecedorID == id);
            string texto = String.Format(Resources.Notificacao.NovoFornecedor, fornecedor.Nome);
            NovaMensagemCondoClub(texto, Evento.NovoFornecedor, id, utilizador, ctx);
        }


        private static void NovoFornecedorActivo(long id, UtilizadorAutenticado utilizador, BD.Context ctx) {
            //Coloca um comunicado em todos os condominios com acesso ao fornecedor
            BD.Fornecedor fornecedor = ctx.Fornecedor.FirstOrDefault(o => o.FornecedorID == id);
            string texto = String.Format(Resources.Notificacao.ComunicadoFornecedorActivo, fornecedor.Nome, fornecedor.Descricao);

            IEnumerable<BD.FornecedorAlcance> alcance = ctx.FornecedorAlcance.Where(o => o.FornecedorID == id);
            using (BD.Context savectx = new BD.Context()) {
                foreach (var item in alcance) {
                    NovoComunicado(utilizador.ID, item.CondominioID, texto, savectx);
                }
            }

            //Responde à mensagem original enviando as boas vindas
            RespostaMensagem(String.Format(Resources.Notificacao.NovoRegistoActivo, ConfigurationManager.AppSettings["AppURL"]),
                Evento.NovoFornecedor, id, utilizador, ctx);
        }


        private static void NovaPublicidade(long id, UtilizadorAutenticado utilizador, BD.Context ctx) {
            //Envia nova mensagem ao condoclub
            BD.Publicidade publicidade = ctx.Publicidade.FirstOrDefault(o => o.PublicidadeID == id);
            string texto = String.Format(Resources.Notificacao.NovaPublicidade, publicidade.Fornecedor.Nome, publicidade.Titulo);
            NovaMensagemCondoClub(texto, Evento.NovaPublicidade, id, utilizador, ctx);
        }


        private static void NovaPublicidadeActiva(long id, UtilizadorAutenticado utilizador, BD.Context ctx) {
            //Coloca um comunicado em todos os condominios com acesso ao fornecedor
            BD.Publicidade publicidade = ctx.Publicidade.FirstOrDefault(o => o.PublicidadeID == id);
            string texto = String.Format(Resources.Notificacao.ComunicadoPublicidadeActiva, publicidade.Fornecedor.Nome,
                (publicidade.Texto != null ? publicidade.Texto : publicidade.Titulo));

            IEnumerable<BD.FornecedorAlcance> alcance = ctx.FornecedorAlcance.Where(o => o.FornecedorID == publicidade.FornecedorID);
            using (BD.Context savectx = new BD.Context()) {
                foreach (var item in alcance) {
                    NovoComunicado(utilizador.ID, item.CondominioID, texto, savectx);
                }
            }

            //Responde à mensagem original informando que publicidade está activa
            RespostaMensagem(
                String.Format(Resources.Notificacao.NovaPublicidadeActiva, publicidade.Titulo),
                Evento.NovaPublicidade, id, utilizador, ctx);
        }


        private static void AvisoPagamentoCondominio(long id, UtilizadorAutenticado utilizador, BD.Context ctx) {
            //Alerta sindico que existe um novo pagamento
            BD.CondominioPagamento pagamento = ctx.CondominioPagamento.FirstOrDefault(o => o.CondominioPagamentoID == id);

            string texto = String.Format(Resources.Notificacao.AvisoPagamentoCondominio, ConfigurationManager.AppSettings["AppURL"]);
            if (pagamento.FormaPagamentoID == (int)Enum.FormaPagamento.Boleto) {
                texto += String.Format(Resources.Notificacao.AvisoPagamentoBoleto, pagamento.ReferenciaPagamento);
                string idBoleto = Regras.Util.UrlEncode(Regras.Util.Cifra(String.Concat(Enum.OrigemPagamento.Condominio, "_", id.ToString())));
                texto += String.Format(Resources.Notificacao.ImprimirBoleto, ConfigurationManager.AppSettings["AppURL"], idBoleto);
            }
                

            List<long> destinatarios = new List<long>();
            IEnumerable<BD.Utilizador> sindicos = ctx.Utilizador.Where(
                                                        o => o.CondominioID == pagamento.CondominioID &&
                                                        o.PerfilUtilizadorID == (int)Enum.Perfil.Síndico);
            foreach (var item in sindicos)
                destinatarios.Add(item.UtilizadorID);

            if (destinatarios.Count > 0)
                new Mensagem().Inserir(texto, destinatarios, Evento.AvisoPagamentoCondominio, id, false, utilizador);
        }


        private static void AvisoPagamentoFornecedor(long id, UtilizadorAutenticado utilizador, BD.Context ctx) {
            //Alerta fornecedor que existe um novo pagamento
            BD.FornecedorPagamento pagamento = ctx.FornecedorPagamento.FirstOrDefault(o => o.FornecedorPagamentoID == id);

            string texto = String.Format(Resources.Notificacao.AvisoPagamentoFornecedor, ConfigurationManager.AppSettings["AppURL"]);
            if (pagamento.FormaPagamentoID == (int)Enum.FormaPagamento.Boleto) {
                texto += String.Format(Resources.Notificacao.AvisoPagamentoBoleto, pagamento.ReferenciaPagamento);
                string idBoleto = Regras.Util.UrlEncode(Regras.Util.Cifra(String.Concat(Enum.OrigemPagamento.Fornecedor, "_", id.ToString())));
                texto += String.Format(Resources.Notificacao.ImprimirBoleto, ConfigurationManager.AppSettings["AppURL"], idBoleto);
            }

            List<long> destinatarios = new List<long>();
            IEnumerable<BD.Utilizador> fornecedores = ctx.Utilizador.Where(
                                                        o => o.FornecedorID == pagamento.FornecedorID &&
                                                        o.PerfilUtilizadorID == (int)Enum.Perfil.Fornecedor);
            foreach (var item in fornecedores)
                destinatarios.Add(item.UtilizadorID);

            if (destinatarios.Count > 0)
                new Mensagem().Inserir(texto, destinatarios, Evento.AvisoPagamentoFornecedor, id, false, utilizador);

        }


        private static void AvisoPagamentoPublicidade(long id, UtilizadorAutenticado utilizador, BD.Context ctx) {
            //Responde à mensagem original informando que publicidade foi aprovada e existe um novo pagamento
            BD.Publicidade publicidade = ctx.Publicidade.FirstOrDefault(o => o.PublicidadeID == id);

            string texto = String.Format(Resources.Notificacao.AvisoPagamentoPublicidade, publicidade.Titulo, ConfigurationManager.AppSettings["AppURL"]);
            if (publicidade.FormaPagamentoID == (int)Enum.FormaPagamento.Boleto) { 
                texto += String.Format(Resources.Notificacao.AvisoPagamentoBoleto, publicidade.ReferenciaPagamento);
                string idBoleto = Regras.Util.UrlEncode(Regras.Util.Cifra(String.Concat(Enum.OrigemPagamento.Publicidade, "_", id.ToString())));
                texto += String.Format(Resources.Notificacao.ImprimirBoleto, ConfigurationManager.AppSettings["AppURL"], idBoleto);
            }

            RespostaMensagem(texto, Evento.NovaPublicidade, id, utilizador, ctx);

        }


        private static void AvisoAtrasoPagamento(Evento origemNotificacao, long id, string texto, UtilizadorAutenticado utilizador, BD.Context ctx) {
            //Enviar notificacao de atraso ou cancelamento
            RespostaMensagem(texto, origemNotificacao, id, utilizador, ctx);
        }


        private static void PagamentoEfectuado(Evento origemNotificacao, long id, UtilizadorAutenticado utilizador, BD.Context ctx) {

            string texto = String.Empty, assunto = String.Empty, destinatarios = String.Empty;

            if (origemNotificacao == Evento.PagamentoCondominioEfectuado) {
                assunto = Resources.Notificacao.PagamentoCondominioEfectuado;
                BD.CondominioPagamento cPagamento = ctx.CondominioPagamento.Include("Condominio").Include("Utilizador").Where(cp => cp.CondominioPagamentoID == id).FirstOrDefault();

                texto = String.Format(Resources.Notificacao.PagamentoCondominioEfectuadoTexto, cPagamento.Condominio.Nome, cPagamento.Inicio.ToShortDateString(), cPagamento.Fim.ToShortDateString());
                destinatarios = cPagamento.Utilizador.Email;

            } else if (origemNotificacao == Evento.PagamentoFornecedorEfectuado) {
                assunto = Resources.Notificacao.PagamentoFornecedorEfectuado;
                BD.FornecedorPagamento fPagamento = ctx.FornecedorPagamento.Include("Fornecedor").Include("Utilizador").Where(fp => fp.FornecedorPagamentoID == id).FirstOrDefault();
                texto = String.Format(Resources.Notificacao.PagamentoFornecedorEfectuadoTexto, fPagamento.Fornecedor.Nome, fPagamento.Inicio.ToShortDateString(), fPagamento.Fim.ToShortDateString());
                destinatarios = fPagamento.Utilizador.Email;

            } else if (origemNotificacao == Evento.PagamentoPublicidadeEfectuado) {
                assunto = Resources.Notificacao.PagamentoPublicidadeEfectuado;
                BD.Publicidade pPagamento = ctx.Publicidade.Include("Utilizador").Where(p => p.PublicidadeID == id).FirstOrDefault();
                texto = String.Format(Resources.Notificacao.PagamentoPublicidadeEfectuadoTexto, pPagamento.Titulo);
                destinatarios = pPagamento.Utilizador.Email;

            } else {
                throw new Exception("A origem de notificação é inválida neste método");
            }

            Util.EnviaEmailAssincrono(ConfigurationManager.AppSettings["AppEmail"], destinatarios, assunto, texto, true, true, utilizador);

        }

        #endregion


        #region Funções auxiliares


        private static void NovoComunicado(long utilizadorID, long condominioID, string texto, BD.Context ctx) {
            BD.Comunicado comunicado = new BD.Comunicado();
            comunicado.CondominioID = condominioID;
            comunicado.Texto = texto;
            comunicado.RemetenteID = utilizadorID;
            comunicado.DataHora = DateTime.Now;
            ctx.Comunicado.AddObject(comunicado);
            ctx.SaveChanges();
        }


        private static void NovaMensagemCondoClub(string texto, Evento origemNotificacao, long id, UtilizadorAutenticado utilizador, BD.Context ctx) {
            
            List<long> destinatarios = new List<long>();
            IEnumerable<BD.Utilizador> condoclub = ctx.Utilizador.Where(o => o.PerfilUtilizadorID == (int)Enum.Perfil.CondoClub);
            foreach (var item in condoclub)
                destinatarios.Add(item.UtilizadorID);

            if (destinatarios.Count > 0)
                new Mensagem().Inserir(texto, destinatarios, origemNotificacao, id, true, utilizador);
        }


        private static void RespostaMensagem(string texto, Evento origemNotificacao, long id, UtilizadorAutenticado utilizador, BD.Context ctx) {
            BD.Mensagem mensagem = ctx.Mensagem.
                FirstOrDefault(o => o.OrigemNotificacao == (short)origemNotificacao && o.OrigemNotificacaoID == id);

            if (mensagem != null) {
                new Mensagem().InserirResposta(
                    new BD.Mensagem() {
                        RespostaID = mensagem.MensagemID,
                        Texto = texto,
                        RemetenteID = utilizador.ID,
                        DataHora = DateTime.Now
                    },
                    utilizador);
            }
        }


        #endregion

    }

}
