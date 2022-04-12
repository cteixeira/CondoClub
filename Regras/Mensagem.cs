using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using CondoClub.Regras.Exceptions;
using System.Runtime.Caching;
using System.Globalization;

namespace CondoClub.Regras {

    public enum MensagemPermissao {
        Visualizar,
        CriarMensagem,
        ResponderMensagem
    }

    public class Mensagem {

        private _Base<BD.Mensagem> _base = new _Base<BD.Mensagem>(); 

        #region consts

        private const long _condoClubDestinatarioID = 0;

        #endregion

        #region seleccionar

        public static IEnumerable<MensagemGrupo> ListaGrupos(UtilizadorAutenticado utilizador) {

            using (BD.Context ctx = new BD.Context()) {

                return ctx.Mensagem.Include("MensagemDestinatario.Utilizador")
                        .Include("Utilizador")
                        .Where(m => !m.RespostaID.HasValue &&
                                    (m.RemetenteID == utilizador.ID ||
                                    m.MensagemDestinatario.Any(md => md.DestinatarioID == utilizador.ID)) &&
                                    (!utilizador.Impersonating ||
                                    m.MensagemDestinatario.Any(md => md.Utilizador.CondominioID == utilizador.CondominioID) ||
                                    m.Utilizador.CondominioID == utilizador.CondominioID))
                        .Select(m => new MensagemGrupo {
                            MensagemID = m.MensagemID,
                            Remetente = m.Utilizador,
                            AvatarID = m.MensagemDestinatario.Count() == 1 ? (m.RemetenteID == utilizador.ID ? m.MensagemDestinatario.FirstOrDefault().Utilizador.AvatarID : m.Utilizador.AvatarID) : 0,
                            TextoCabecalho = m.TextoCabecalho,
                            DataHora = m.DataHora,
                            NrMensagensNovas = ctx.MensagemDestinatario.Count(md => md.DestinatarioID == utilizador.ID
                                && (md.Mensagem.MensagemID == m.MensagemID || md.Mensagem.RespostaID == m.MensagemID)
                                && !md.Visto),
                            UltimaRespostaData = m.UltimaRespostaData.HasValue ? m.UltimaRespostaData : m.DataHora,
                            UtlimaRespostaRemetente = m.UltimaRespostaRemetente,
                            UltimaRespostaTexto = m.UltimaRespostaTexto,
                        }).OrderByDescending(m => m.UltimaRespostaData).ToList();
            }

        }

        public static IEnumerable<MensagemDestinatario> ListaDestinatarios(string termoPesquisa, UtilizadorAutenticado utilizador, int take) {
            termoPesquisa = termoPesquisa.Trim().ToLower();
            if (!String.IsNullOrEmpty(termoPesquisa)) {
                using (BD.Context ctx = new BD.Context()) {
                    //retornar os destinatários excluindo o próprio utilizador
                    return GetDestinatarios(utilizador, ctx).
                        Where(md => md.Designacao.ToLower().ReplaceAccents().Contains(termoPesquisa.ToLower().ReplaceAccents()) && (!md.IsMorador || (md.IsMorador && md.ID != utilizador.ID))).
                        Take(take).
                        ToList();
                }
            } else {
                //retorna todos os destinatários possíveis
                using (BD.Context ctx = new BD.Context()) {
                    return GetDestinatarios(utilizador, ctx).Where(md => md.ID != utilizador.ID).ToList();
                }
            }

        }

        public IEnumerable<BD.Mensagem> DetalheGrupoMensagens(long mensagemID, UtilizadorAutenticado utilizador) {

            using (BD.Context ctx = new BD.Context()) {
                IEnumerable<BD.Mensagem> msgs = ctx.Mensagem.
                                                    Include("Utilizador").
                                                    Include("Utilizador.Condominio").
                                                    Include("Utilizador.Empresa").
                                                    Include("Utilizador.Fornecedor").
                                                    Include("Mensagemficheiro.Ficheiro").
                                                    Include("MensagemDestinatario").
                                                    Where(m => m.MensagemID == mensagemID || m.RespostaID == mensagemID).
                                                    OrderBy(m => m.DataHora).ToList();

                if (!Permissoes(utilizador, msgs).Contains(MensagemPermissao.Visualizar)) { 
                    throw new Exceptions.SemPermissao();
                }

                return msgs;

            }
        }

        public BD.Ficheiro SeleccionaMensagemFicheiro(long FicheiroID, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {
                BD.Mensagem msg = ctx.Mensagem.Include("MensagemFicheiro").
                    Where(m => m.MensagemFicheiro.
                                Contains(ctx.MensagemFicheiro.
                                            Where(mf => mf.FicheiroID == FicheiroID).FirstOrDefault())).FirstOrDefault();


                if (msg != null) {

                    if (!Permissoes(utilizador, msg).Contains(MensagemPermissao.Visualizar))
                        throw new Exceptions.SemPermissao();


                    return ctx.Ficheiro.Include("FicheiroConteudo").Where(f => f.FicheiroID == FicheiroID).FirstOrDefault();

                }

                return null;
            }
        }

