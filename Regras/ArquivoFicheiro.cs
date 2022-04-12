using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CondoClub.Regras
{
    public class ArquivoFicheiro
    {

        private _Base<BD.ArquivoFicheiro> _base = new _Base<BD.ArquivoFicheiro>();

        public IEnumerable<BD.ArquivoFicheiro> Lista(long? arquivoDirectoriaID, UtilizadorAutenticado utilizador)
        {
            if (!Permissoes(utilizador).Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context())
            {
                return ctx.ArquivoFicheiro.Include("Ficheiro").Where(
                    af => af.CondominioID == utilizador.CondominioID.Value &&
                    (!arquivoDirectoriaID.HasValue || af.ArquivoDirectoriaID == arquivoDirectoriaID)
                ).ToList();
            }
        }


        public IEnumerable<BD.ArquivoFicheiro> Lista(string nome, long? arquivoDirectoriaID, UtilizadorAutenticado utilizador)
        {
            if (!Permissoes(utilizador).Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context())
            {
                return Lista(nome, arquivoDirectoriaID, utilizador, ctx);
            }
        }


        internal IEnumerable<BD.ArquivoFicheiro> Lista(string nome, long? arquivoDirectoriaID, UtilizadorAutenticado utilizador, 
            BD.Context ctx)
        {
            if (!Permissoes(utilizador).Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            return ctx.ArquivoFicheiro.Include("Ficheiro").Where(
                af => af.CondominioID == utilizador.CondominioID.Value &&
                (nome == null || af.Ficheiro.Nome.Equals(nome)) &&
                (!arquivoDirectoriaID.HasValue || af.ArquivoDirectoriaID == arquivoDirectoriaID)
            ).ToList();
        }


        public BD.ArquivoFicheiro Abrir(long id, UtilizadorAutenticado utilizador)
        {
            if (!Permissoes(utilizador).Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context())
            {
                return ctx.ArquivoFicheiro.Include("Ficheiro").Where(af => af.CondominioID == utilizador.CondominioID.Value &&
                    af.ArquivoFicheiroID == id).Single();
            }
        }


        public void Inserir(IEnumerable<BD.ArquivoFicheiro> objs, UtilizadorAutenticado utilizador)
        {
            if (!Permissoes(utilizador).Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context())
            {
                BD.Ficheiro aux;

                foreach (BD.ArquivoFicheiro obj in objs)
                {
                    obj.CondominioID = utilizador.CondominioID.Value;
                    obj.UtilizadorID = utilizador.ID;

                    if (!ObjectoValido(obj))
                        throw new Exceptions.DadosIncorrectos();

                    if (Lista(obj.Ficheiro.Nome, obj.ArquivoDirectoriaID, utilizador, ctx).Count() > 0)
                        throw new Exceptions.DesignacaoRepetida();

                    aux = obj.Ficheiro;
                    obj.Ficheiro = null;
                    
                    _base.Inserir(obj, ctx);

                    aux.Temporario = false;
                    new Regras.Ficheiro().Actualizar(aux, ctx);
                    ctx.SaveChanges();

                    obj.Ficheiro = aux;

                    Notificacao.Processa(obj.ArquivoFicheiroID, Notificacao.Evento.NovoArquivo, utilizador);
                }
            }
        }


        public void Actualizar(BD.ArquivoFicheiro obj, UtilizadorAutenticado utilizador)
        {
            if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();


            _base.Actualizar(obj);
        }


        public void Apagar(long id, UtilizadorAutenticado utilizador)
        {
            using (BD.Context ctx = new BD.Context())
            {
                Apagar(id, utilizador, ctx);
                ctx.SaveChanges();
            }
        }


        internal void Apagar(long id, UtilizadorAutenticado utilizador, BD.Context ctx)
        {
            Apagar(_base.Abrir(id, ctx), utilizador, ctx);
        }


        internal void Apagar(Regras.BD.ArquivoFicheiro obj, UtilizadorAutenticado utilizador, BD.Context ctx)
        {
            if (obj == null)
                throw new Exceptions.DadosIncorrectos();

            if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Apagar))
                throw new Exceptions.SemPermissao();

            new Regras.Ficheiro().Apagar(obj.FicheiroID, ctx);

            _base.Apagar(obj.ArquivoFicheiroID, ctx);
        }


        internal void ApagarFicheiros(long arquivoDirectoriaID, UtilizadorAutenticado utilizador, BD.Context ctx)
        {
            IEnumerable<BD.ArquivoFicheiro> objs = ctx.ArquivoFicheiro
                .Where(af => af.ArquivoDirectoriaID == arquivoDirectoriaID).ToList();

            foreach (BD.ArquivoFicheiro obj in objs)
                Apagar(obj, utilizador, ctx);
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

                if (utilizador.Perfil == Regras.Enum.Perfil.Morador ||
                    utilizador.Perfil == Regras.Enum.Perfil.Portaria ||
                    utilizador.Perfil == Regras.Enum.Perfil.Consulta)
                {
                    return new List<Enum.Permissao>() { Enum.Permissao.Visualizar };
                }
            }

            return new List<Enum.Permissao>();
        }


        public static List<Enum.Permissao> Permissoes(UtilizadorAutenticado utilizador, BD.ArquivoFicheiro obj)
        {
            if (utilizador.CondominioID.HasValue && utilizador.CondominioID.Value == obj.CondominioID)
            {
                if (utilizador.Perfil == Regras.Enum.Perfil.CondoClub ||
                    utilizador.Perfil == Regras.Enum.Perfil.Empresa ||
                    utilizador.Perfil == Regras.Enum.Perfil.Síndico)
                {
                    return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar, Enum.Permissao.Apagar };
                }

                if (utilizador.Perfil == Regras.Enum.Perfil.Morador ||
                    utilizador.Perfil == Regras.Enum.Perfil.Portaria ||
                    utilizador.Perfil == Regras.Enum.Perfil.Consulta)
                {
                    return new List<Enum.Permissao>() { Enum.Permissao.Visualizar };
                }
            }

            return new List<Enum.Permissao>();
        }

        #endregion


        private bool ObjectoValido(BD.ArquivoFicheiro obj)
        {
            return obj.ArquivoDirectoriaID.HasValue && obj.CondominioID != 0 && obj.FicheiroID != 0 && obj.UtilizadorID != 0;
        }
    }
}
