using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CondoClub.Regras
{
    public class ArquivoDirectoria
    {

        private _Base<BD.ArquivoDirectoria> _base = new _Base<BD.ArquivoDirectoria>();


        #region Campos

        internal const string directorioRaiz = "RootDir_";

        private static Dictionary<string, List<string>> _directoriasDefault = new Dictionary<string, List<string>>()
        {
            { "Avisos", null },
            { "Comunicados", null}, 
            { "Atas de Reunião", null}, 
            { "Contratos", null}, 
            { "Financeiro", new List<string>(){ "Demonstrativos", "Notas Fiscais", "Cobranças", "Pagamentos", "Comprovantes", "Recibos" } },
            { "Certificados", null}, 
            { "Portaria", null}, 
            { "Memorial Descritivo", null}, 
            { "Plantas", null}, 
            { "Eventos", new List<string>(){ "Convites", "Fotos" } }
        };

        #endregion


        public IEnumerable<BD.ArquivoDirectoria> Lista(long? id, UtilizadorAutenticado utilizador)
        {
            if (!Permissoes(utilizador).Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context())
            {
                return ctx.ArquivoDirectoria.Where(ad => ad.CondominioID == utilizador.CondominioID.Value && 
                    (
                        (!id.HasValue && !ad.ArquivoDirectoriaPaiID.HasValue) ||
                        (id.HasValue && ad.ArquivoDirectoriaPaiID == id)
                    )).ToList();
            }
        }


        public IEnumerable<BD.ArquivoDirectoria> Lista(string nome, long? pai, UtilizadorAutenticado utilizador)
        {
            if (!Permissoes(utilizador).Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context())
            {
                return Lista(nome, pai, utilizador, ctx);
            }
        }


        internal IEnumerable<BD.ArquivoDirectoria> Lista(string nome, long? pai, UtilizadorAutenticado utilizador, BD.Context ctx)
        {
            if (!Permissoes(utilizador).Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            return ctx.ArquivoDirectoria.Where(ad => ad.CondominioID == utilizador.CondominioID.Value &&
                (nome == null || ad.Nome.Equals(nome)) &&
                (pai == null || ad.ArquivoDirectoriaPaiID == pai)
            ).ToList();
        }


        public BD.ArquivoDirectoria Abrir(long id, UtilizadorAutenticado utilizador)
        {
            if (!Permissoes(utilizador).Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context())
            {
                return Abrir(id, utilizador, ctx);
            }
        }


        internal BD.ArquivoDirectoria Abrir(long id, UtilizadorAutenticado utilizador, BD.Context ctx)
        {
            if (!Permissoes(utilizador).Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            return ctx.ArquivoDirectoria.Where(ad => ad.CondominioID == utilizador.CondominioID.Value &&
                ad.ArquivoDirectoriaID == id).Single();
        }


        public BD.ArquivoDirectoria Abrir(string nome, long? pai, UtilizadorAutenticado utilizador)
        {
            if (!Permissoes(utilizador).Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context())
            {
                return ctx.ArquivoDirectoria.Where(ad => ad.CondominioID == utilizador.CondominioID.Value &&
                    ad.Nome.Equals(nome) && ad.ArquivoDirectoriaPaiID == pai).Single();
            }
        }


        public BD.ArquivoDirectoria AbrirOuCriarDirectoriaBase(UtilizadorAutenticado utilizador)
        {
            if (!Permissoes(utilizador).Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using(BD.Context ctx = new BD.Context())
            {
                IQueryable<BD.ArquivoDirectoria> q = ctx.ArquivoDirectoria.Where(
                    ad => ad.CondominioID == utilizador.CondominioID.Value &&
                    !ad.ArquivoDirectoriaPaiID.HasValue
                );

                if (q.Count() > 0)
                    return q.Single();
                else
                {
                    BD.ArquivoDirectoria root = new BD.ArquivoDirectoria()
                    {
                        ArquivoDirectoriaPaiID = null,
                        CondominioID = utilizador.CondominioID.Value,
                        DataHora = DateTime.Now,
                        Nome = directorioRaiz + utilizador.CondominioID.Value,
                        UtilizadorID = utilizador.ID
                    };

                    _base.Inserir(root, ctx);
                    ctx.SaveChanges();

                    CriarDirectoriasDefault(root, utilizador, ctx);

                    return root;
                }
            }
        }


        public void Inserir(BD.ArquivoDirectoria obj, UtilizadorAutenticado utilizador)
        {
            obj.UtilizadorID = utilizador.ID;
            obj.CondominioID = utilizador.CondominioID.Value;

            if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            if (Lista(obj.Nome, obj.ArquivoDirectoriaPaiID, utilizador).Count() > 0)
                throw new Exceptions.DesignacaoRepetida();

            _base.Inserir(obj);
        }


        public void Actualizar(BD.ArquivoDirectoria obj, UtilizadorAutenticado utilizador)
        {
            obj.UtilizadorID = utilizador.ID;
            obj.CondominioID = utilizador.CondominioID.Value;

            if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            IEnumerable<BD.ArquivoDirectoria> lista = Lista(obj.Nome, obj.ArquivoDirectoriaPaiID, utilizador);
            if (lista.Count() > 0 && lista.Where(ad => ad.ArquivoDirectoriaID == obj.ArquivoDirectoriaID).Count() == 0)
                throw new Exceptions.DesignacaoRepetida();

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
            Regras.BD.ArquivoDirectoria obj = Abrir(id, utilizador, ctx);

            if (obj == null)
                throw new Exceptions.DadosIncorrectos();

            if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Apagar))
                throw new Exceptions.SemPermissao();

            IEnumerable<BD.ArquivoDirectoria> childObjs = Lista(null, obj.ArquivoDirectoriaID, utilizador, ctx);

            foreach (BD.ArquivoDirectoria childObj in childObjs)
            {
                // Apagar diretorias filhas (chamada recursiva)
                Apagar(childObj.ArquivoDirectoriaID, utilizador, ctx);
            }

            // Apagar ficheiros na directoria
            new Regras.ArquivoFicheiro().ApagarFicheiros(id, utilizador, ctx);

            // Apagar directoria
            _base.Apagar(id, ctx);
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


        public static List<Enum.Permissao> Permissoes(UtilizadorAutenticado utilizador, BD.ArquivoDirectoria obj)
        {
            if (utilizador.CondominioID.HasValue && utilizador.CondominioID.Value == obj.CondominioID)
            {
                if (utilizador.Perfil == Regras.Enum.Perfil.CondoClub ||
                    utilizador.Perfil == Regras.Enum.Perfil.Empresa ||
                    utilizador.Perfil == Regras.Enum.Perfil.Síndico)
                {
                    List<Enum.Permissao> permissoes = new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar };

                    // Permissão para apagar
                    if (obj.ArquivoDirectoriaPaiID.HasValue)
                        permissoes.Add(Enum.Permissao.Apagar);

                    return permissoes;
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


        #region Funções auxiliares
        

        private bool ObjectoValido(BD.ArquivoDirectoria obj)
        {
            return !string.IsNullOrEmpty(obj.Nome) && obj.CondominioID != 0 && obj.UtilizadorID != 0;
        }


        private void CriarDirectoriasDefault(BD.ArquivoDirectoria root, UtilizadorAutenticado utilizador, BD.Context ctx)
        {
            if (root == null || _directoriasDefault == null || _directoriasDefault.Count == 0)
                return;

            BD.ArquivoDirectoria bdDir;
            foreach (var directoria in _directoriasDefault)
            {
                bdDir = new BD.ArquivoDirectoria()
                {
                    CondominioID = utilizador.CondominioID.Value,
                    ArquivoDirectoriaPaiID = root.ArquivoDirectoriaID,
                    Nome = directoria.Key,
                    DataHora = DateTime.Now,
                    UtilizadorID = utilizador.ID
                };

                _base.Inserir(bdDir, ctx);
                ctx.SaveChanges();

                if (directoria.Value != null)
                {
                    foreach (string subDirectoria in directoria.Value)
                    {
                        _base.Inserir(
                            new BD.ArquivoDirectoria()
                            {
                                CondominioID = utilizador.CondominioID.Value,
                                ArquivoDirectoriaPaiID = bdDir.ArquivoDirectoriaID,
                                Nome = subDirectoria,
                                DataHora = DateTime.Now,
                                UtilizadorID = utilizador.ID
                            },
                            ctx
                        );
                    }
                    ctx.SaveChanges();
                }
            }
        }


        #endregion
    }
}