        public static int NumeroMensagensNovas(UtilizadorAutenticado utilizador) {

            using (BD.Context ctx = new BD.Context()) {

                return ctx.MensagemDestinatario.Count(md =>
                    md.DestinatarioID == utilizador.ID &&
                    !md.Visto &&
                    (!utilizador.Impersonating
                        || md.Mensagem.MensagemDestinatario.Any(md1 => md1.Utilizador.CondominioID == utilizador.CondominioID)
                        || md.Mensagem.Utilizador.CondominioID == utilizador.CondominioID));

            }
        }

        #endregion

        #region inserir

        public void Inserir(string mensagem, IEnumerable<MensagemDestinatario> destinatarios, List<long> ficheiros, UtilizadorAutenticado utilizador) {

            if (!Permissoes(utilizador).Contains(MensagemPermissao.CriarMensagem))
                throw new Exceptions.SemPermissao();

            if (String.IsNullOrEmpty(mensagem) || destinatarios == null || destinatarios.Count() == 0)
                throw new Exceptions.DadosIncorrectos();

            using (BD.Context ctx = new BD.Context()) {

                Regras.BD.Mensagem newMensagem = new BD.Mensagem();
                newMensagem.RemetenteID = utilizador.ID;
                newMensagem.Texto = mensagem;
                newMensagem.DataHora = DateTime.Now;


                BD.Utilizador remetente = ctx.Utilizador.First(u => u.UtilizadorID == utilizador.ID);

                StringBuilder sbTextoCabecalho = new StringBuilder();
                sbTextoCabecalho.Append(FormataNomeRemetenteTextoCabecalho(remetente, destinatarios.Any(d => d.IsCondoClub || d.IsEmpresa), destinatarios.Any(d => d.IsCondominio)));

                IEnumerable<MensagemDestinatario> destsPermitidos = GetDestinatarios(utilizador, ctx);
                foreach (MensagemDestinatario dest in destinatarios) {
                    //validar que pode enviar para este destinatário
                    if (!destsPermitidos.Contains(dest)) {
                        throw new SemPermissao();
                    }
                    if (dest.IsMorador) {
                        //enviar para o morador
                        if (!newMensagem.MensagemDestinatario.Select(md => md.MensagemDestinatarioID).Contains(dest.ID)) { //validar que não está duplicado
                            if (!dest.IsCondominio && !destinatarios.Any(d => d.IsCondominio)) {
                                //apenas insere o nome do morador se não existir Todos os Moradores
                                sbTextoCabecalho.AppendFormat("{0},", dest.Designacao);
                            }
                            newMensagem.MensagemDestinatario.Add(new BD.MensagemDestinatario { DestinatarioID = dest.ID });
                        }
                        continue;
                    }
                    if (dest.IsSindico) {
                        //enviar para os sindicos
                        var uSindicos = ctx.Utilizador.Where(u => u.CondominioID == dest.ID && u.UtilizadorID != utilizador.ID &&
                                u.PerfilUtilizadorID == (int)Enum.Perfil.Síndico);
                        foreach (BD.Utilizador u in uSindicos) {
                            if (!newMensagem.MensagemDestinatario.Select(md => md.MensagemDestinatarioID).Contains(u.UtilizadorID)) { //validar que não está duplicado
                                newMensagem.MensagemDestinatario.Add(new BD.MensagemDestinatario { DestinatarioID = u.UtilizadorID });
                            }
                        }
                        sbTextoCabecalho.AppendFormat("{0},", dest.Designacao);
                        continue;
                    }
                    if (dest.IsCondominio) {
                        //enviar para todos os moradores excepto os utilizadores com perfil consulta
                        var uCondominio = ctx.Utilizador.Where(u => u.CondominioID == dest.ID && u.UtilizadorID != utilizador.ID &&
                                (u.PerfilUtilizadorID == (int)Enum.Perfil.Morador ||
                                u.PerfilUtilizadorID == (int)Enum.Perfil.Síndico ||
                                u.PerfilUtilizadorID == (int)Enum.Perfil.Portaria));
                        foreach (BD.Utilizador u in uCondominio) {
                            if (!newMensagem.MensagemDestinatario.Select(md => md.MensagemDestinatarioID).Contains(u.UtilizadorID)) { //validar que não está duplicado
                                newMensagem.MensagemDestinatario.Add(new BD.MensagemDestinatario { DestinatarioID = u.UtilizadorID });
                            }
                        }
                        sbTextoCabecalho.AppendFormat("{0},", dest.Designacao);
                        continue;
                    }
                    if (dest.IsEmpresa) {
                        //enviar para todos os utilizadores da empresa
                        var uEmpresa = ctx.Utilizador.Where(u => u.EmpresaID == dest.ID);
                        foreach (BD.Utilizador u in uEmpresa) {
                            newMensagem.MensagemDestinatario.Add(new BD.MensagemDestinatario { DestinatarioID = u.UtilizadorID });
                        }
                        sbTextoCabecalho.AppendFormat("{0},", dest.Designacao);
                        continue;
                    }
                    if (dest.IsCondoClub) {
                        //enviar para todos os utilizadores com perfil CondoClub
                        var uCondoClub = ctx.Utilizador.Where(u => u.PerfilUtilizadorID == (int)Enum.Perfil.CondoClub);
                        foreach (BD.Utilizador u in uCondoClub) {
                            newMensagem.MensagemDestinatario.Add(new BD.MensagemDestinatario { DestinatarioID = u.UtilizadorID });
                        }
                        sbTextoCabecalho.AppendFormat("{0},", dest.Designacao);
                        continue;
                    }
                }

                if (ficheiros != null) {

                    IEnumerable<BD.Ficheiro> ficheiroBD = ctx.Ficheiro.Where(f => ficheiros.Contains(f.FicheiroID));

                    foreach (BD.Ficheiro f in ficheiroBD) {
                        if (f.UtilizadorID != utilizador.ID) {
                            throw new SemPermissao();
                        }
                        if (!f.Temporario) {
                            throw new InvalidOperationException("O ficheiro já está associado com outro registo");
                        }
                        f.Temporario = false;
                        newMensagem.MensagemFicheiro.Add(new BD.MensagemFicheiro { Ficheiro = f });
                    }
                }

                _base.Inserir(newMensagem, ctx);

                newMensagem.TextoCabecalho = sbTextoCabecalho.ToString().TrimEnd(new char[] { ',' });
                if (newMensagem.TextoCabecalho.Length > 100) {
                    newMensagem.TextoCabecalho = newMensagem.TextoCabecalho.Substring(0, 100);
                }

                newMensagem.UltimaRespostaRemetente = FormataNomeUtilizadorMensagem(remetente);
                newMensagem.UltimaRespostaTexto = FormataTextoUltimaMensagem(newMensagem.Texto);

                //base.Inserir(newMensagem, ctx);
                ctx.SaveChanges();

                //Notificação
                Notificacao.Processa(newMensagem.MensagemID, Notificacao.Evento.NovaMensagem, utilizador);

            }
        }

