using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CondoClub.Regras
{
    public class Questionario {

        private _Base<BD.Questionario> _base = new _Base<BD.Questionario>();

        public IEnumerable<BD.Questionario> Lista(UtilizadorAutenticado utilizador) {

            if (!Permissoes(utilizador).Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context()) {
                if (utilizador.Perfil == Enum.Perfil.CondoClub || utilizador.Perfil == Enum.Perfil.Síndico)
                    return ctx.Questionario
                        .Include("QuestionarioResposta")
                        .Include("QuestionarioResposta.Utilizador")
                        .Where(o => o.CondominioID == utilizador.CondominioID)
                        .OrderByDescending(o => o.Inicio)
                        .ToList();
                else {
                    DateTime dataActual = DateTime.Now.Date;
                    return ctx.Questionario
                        .Include("QuestionarioResposta")
                        .Include("QuestionarioResposta.Utilizador")
                        .Where(o => o.CondominioID == utilizador.CondominioID)
                        .OrderByDescending(o => o.Inicio)
                        .ToList();
                }
            }
        }


        public BD.Questionario Abrir(long id, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {
                BD.Questionario obj = ctx.Questionario
                                        .Include("QuestionarioResposta")
                                        .Include("QuestionarioResposta.Utilizador")
                                        .FirstOrDefault(o => o.QuestionarioID == id);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Visualizar))
                    throw new Exceptions.SemPermissao();

                return obj;
            }
        }


        public void Inserir(BD.Questionario obj, UtilizadorAutenticado utilizador)
        {
            //No minimo tem que ter duas opções de resposta
            if (string.IsNullOrEmpty(obj.Questao) || obj.Inicio == null || 
                obj.Fim == null || string.IsNullOrEmpty(obj.Opcao1) || 
                string.IsNullOrEmpty(obj.Opcao2) || obj.AutorID != utilizador.ID)
                throw new Exceptions.DadosIncorrectos();

            if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context())
            {
                ctx.Questionario.AddObject(obj);
                ctx.SaveChanges();
                Notificacao.Processa(obj.QuestionarioID, Notificacao.Evento.NovoQuestionario, utilizador);
            }
        }


        public void Actualizar(BD.Questionario obj, UtilizadorAutenticado utilizador) {
            
            //No minimo tem que ter duas opções de resposta
            if (string.IsNullOrEmpty(obj.Questao) || obj.Inicio == null ||
                obj.Fim == null || string.IsNullOrEmpty(obj.Opcao1) ||
                string.IsNullOrEmpty(obj.Opcao2) || obj.AutorID != utilizador.ID)
                throw new Exceptions.DadosIncorrectos();

            Regras.BD.Questionario original = _base.Abrir(obj.QuestionarioID);
            
            if (original == null)
                throw new Exceptions.DadosIncorrectos();

            if (!Permissoes(utilizador, original).Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context()) {
                _base.Actualizar(obj, ctx);
                ctx.SaveChanges();
            }
        }


        public void Apagar(long id, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {
                Regras.BD.Questionario registo = _base.Abrir(id, ctx);

                if (registo == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, registo).Contains(Enum.Permissao.Apagar))
                    throw new Exceptions.SemPermissao();

                _base.Apagar(registo.QuestionarioID, ctx);
                ctx.SaveChanges();
            }
        }


        public void InserirResposta(BD.QuestionarioResposta obj, UtilizadorAutenticado utilizador) {
            if (obj.OpcaoSeleccionada < 1 || obj.MoradorID < 1 || obj.QuestionarioID < 1)
                throw new Exceptions.DadosIncorrectos();

            using (BD.Context ctx = new BD.Context()) {
                Regras.BD.Questionario original = _base.Abrir(obj.QuestionarioID, ctx);

                if (original == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, original).Contains(Enum.Permissao.Visualizar))
                    throw new Exceptions.SemPermissao();

                if (!PermissoesResposta(utilizador, obj).Contains(Enum.Permissao.Gravar))
                    throw new Exceptions.SemPermissao();

                ctx.QuestionarioResposta.AddObject(obj);
                ctx.SaveChanges();
            }
        }


        public static int NumeroQuestionariosNaoRespondidos(UtilizadorAutenticado utilizador) {

            if (utilizador.CondominioID.HasValue) {
                using (BD.Context ctx = new BD.Context()) {
                    return ctx.Questionario.Count(q =>
                            q.CondominioID == utilizador.CondominioID &&
                            !q.QuestionarioResposta.Any(qr => qr.MoradorID == utilizador.ID)
                            && System.Data.Objects.EntityFunctions.TruncateTime(q.Fim) >= System.Data.Objects.EntityFunctions.TruncateTime(DateTime.Now));
                }
            }

            return 0;

        }


        #region Permissoes

        public static List<Enum.Permissao> Permissoes(UtilizadorAutenticado utilizador){

            if (utilizador.Perfil == Regras.Enum.Perfil.CondoClub) {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar };
            }
            else if (utilizador.Perfil == Regras.Enum.Perfil.Síndico) {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar };
            }
            else if (
                utilizador.Perfil == Regras.Enum.Perfil.Morador ||
                utilizador.Perfil == Regras.Enum.Perfil.Consulta ||
                utilizador.Perfil == Regras.Enum.Perfil.Portaria) {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar };
            }

            return new List<Enum.Permissao>();
        }


        public static List<Enum.Permissao> Permissoes(UtilizadorAutenticado utilizador, BD.Questionario obj)
        {
            if (utilizador.Perfil == Regras.Enum.Perfil.CondoClub) {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar, Enum.Permissao.Apagar };
            }
            else if (
                utilizador.Perfil == Regras.Enum.Perfil.Síndico && 
                utilizador.CondominioID == obj.CondominioID) {

                //O sindico apenas pode visualizar se já houve respostas ao comunicado
                if (obj.QuestionarioID > 0) {
                    using (BD.Context ctx = new BD.Context()) {
                        if (ctx.QuestionarioResposta.Any(o => o.QuestionarioID == obj.QuestionarioID))
                            return new List<Enum.Permissao>() { Enum.Permissao.Visualizar };
                    }
                }
                
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar, Enum.Permissao.Apagar };
            }
            else if (
                (utilizador.Perfil == Regras.Enum.Perfil.Morador ||
                utilizador.Perfil == Regras.Enum.Perfil.Consulta ||
                utilizador.Perfil == Regras.Enum.Perfil.Portaria) &&
                utilizador.CondominioID == obj.CondominioID) {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar };
            }

            return new List<Enum.Permissao>();
        }


        public static List<Enum.Permissao> PermissoesResposta(UtilizadorAutenticado utilizador, BD.QuestionarioResposta obj) {

            if (utilizador.Perfil == Regras.Enum.Perfil.CondoClub) {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar, Enum.Permissao.Apagar };
            }
            else if (
                utilizador.Perfil == Regras.Enum.Perfil.Síndico ||
                utilizador.Perfil == Regras.Enum.Perfil.Morador) {
                
                //Verificar se já votou neste questionário
                using (BD.Context ctx = new BD.Context()) {
                    if (ctx.QuestionarioResposta.Any(o => o.QuestionarioID == obj.QuestionarioID && o.MoradorID == utilizador.ID))
                        return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Apagar };
                    else
                        return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar };
                }
            }
            else if (
                utilizador.Perfil == Regras.Enum.Perfil.Consulta ||
                utilizador.Perfil == Regras.Enum.Perfil.Portaria) {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar };
            }
            
            return new List<Enum.Permissao>();
        }

        #endregion

    }
}