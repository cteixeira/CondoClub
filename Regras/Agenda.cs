using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CondoClub.Regras
{
    public class Agenda
    {

        private _Base<BD.Agenda> _base = new _Base<BD.Agenda>();


        public IEnumerable<BD.Agenda> Lista(string designacao, UtilizadorAutenticado utilizador)
        {
            if (!Permissoes(utilizador).Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context())
            {
                return ctx.Agenda.Where(cr => cr.CondominioID == utilizador.CondominioID.Value &&
                    (designacao == null || cr.Designacao.Contains(designacao))).ToList();
            }
        }


        public BD.Agenda Abrir(long id, UtilizadorAutenticado utilizador)
        {
            using (BD.Context ctx = new BD.Context())
            {
                BD.Agenda obj = ctx.Agenda.FirstOrDefault(a => a.AgendaID == id);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exceptions.SemPermissao();

                return obj;
            }
        }


        public void Inserir(BD.Agenda obj, UtilizadorAutenticado utilizador)
        {
            if (string.IsNullOrEmpty(obj.Designacao) || string.IsNullOrEmpty(obj.Telefone))
                throw new Exceptions.DadosIncorrectos();

            obj.CondominioID = utilizador.CondominioID.Value;
            obj.PaisID = new Regras.Condominio().Abrir(utilizador.CondominioID.Value).PaisID;

            if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (Lista(obj.Designacao, utilizador).Count() > 0)
                throw new Exceptions.DesignacaoRepetida();

            _base.Inserir(obj);
            Notificacao.Processa(obj.AgendaID, Notificacao.Evento.NovaAgenda, utilizador);
        }


        public void Actualizar(BD.Agenda obj, UtilizadorAutenticado utilizador)
        {
            if (string.IsNullOrEmpty(obj.Designacao) || string.IsNullOrEmpty(obj.Telefone))
                throw new Exceptions.DadosIncorrectos();

            obj.CondominioID = utilizador.CondominioID.Value;
            obj.PaisID = new Regras.Condominio().Abrir(utilizador.CondominioID.Value).PaisID;

            BD.Agenda original = _base.Abrir(obj.AgendaID);

            if (original == null)
                throw new Exceptions.DadosIncorrectos();

            if (!Permissoes(utilizador, original).Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            IEnumerable<BD.Agenda> lista = Lista(obj.Designacao, utilizador);
            if (lista.Count() > 0 && lista.Where(x => x.AgendaID == obj.AgendaID).Count() == 0)
                throw new Exceptions.DesignacaoRepetida();

            _base.Actualizar(obj);
        }


        public void Apagar(long id, UtilizadorAutenticado utilizador)
        {
            using (BD.Context ctx = new BD.Context())
            {
                Regras.BD.Agenda obj = _base.Abrir(id, ctx);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Apagar))
                    throw new Exceptions.SemPermissao();

                _base.Apagar(obj.AgendaID, ctx);
                ctx.SaveChanges();
            }
        }

        
        #region Permissoes


        public static List<Enum.Permissao> Permissoes(UtilizadorAutenticado utilizador)
        {
            if (utilizador.CondominioID.HasValue)
            {
                if (utilizador.Perfil == Regras.Enum.Perfil.CondoClub ||
                    utilizador.Perfil == Regras.Enum.Perfil.Empresa ||
                    utilizador.Perfil == Regras.Enum.Perfil.Síndico)
                {
                    return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar };
                }

                if (utilizador.Perfil == Regras.Enum.Perfil.Consulta ||
                    utilizador.Perfil == Regras.Enum.Perfil.Morador ||
                    utilizador.Perfil == Regras.Enum.Perfil.Portaria)
                {
                    return new List<Enum.Permissao>() { Enum.Permissao.Visualizar };
                }
            }

            return new List<Enum.Permissao>();
        }


        public static List<Enum.Permissao> Permissoes(UtilizadorAutenticado utilizador, BD.Agenda obj)
        {
            if (utilizador.CondominioID.HasValue && utilizador.CondominioID.Value == obj.CondominioID)
            {
                if (utilizador.Perfil == Regras.Enum.Perfil.CondoClub ||
                    utilizador.Perfil == Regras.Enum.Perfil.Empresa ||
                    utilizador.Perfil == Regras.Enum.Perfil.Síndico)
                {
                    return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar, Enum.Permissao.Apagar };
                }
                if (utilizador.Perfil == Regras.Enum.Perfil.Consulta ||
                    utilizador.Perfil == Regras.Enum.Perfil.Morador)
                {
                    return new List<Enum.Permissao>() { Enum.Permissao.Visualizar };
                }
            }
            
            return new List<Enum.Permissao>();
        }


        #endregion

    }
}