        public void InserirResposta(Regras.BD.Mensagem obj, UtilizadorAutenticado utilizador) {

            if (!Permissoes(utilizador).Contains(MensagemPermissao.ResponderMensagem))
                throw new Exceptions.SemPermissao();

            if (obj == null || String.IsNullOrEmpty(obj.Texto))
                throw new Exceptions.DadosIncorrectos();

            using (BD.Context ctx = new BD.Context()) {
                if (obj.RespostaID.HasValue && obj.RespostaID.Value > 0) {
                    BD.Mensagem msgOriginal = ctx.Mensagem.Where(m => m.MensagemID == obj.RespostaID).First();

                    if (!Permissoes(utilizador, msgOriginal).Contains(MensagemPermissao.Visualizar))
                        throw new Exceptions.SemPermissao();

                    //Destinatários da mensagem original
                    List<BD.Utilizador> destinatarios = msgOriginal.MensagemDestinatario.
                        Where(md => md.DestinatarioID != obj.RemetenteID).Select(md => md.Utilizador).ToList();

                    if (msgOriginal.RemetenteID != obj.RemetenteID) {
                        //adicionar o remetente da mensagem original aos destinatários da resposta (se não for o mesmo da mensagem original)
                        destinatarios.Add(msgOriginal.Utilizador);
                    }

                    //copiar os destinatários
                    foreach (BD.Utilizador destinatario in destinatarios) {
                        BD.MensagemDestinatario destinatarioCopiado = new BD.MensagemDestinatario();
                        destinatarioCopiado.DestinatarioID = destinatario.UtilizadorID;
                        destinatarioCopiado.Visto = false;
                        obj.MensagemDestinatario.Add(destinatarioCopiado);
                    }

                    BD.Utilizador remetente = ctx.Utilizador.First(u => u.UtilizadorID == utilizador.ID);

                    msgOriginal.UltimaRespostaRemetente = FormataNomeUtilizadorMensagem(remetente);
                    msgOriginal.UltimaRespostaTexto = FormataTextoUltimaMensagem(obj.Texto);
                    msgOriginal.UltimaRespostaData = obj.DataHora;

                    _base.Inserir(obj, ctx);
                    ctx.SaveChanges();

                    //Notificação
                    Notificacao.Processa(obj.MensagemID, Notificacao.Evento.NovaMensagem, utilizador);

                }
            }
        }

        internal void Inserir(string mensagem, IEnumerable<long> destinatarios, Notificacao.Evento? origemNotificacao, long? origemNotificacaoID, bool MensagemParaCondoclub, UtilizadorAutenticado utilizador) {

            using (BD.Context ctx = new BD.Context()) {

                if (string.IsNullOrEmpty(mensagem) || destinatarios == null || destinatarios.Count() == 0 || utilizador == null) {
                    throw new DadosIncorrectos();
                }

                IEnumerable<BD.Utilizador> dests = ctx.Utilizador.Where(u => destinatarios.Contains(u.UtilizadorID)).ToList();
                BD.Utilizador remetente = ctx.Utilizador.First(u => u.UtilizadorID == utilizador.ID);

                Regras.BD.Mensagem newMensagem = new BD.Mensagem();
                newMensagem.RemetenteID = utilizador.ID;
                newMensagem.Texto = mensagem;
                newMensagem.DataHora = DateTime.Now;
                newMensagem.UltimaRespostaRemetente = FormataNomeUtilizadorMensagem(remetente);
                newMensagem.UltimaRespostaTexto = FormataTextoUltimaMensagem(newMensagem.Texto);
                newMensagem.OrigemNotificacao = (short)origemNotificacao;
                newMensagem.OrigemNotificacaoID = origemNotificacaoID;

                StringBuilder sbTextoCabecalho = new StringBuilder();

                sbTextoCabecalho.AppendFormat("{0},", utilizador.Nome);
                foreach (BD.Utilizador dest in dests) {
                    newMensagem.MensagemDestinatario.Add(new BD.MensagemDestinatario { Utilizador = dest });
                    sbTextoCabecalho.AppendFormat("{0},", dest.Nome);
                }

                if (MensagemParaCondoclub) {
                   
                   sbTextoCabecalho = new StringBuilder(FormataNomeRemetenteTextoCabecalho(remetente, true, false));
                   sbTextoCabecalho.AppendFormat(" {0}", Resources.Mensagem.CondoClub);

                   newMensagem.TextoCabecalho = sbTextoCabecalho.ToString();
                } else {
                    newMensagem.TextoCabecalho = sbTextoCabecalho.ToString();
                }

                if (newMensagem.TextoCabecalho.Length > 100) {
                    newMensagem.TextoCabecalho = newMensagem.TextoCabecalho.Substring(0, 100);
                }

                _base.Inserir(newMensagem, ctx);
                ctx.SaveChanges();

                //Notificação
                Notificacao.Processa(newMensagem.MensagemID, Notificacao.Evento.NovaMensagem, utilizador);
            }

        }

        internal void InserirMensagemFornecedor(string mensagem, long destinatario, UtilizadorAutenticado utilizador) {

            using (BD.Context ctx = new BD.Context()) {

                if (!Permissoes(utilizador).Contains(MensagemPermissao.CriarMensagem))
                    throw new Exceptions.SemPermissao();

                if (string.IsNullOrEmpty(mensagem) || destinatario == 0 || utilizador == null) {
                    throw new DadosIncorrectos();
                }

                BD.Utilizador dest = ctx.Utilizador.Where(u => u.UtilizadorID == destinatario).First();
                string destNome = dest.Fornecedor.Nome;

                BD.Utilizador remetente = ctx.Utilizador.First(u => u.UtilizadorID == utilizador.ID);
                string remetenteNome = String.Format("{0} ({1})", remetente.Nome, remetente.Condominio.Nome);

                Regras.BD.Mensagem newMensagem = new BD.Mensagem();
                newMensagem.RemetenteID = utilizador.ID;
                newMensagem.Texto = mensagem;
                newMensagem.DataHora = DateTime.Now;
                newMensagem.MensagemDestinatario.Add(new BD.MensagemDestinatario { Utilizador = dest });
                newMensagem.TextoCabecalho = String.Concat(remetenteNome, ", ", destNome);
                newMensagem.UltimaRespostaRemetente = FormataNomeUtilizadorMensagem(remetente);
                newMensagem.UltimaRespostaTexto = FormataTextoUltimaMensagem(newMensagem.Texto);

                if (newMensagem.TextoCabecalho.Length > 100) {
                    newMensagem.TextoCabecalho = newMensagem.TextoCabecalho.Substring(0, 100);
                }

                _base.Inserir(newMensagem, ctx);
                ctx.SaveChanges();

                //Notificação
                Notificacao.Processa(newMensagem.MensagemID, Notificacao.Evento.NovaMensagem, utilizador);

            }

        }

        #endregion

        #region actualizar

        public void ActualizaMensagensVistas(long mensagemID, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {
                IEnumerable<BD.Mensagem> msgs = ctx.Mensagem.Include("MensagemDestinatario")
                            .Where(m => m.MensagemID == mensagemID || m.RespostaID == mensagemID).ToList();

                if (!Permissoes(utilizador, msgs).Contains(MensagemPermissao.Visualizar))
                    throw new Exceptions.SemPermissao();

                foreach (BD.Mensagem msg in msgs) {
                    BD.MensagemDestinatario msgDestinatario = msg.MensagemDestinatario.Where(md => md.DestinatarioID == utilizador.ID).FirstOrDefault();
                    if (msgDestinatario != null) {
                        msgDestinatario.Visto = true;
                    }
                }

                ctx.SaveChanges();
            }
        }

        #endregion

        #region private methods

        private string FormataNomeRemetenteTextoCabecalho(BD.Utilizador remetente, bool destIsCondoClubOrEmpresa, bool destIsTodosMoradores) {
            
            string ret = "";

            if (remetente.PerfilUtilizadorID == (Int32)Enum.Perfil.CondoClub) {
                ret = String.Format("{0}, ", Resources.Mensagem.CondoClub);
            } else if (remetente.PerfilUtilizadorID == (Int32)Enum.Perfil.Empresa) {
                ret = String.Format("{0}, ", remetente.Empresa.Nome);
            } else if (remetente.PerfilUtilizadorID == (Int32)Enum.Perfil.Fornecedor) {
                ret = String.Format("{0} ({1}), ", remetente.Nome, remetente.Fornecedor.Nome);
            } else if (remetente.PerfilUtilizadorID == (Int32)Enum.Perfil.Síndico) {
                if (!destIsTodosMoradores) {
                    //apenas coloca o nome do sindico se a mensagem não for para todo o condominio
                    if (destIsCondoClubOrEmpresa) {
                        ret = String.Format("{0} {1}, ", Resources.Mensagem.SindicoCondominio, remetente.Condominio.Nome);
                    } else {
                        if (!String.IsNullOrEmpty(remetente.Fraccao)) {
                            ret = String.Format("{0} ({1} - {2}), ", remetente.Nome, remetente.Fraccao, Resources.Mensagem.Sindico);
                        } else {
                            ret = String.Format("{0} ({1}), ", remetente.Nome, Resources.Mensagem.Sindico);
                        }
                    }
                }
            } else if (remetente.PerfilUtilizadorID == (Int32)Enum.Perfil.Portaria) {
                if (!destIsTodosMoradores) {
                    //apenas coloca o nome do porteiro se a mensagem não for para todo o condominio
                    ret = String.Format("{0} ({1}), ", remetente.Nome, Resources.Mensagem.Portaria);
                }
            } else {
                if (!destIsTodosMoradores) {
                    //apenas coloca o nome do morador se a mensagem não for para todo o condominio
                    if (!String.IsNullOrEmpty(remetente.Fraccao)) {
                        ret = String.Format("{0} ({1}), ", remetente.Nome, remetente.Fraccao);
                    } else {
                        ret = remetente.Nome;
                    }
                }
            }

            return ret;
        }
        
        //Este método também é utilizado pelo modelo para formatar o nome do remetente da mensagem, quando é expandida
        public static string FormataNomeUtilizadorMensagem(BD.Utilizador remetente) {

            string ret = "";

            if (remetente.PerfilUtilizadorID == (Int32)Enum.Perfil.CondoClub) {
                ret = String.Format("{0} ({1}) ", remetente.Nome, Resources.Mensagem.CondoClub);
            } else if (remetente.PerfilUtilizadorID == (Int32)Enum.Perfil.Empresa) {
                ret = String.Format("{0} ({1}) ", remetente.Nome, remetente.Empresa.Nome);
            } else if (remetente.PerfilUtilizadorID == (Int32)Enum.Perfil.Fornecedor) {
                ret = String.Format("{0} ({1}) ", remetente.Nome, remetente.Fornecedor.Nome);
            } else if (remetente.PerfilUtilizadorID == (Int32)Enum.Perfil.Síndico) {
                if (!String.IsNullOrEmpty(remetente.Fraccao)) {
                    ret = String.Format("{0} ({1} - {2}) ", remetente.Nome, remetente.Fraccao, Resources.Mensagem.Sindico);
                } else {
                    ret = String.Format("{0} ({1}) ", remetente.Nome, Resources.Mensagem.Sindico);
                }
            } else if (remetente.PerfilUtilizadorID == (Int32)Enum.Perfil.Portaria) {
                ret = String.Format("{0} ({1}) ", remetente.Nome, Resources.Mensagem.Portaria);
            } else {
                if (!String.IsNullOrEmpty(remetente.Fraccao)) {
                    ret = String.Format("{0} ({1}) ", remetente.Nome, remetente.Fraccao);
                } else {
                    ret = remetente.Nome;
                } 
            }

            return ret;

        }

        private string FormataTextoUltimaMensagem(string texto) {
            texto = texto.Replace("<br/>", String.Empty);
            texto = texto.Replace("<strong>", String.Empty);
            texto = texto.Replace("</strong>", String.Empty);
            return texto.Length > 200 ? texto.Substring(0, 200) : texto;
        }

        private static IEnumerable<MensagemDestinatario> GetDestinatarios(UtilizadorAutenticado utilizador, BD.Context ctx) {

            IEnumerable<MensagemDestinatario> ret = null;
            DateTimeOffset duracaoCache = DateTimeOffset.Now.AddHours(1);

            if (utilizador.Perfil == Enum.Perfil.CondoClub) {
                //CondoClub pode enviar mensagens:Empresa(todos utilizadores da empresa), Condominio(todos moradores + sindico), sindico condominio,
                //outros utilizadores com perfil condoclub

                //validar se existe em cache
                string chaveCache = "MensagemDestinatario_CondoClub";
                ret = MemoryCache.Default.Get(chaveCache) as IEnumerable<MensagemDestinatario>;
                if (ret != null) {
                    return ret;
                }

                //Empresas Gestoras (apenas as que têm utilizadores)
                ret = ctx.Empresa.Where(e => ctx.Utilizador.Any(u => u.Activo && u.EmpresaID == e.EmpresaID)).Select(e => new MensagemDestinatario {
                    ID = e.EmpresaID,
                    AvatarID = e.AvatarID,
                    Designacao = e.Nome,
                    IsEmpresa = true
                });


                //Condominios
                ret = ret.Union(
                    ctx.Condominio.
                        Select(c => new MensagemDestinatario {
                            ID = c.CondominioID,
                            AvatarID = c.AvatarID,
                            Designacao = Resources.Mensagem.TodosMoradores + " " + c.Nome,
                            IsCondominio = true
                        })
                );

                //Sindico Condominio
                IEnumerable<BD.Utilizador> sindicos = ctx.Utilizador.
                        Where(u => u.PerfilUtilizadorID == (int)Enum.Perfil.Síndico);
                foreach (var s in sindicos) {
                    if (!ret.Any(md => md.IsSindico && md.ID == s.CondominioID)) {
                        ret = ret.Concat(new[] {
                            new MensagemDestinatario {
                                ID = s.CondominioID.Value,
                                AvatarID = s.Condominio.AvatarID,
                                Designacao = Resources.Mensagem.SindicoCondominio + " " + s.Condominio.Nome,
                                IsSindico = true
                            }
                        });
                    }
                }

                //Outros utilizadores com perfil condoclub
                ret = ret.Union(
                    ctx.Utilizador.
                        Where(u => u.PerfilUtilizadorID == (int)Enum.Perfil.CondoClub).
                        Select(u => new MensagemDestinatario {
                            ID = u.UtilizadorID,
                            AvatarID = u.AvatarID,
                            Designacao = u.Nome,
                            IsMorador = true
                        })
                );

                //guardar em cache
                MemoryCache.Default.Add(chaveCache, ret.ToList(), duracaoCache);

                return ret;

            }

            if (utilizador.Perfil == Enum.Perfil.Empresa && utilizador.EmpresaID != null) {
                //Empresa pode enviar mensagens: Condominio(todos moradores + sindico), "Sindico Condominio X",
                //CondoClub(todos os utilizadores com perfil CondoClub)

                //validar se existe em cache
                string chaveCache = string.Concat("MensagemDestinatario_Empresa_", utilizador.EmpresaID);
                ret = MemoryCache.Default.Get(chaveCache) as IEnumerable<MensagemDestinatario>;
                if (ret != null) {
                    return ret;
                }

                //Condominios
                ret = ctx.Condominio.
                        Where(c => c.EmpresaID == utilizador.EmpresaID).
                        Select(c => new MensagemDestinatario {
                            ID = c.CondominioID,
                            AvatarID = c.AvatarID,
                            Designacao = Resources.Mensagem.TodosMoradores + " " + c.Nome,
                            IsCondominio = true
                        });

                //Sindico Condominio
                IEnumerable<BD.Utilizador> sindicos = ctx.Utilizador.
                        Where(u => u.PerfilUtilizadorID == (int)Enum.Perfil.Síndico && u.Condominio.EmpresaID == utilizador.EmpresaID);
                foreach (var s in sindicos) {
                    if (!ret.Any(md => md.IsSindico && md.ID == s.CondominioID)) {
                        ret = ret.Concat(new[] {
                            new MensagemDestinatario {
                                ID = s.CondominioID.Value,
                                AvatarID = s.Condominio.AvatarID,
                                Designacao = Resources.Mensagem.SindicoCondominio + " " + s.Condominio.Nome,
                                IsSindico = true
                            }
                        });
                    }
                }

                //CondoClub
                BD.Utilizador uCondoClub = ctx.Utilizador.
                    First(u => u.PerfilUtilizadorID == (int)Enum.Perfil.CondoClub);

                ret = ret.Concat(new[] { 
                    new MensagemDestinatario{ 
                        ID= _condoClubDestinatarioID,
                        Designacao= Resources.Mensagem.CondoClub,
                        AvatarID = uCondoClub.AvatarID,
                        IsCondoClub = true
                    } 
                });

                //guardar em cache
                MemoryCache.Default.Add(chaveCache, ret.ToList(), duracaoCache);

                return ret;

            }

            if (utilizador.Perfil == Enum.Perfil.Síndico && utilizador.CondominioID != null) {
                //Sindico pode enviar mensagens:CondoClub(todos os utilizadores com perfil CondoClub),
                //Empresa Gestora Condominio(todos utilizadores da empresa), Morador, "Todos Moradores"

                //validar se existe em cache
                string chaveCache = string.Concat("MensagemDestinatario_Sindico_", utilizador.CondominioID);
                ret = MemoryCache.Default.Get(chaveCache) as IEnumerable<MensagemDestinatario>;
                if (ret != null) {
                    return ret;
                }

                BD.Condominio condoUtilizador = ctx.Condominio.Where(c => c.CondominioID == utilizador.CondominioID).First();

                //Empresa Gestora
                ret = ctx.Empresa.
                        Where(e => condoUtilizador.EmpresaID != null && e.EmpresaID == condoUtilizador.EmpresaID &&
                            ctx.Utilizador.Any(u => u.Activo && u.EmpresaID == e.EmpresaID)).
                        Select(e => new MensagemDestinatario {
                            ID = e.EmpresaID,
                            AvatarID = e.AvatarID,
                            Designacao = e.Nome,
                            IsEmpresa = true
                        });

                //Moradores
                ret = ret.Union(
                    ctx.Utilizador.
                        Where(u => u.CondominioID == utilizador.CondominioID &&
                                u.PerfilUtilizadorID == (int)Enum.Perfil.Morador).
                        Select(u => new MensagemDestinatario {
                            ID = u.UtilizadorID,
                            AvatarID = u.AvatarID,
                            Designacao = !String.IsNullOrEmpty(u.Fraccao) ? u.Nome + " (" + u.Fraccao + ")" : u.Nome,
                            IsMorador = true
                        })
                );

                //Sindico
                ret = ret.Union(
                    ctx.Utilizador.
                        Where(u => u.CondominioID == utilizador.CondominioID &&
                               u.PerfilUtilizadorID == (int)Enum.Perfil.Síndico).
                        Select(u => new MensagemDestinatario {
                            ID = u.UtilizadorID,
                            AvatarID = u.AvatarID,
                            Designacao = !String.IsNullOrEmpty(u.Fraccao) ? u.Nome + " (" + u.Fraccao + " - " + Resources.Mensagem.Sindico + ")" : u.Nome + " (" + Resources.Mensagem.Sindico + ")",
                            IsMorador = true
                        })
                    );

                //Portaria
                ret = ret.Union(
                        ctx.Utilizador.
                        Where(u => u.CondominioID == utilizador.CondominioID &&
                               u.PerfilUtilizadorID == (int)Enum.Perfil.Portaria).
                        Select(u => new MensagemDestinatario {
                            ID = u.UtilizadorID,
                            AvatarID = u.AvatarID,
                            Designacao = u.Nome + " (" + Resources.Mensagem.Portaria + ")",
                            IsMorador = true
                        })
                    );

                //CondoClub
                BD.Utilizador uCondoClub = ctx.Utilizador.
                    First(u => u.PerfilUtilizadorID == (int)Enum.Perfil.CondoClub);

                ret = ret.Concat(new[] { 
                    new MensagemDestinatario{ 
                        ID= _condoClubDestinatarioID,
                        Designacao= Resources.Mensagem.CondoClub,
                        AvatarID = uCondoClub.AvatarID,
                        IsCondoClub = true
                    } 
                });


                //"Todos Moradores"
                if (!utilizador.Impersonating) {
                    //é um sindico
                    ret = ret.Concat(new[] { 
                        new MensagemDestinatario{ 
                            ID= utilizador.CondominioID.Value,
                            Designacao= Resources.Mensagem.TodosMoradores,
                            AvatarID = condoUtilizador.AvatarID,
                            IsCondominio = true
                        } 
                    });
                } else {
                    //é o condoclub ou empresa no contexto de um condominio
                    ret = ret.Concat(new[] { 
                        new MensagemDestinatario{ 
                            ID= utilizador.CondominioID.Value,
                            Designacao= Resources.Mensagem.TodosMoradores,
                            AvatarID = condoUtilizador.AvatarID,
                            IsCondominio = true
                        } 
                    });
                }

                //guardar em cache
                MemoryCache.Default.Add(chaveCache, ret.ToList(), duracaoCache);

                return ret;

            }

            if ((utilizador.Perfil == Enum.Perfil.Morador ||
                    utilizador.Perfil == Enum.Perfil.Portaria) &&
                    utilizador.CondominioID != null) {
                //Morador pode enviar mensagens: Morador, "Todos Moradores"

                //validar se existe em cache
                string chaveCache = string.Concat("MensagemDestinatario_Morador_", utilizador.CondominioID);
                ret = MemoryCache.Default.Get(chaveCache) as IEnumerable<MensagemDestinatario>;
                if (ret != null) {
                    return ret;
                }

                BD.Condominio condoUtilizador = ctx.Condominio.Where(c => c.CondominioID == utilizador.CondominioID).First();

                //Moradores
                ret = ctx.Utilizador.
                        Where(u => u.CondominioID == utilizador.CondominioID &&
                                u.PerfilUtilizadorID == (int)Enum.Perfil.Morador).
                        Select(u => new MensagemDestinatario {
                            ID = u.UtilizadorID,
                            AvatarID = u.AvatarID,
                            Designacao = !String.IsNullOrEmpty(u.Fraccao) ? u.Nome + " (" + u.Fraccao + ")" : u.Nome,
                            IsMorador = true
                        });

                //Sindico
                ret = ret.Union(
                        ctx.Utilizador.
                        Where(u => u.CondominioID == utilizador.CondominioID &&
                               u.PerfilUtilizadorID == (int)Enum.Perfil.Síndico).
                        Select(u => new MensagemDestinatario {
                            ID = u.UtilizadorID,
                            AvatarID = u.AvatarID,
                            Designacao = !String.IsNullOrEmpty(u.Fraccao) ? u.Nome + " (" + u.Fraccao + " - " + Resources.Mensagem.Sindico + ")" : u.Nome + " (" + Resources.Mensagem.Sindico + ")",
                            IsMorador = true
                        })
                    );

                //Portaria
                ret = ret.Union(
                        ctx.Utilizador.
                        Where(u => u.CondominioID == utilizador.CondominioID &&
                               u.PerfilUtilizadorID == (int)Enum.Perfil.Portaria).
                        Select(u => new MensagemDestinatario {
                            ID = u.UtilizadorID,
                            AvatarID = u.AvatarID,
                            Designacao = u.Nome + " (" + Resources.Mensagem.Portaria + ")",
                            IsMorador = true
                        })
                    );


                //"Todos Moradores"
                ret = ret.Concat(new[] { 
                    new MensagemDestinatario{ 
                        ID= utilizador.CondominioID.Value,
                        Designacao= String.Concat(Resources.Mensagem.TodosMoradores, " ", utilizador.EntidadeNome),
                        AvatarID = condoUtilizador.AvatarID,
                        IsCondominio = true
                    } 
                });

                //guardar em cache
                MemoryCache.Default.Add(chaveCache, ret.ToList(), duracaoCache);

                return ret;

            }

            throw new SemPermissao();

        }

        #endregion

        # region Permissões

        public static List<MensagemPermissao> Permissoes(UtilizadorAutenticado utilizador) {
            if (utilizador.Perfil == Enum.Perfil.CondoClub ||
                utilizador.Perfil == Enum.Perfil.Empresa ||
                utilizador.Perfil == Enum.Perfil.Síndico ||
                utilizador.Perfil == Enum.Perfil.Morador ||
                utilizador.Perfil == Enum.Perfil.Portaria) {
                return new List<MensagemPermissao>() { MensagemPermissao.Visualizar, MensagemPermissao.CriarMensagem, MensagemPermissao.ResponderMensagem };
            }
            if (utilizador.Perfil == Enum.Perfil.Fornecedor) {
                return new List<MensagemPermissao>() { MensagemPermissao.Visualizar, MensagemPermissao.ResponderMensagem };
            }
            return new List<MensagemPermissao>();
        }

        public static List<MensagemPermissao> Permissoes(UtilizadorAutenticado utilizador, IEnumerable<BD.Mensagem> MensagemGrupo) {
            BD.Mensagem MensagemOriginal = MensagemGrupo.Where(m => !m.RespostaID.HasValue).First();
            return Permissoes(utilizador, MensagemOriginal);
        }

        public static List<MensagemPermissao> Permissoes(UtilizadorAutenticado utilizador, BD.Mensagem MensagemGrupo) {

            if (MensagemGrupo.RemetenteID == utilizador.ID || MensagemGrupo.MensagemDestinatario.Any(md => md.DestinatarioID == utilizador.ID)) {
                return new List<MensagemPermissao>() { MensagemPermissao.Visualizar, MensagemPermissao.ResponderMensagem };
            }

            return new List<MensagemPermissao>();

        }

        #endregion

    }

    public class MensagemGrupo {
        public long MensagemID { get; set; }
        public long? AvatarID { get; set; }
        public BD.Utilizador Remetente { get; set; }
        public string TextoCabecalho { get; set; }
        public DateTime DataHora { get; set; }
        public int NrMensagensNovas { get; set; }
        public DateTime? UltimaRespostaData { get; set; }
        public string UtlimaRespostaRemetente { get; set; }
        public string UltimaRespostaTexto { get; set; }
    }

    public class MensagemDestinatario {

        public long ID { get; set; }
        public string Designacao { get; set; }
        public long? AvatarID { get; set; }

        public bool IsMorador { get; set; }
        public bool IsEmpresa { get; set; }
        public bool IsSindico { get; set; }
        public bool IsCondominio { get; set; }
        public bool IsCondoClub { get; set; }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public override bool Equals(object obj) {
            MensagemDestinatario y = obj as MensagemDestinatario;
            if (y != null) {
                return this.ID == y.ID && this.IsMorador == y.IsMorador && this.IsEmpresa == y.IsEmpresa &&
                        this.IsCondominio == y.IsCondominio && this.IsCondoClub == y.IsCondoClub;
            }
            return base.Equals(obj);
        }

    }

}
