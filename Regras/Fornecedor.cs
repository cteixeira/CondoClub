using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Runtime.Caching;

namespace CondoClub.Regras {

    public enum FornecedorEstado {
        Activo,
        Inactivo,
        PorValidar
    }

    public class Fornecedor {

        private static _Base<BD.Fornecedor> _base = new _Base<BD.Fornecedor>();

        public enum Permissao {
            Visualizar,
            Gravar,
            Apagar,
            AdicionarClassificacao,
            EnviarMensagem,
            EnviarConvites,
            ActualizarPerfil
        }


        public class Categoria {
            public int CategoriaID { get; set; }
            public string Designacao { get; set; }
            public List<Categoria> SubCategorias { get; set; }
        }


        #region Pesquisa

        public IEnumerable<BD.Fornecedor> Lista() {
            return _base.Lista();
        }


        public IEnumerable<BD.Fornecedor> Pesquisa(long? fornecedorID, FornecedorEstado? estado, string termo, int skip,
            int take, UtilizadorAutenticado utilizador) {
            if (!Permissoes(utilizador).Contains(Fornecedor.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context()) {
                return ctx.Fornecedor
                    .Include("Pais")
                    .Include("FormaPagamento")
                    .Include("OpcaoPagamento")
                    .Include("FornecedorCategoria")
                    .Include("FornecedorCategoria.Categoria.Categoria2")
                    .Include("Utilizador")
                    .Where(f =>
                        (fornecedorID == null || f.FornecedorID == fornecedorID) &&
                        (!estado.HasValue ||
                            (estado == FornecedorEstado.Activo && f.Activo) ||
                            (estado == FornecedorEstado.Inactivo && !f.Activo && f.DataActivacao.HasValue) ||
                            (estado == FornecedorEstado.PorValidar && !f.Activo && !f.DataActivacao.HasValue)
                        ) &&
                        (string.IsNullOrEmpty(termo) || f.Nome.Contains(termo) || f.Contribuinte.Contains(termo) ||
                        f.Cidade.Contains(termo) || f.Estado.Contains(termo))
                    )
                    .OrderBy(e => e.Nome)
                    .Skip(skip)
                    .Take(take)
                    .ToList();
            }
        }


        public static List<Fornecedor.Categoria> CategoriasDisponiveis(UtilizadorAutenticado utilizador) {

            if (!Permissoes(utilizador).Contains(Fornecedor.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            //Verificar se já existe em cache
            /*object cacheLista = null;
            if (utilizador.Perfil == Enum.Perfil.CondoClub)
                cacheLista = MemoryCache.Default.Get("Categorias");
            else if (utilizador.Perfil == Enum.Perfil.Empresa)
                cacheLista = MemoryCache.Default.Get(String.Concat("CategoriasPorEmpresa_", utilizador.EmpresaID.ToString()));
            else
                cacheLista = MemoryCache.Default.Get(String.Concat("CategoriasPorCondominio_", utilizador.CondominioID.ToString()));
            
            if (cacheLista != null) 
                return (List<Fornecedor.Categoria>)cacheLista;*/

            using (BD.Context ctx = new BD.Context()) {

                IEnumerable<long> fornecedores = FornecedoresDisponiveis(utilizador, ctx);

                IEnumerable<BD.Categoria> categorias = ctx.FornecedorCategoria
                                                            .Where(o => fornecedores.Contains(o.FornecedorID))
                                                            .Select(o => o.Categoria)
                                                            .Distinct();

                //Constroi a árvore de categorias
                List<Categoria> lista = new List<Categoria>();
                foreach (var item in categorias) {

                    //*********************************************************
                    //** Apenas adiciona as categorias que têm categoria pai **
                    //*********************************************************
                    if (item.CategoriaPaiID != null) {
                        Categoria catPai = lista.FirstOrDefault(o => o.CategoriaID == item.CategoriaPaiID);
                        if (catPai == null) {
                            //Adiciona primeiro categoria pai se ainda não existir
                            lista.Add(new Categoria() {
                                CategoriaID = item.Categoria2.CategoriaID,
                                Designacao = item.Categoria2.Designacao,
                                SubCategorias = new List<Categoria>() { 
                                    new Categoria() { CategoriaID = item.CategoriaID, Designacao = item.Designacao } }
                            });
                        } else {
                            //Adiciona subcategoria dentro de categoria pai
                            if (!catPai.SubCategorias.Exists(o => o.CategoriaID == item.CategoriaID)) {
                                catPai.SubCategorias.Add(new Categoria() { CategoriaID = item.CategoriaID, Designacao = item.Designacao });
                            }
                        }
                    }
                    /*else{
                        //Subcategoria não tem categoria pai por isso é logo adicionada
                        if (!lista.Exists(o => o.CategoriaID == item.CategoriaID)) {
                            lista.Add(new Categoria() {
                                CategoriaID = item.CategoriaID,
                                Designacao = item.Designacao,
                                SubCategorias = new List<Categoria>()
                            });
                        }
                    }*/
                }

                //Adicionar a cache
                /*DateTimeOffset duracao = DateTimeOffset.Now.AddHours(1);
                if (utilizador.Perfil == Enum.Perfil.CondoClub)
                    MemoryCache.Default.Add("Categorias", lista, duracao);
                else if (utilizador.Perfil == Enum.Perfil.Empresa)
                    MemoryCache.Default.Add(String.Concat("CategoriasPorEmpresa_", utilizador.EmpresaID.ToString()), lista, duracao);
                else
                    MemoryCache.Default.Add(String.Concat("CategoriasPorCondominio_", utilizador.CondominioID.ToString()), lista, duracao);*/

                return lista;
            }

        }


        public static IEnumerable<BD.Fornecedor> PesquisaServico(string termoPesquisa, UtilizadorAutenticado utilizador, int skip, int take) {

            if (!Permissoes(utilizador).Contains(Fornecedor.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context()) {

                IEnumerable<long> fornecedores = FornecedoresDisponiveis(utilizador, ctx);

                //parte os termos de pesquisa em diversas palavras com mais do que dois caracteres
                //isto permite fazer pesquisa sem levar em conta a ordem de introdução dos termos de pesquisa
                termoPesquisa = termoPesquisa.ToLower();
                IEnumerable<string> termos = termoPesquisa.Split(' ').Where(o => o.Length > 2);

                //filtra os fornecedores disponiveis comparando cada termo de pesquisa com as keywords
                if (termos.Count() > 0) {
                    fornecedores = ctx.FornecedorKeyword
                                        .Where(o => fornecedores.Contains(o.FornecedorID) && termos.Any(t => o.Keyword.Contains(t)))
                                        .Select(o => o.FornecedorID)
                                        .Distinct()
                                        .ToList();
                }

                return ctx.Fornecedor
                    .Where(o => o.Activo == true && fornecedores.Contains(o.FornecedorID))
                    .OrderByDescending(o => o.ClassificacaoMedia).ThenBy(o => o.Nome)
                    .Skip(skip)
                    .Take(take)
                    .ToList();

            }
        }


        internal static IEnumerable<long> FornecedoresDisponiveis(UtilizadorAutenticado utilizador, BD.Context ctx) {
            if (utilizador.Perfil == Enum.Perfil.CondoClub)
                return ctx.FornecedorAlcance.Select(o => o.FornecedorID).Distinct();
            else if (utilizador.Perfil == Enum.Perfil.Empresa)
                return ctx.FornecedorAlcance.Where(o => o.EmpresaID == utilizador.EmpresaID).Select(o => o.FornecedorID).Distinct();
            else
                return ctx.FornecedorAlcance.Where(o => o.CondominioID == utilizador.CondominioID).Select(o => o.FornecedorID);
        }


        #endregion


        #region Configuração de Pesquisa


        public void DefineAlcanceFornecedor(long? fornecedorID, long? condominioID, UtilizadorAutenticado utilizador) {

            //******************************************************************************************************
            //** Este método corre diariamente e define os condominios que estão disponiveis para cada fornecedor **
            //******************************************************************************************************

            System.Globalization.NumberFormatInfo formatoNumerico = new System.Globalization.NumberFormatInfo();
            formatoNumerico.NumberDecimalSeparator = ".";

            using (BD.Context ctx = new BD.Context()) {

                IEnumerable<BD.Fornecedor> fornecedores = ctx.Fornecedor
                    .Where(o => (fornecedorID == null || o.FornecedorID == fornecedorID) && o.Activo == true);
                IEnumerable<BD.Condominio> condominios = ctx.Condominio
                    .Where(o => (condominioID == null || o.CondominioID == condominioID) && o.Activo == true);

                foreach (var fornecedor in fornecedores) {

                    foreach (var fornAlcance in fornecedor.FornecedorAlcance.ToList()) {
                        if (condominioID == null)
                            ctx.DeleteObject(fornAlcance);
                        else if (fornAlcance.CondominioID == condominioID)
                            ctx.DeleteObject(fornAlcance);
                    }

                    //cálculos georeferenciacao
                    foreach (var condominio in condominios) {

                        if (CalculaDistanciaKms(
                            fornecedor.Latitude, fornecedor.Longitude,
                            condominio.Latitude, condominio.Longitude) <= (double)fornecedor.RaioAccao) {

                            ctx.FornecedorAlcance.AddObject(new BD.FornecedorAlcance() {
                                FornecedorID = fornecedor.FornecedorID,
                                CondominioID = condominio.CondominioID,
                                EmpresaID = condominio.EmpresaID
                            });

                        }
                    }
                }

                ctx.SaveChanges();
            }

        }


        private double CalculaDistanciaKms(
            double latitudeOrigem, double longitudeOrigem,
            double latitudeDestino, double longitudeDestino) {

            double dLat = toRadian(latitudeDestino - latitudeOrigem);
            double dLon = toRadian(longitudeDestino - longitudeOrigem);
            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(toRadian(latitudeOrigem)) * Math.Cos(toRadian(latitudeDestino));
            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            return (6371 * c); //6371 = KM
        }


        private static double toRadian(double val) {
            return (Math.PI / 180) * val;
        }


        public void DefineKeywordsFornecedor(long? fornecedorID, UtilizadorAutenticado utilizador) {

            //*******************************************************************************************
            //** Este método corre diariamente e define as keywords de cada fornecedor para a pesquisa **
            //*******************************************************************************************

            using (BD.Context ctx = new BD.Context()) {

                IEnumerable<BD.Fornecedor> fornecedores = ctx.Fornecedor
                    .Where(o => (fornecedorID == null || o.FornecedorID == fornecedorID) && o.Activo == true);

                foreach (var fornecedor in fornecedores) {

                    foreach (var fornKeyword in fornecedor.FornecedorKeyword.ToList()) {
                        ctx.DeleteObject(fornKeyword);
                    }

                    List<String> listaKeywords = new List<string>();

                    //nome
                    AdicionaKeywords(fornecedor.Nome, listaKeywords);

                    //descrição
                    AdicionaKeywords(fornecedor.Descricao, listaKeywords);

                    //cidade e localidade
                    AdicionaKeywords(fornecedor.Cidade, listaKeywords);
                    AdicionaKeywords(fornecedor.Localidade, listaKeywords);

                    //categorias
                    foreach (var categoria in fornecedor.FornecedorCategoria.Select(o => o.Categoria.Designacao)) {
                        AdicionaKeywords(categoria, listaKeywords);
                    }

                    foreach (var keyword in listaKeywords) {
                        ctx.FornecedorKeyword.AddObject(new BD.FornecedorKeyword() {
                            FornecedorID = fornecedor.FornecedorID,
                            Keyword = keyword
                        });
                    }
                }

                ctx.SaveChanges();
            }

        }


        private void AdicionaKeywords(string texto, List<String> listaKeywords) {
            if (texto != null) {
                string[] keywords = texto.Split(' ');
                foreach (var keyword in keywords) {
                    string keywordAICI = keyword.ToLower().ReplaceAccents();
                    if (keyword.Length > 2 && !listaKeywords.Contains(keywordAICI))
                        listaKeywords.Add(keywordAICI);
                }
            }
        }


        #endregion


        #region Operações

        internal BD.Fornecedor Abrir(long id) {
            using (BD.Context ctx = new BD.Context()) {
                return ctx.Fornecedor.Include("FornecedorCategoria").FirstOrDefault(f => f.FornecedorID == id);
            }
        }


        public BD.Fornecedor Abrir(long id, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {
                BD.Fornecedor obj = ctx.Fornecedor
                    .Include("Pais")
                    .Include("FormaPagamento")
                    .Include("OpcaoPagamento")
                    .Include("FornecedorCategoria.Categoria")
                    .Include("FornecedorCategoria.Categoria.Categoria2")
                    .Include("Utilizador")
                    .FirstOrDefault(f => f.FornecedorID == id);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(Fornecedor.Permissao.Visualizar))
                    throw new Exceptions.SemPermissao();

                return obj;
            }
        }


        public BD.Fornecedor Abrir(string nome, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {

                BD.Fornecedor obj = ctx.Fornecedor
                                        .Include("FornecedorCategoria.Categoria")
                                        .Include("FornecedorClassificacao.Utilizador")
                                        .Include("Publicidade")
                                        .FirstOrDefault(o => o.Nome == nome);


                if (obj == null)
                    throw new Exceptions.FornecedorNaoExiste();

                if (!Permissoes(utilizador, obj).Contains(Fornecedor.Permissao.Visualizar))
                    throw new Exceptions.SemPermissao();

                return obj;
            }
        }


        private BD.Fornecedor Abrir(long id, BD.Context ctx) {
            //Esta função é apenas para uso interno porque não faz validações de segurança

            BD.Fornecedor obj = ctx.Fornecedor
                                    .Include("FornecedorCategoria.Categoria")
                                    .Include("FornecedorClassificacao.Utilizador")
                                    .FirstOrDefault(o => o.FornecedorID == id);

            if (obj == null)
                throw new Exceptions.FornecedorNaoExiste();

            return obj;
        }


        public BD.Fornecedor AbrirPorPublicidade(long idPublicidade, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {

                BD.Publicidade pub = ctx.Publicidade.Include("Fornecedor").FirstOrDefault(p => p.PublicidadeID == idPublicidade);

                BD.Fornecedor obj = null;

                if (pub != null) {
                    obj = pub.Fornecedor;
                }

                if (obj == null)
                    throw new Exceptions.FornecedorNaoExiste();

                if (!Permissoes(utilizador, obj).Contains(Fornecedor.Permissao.Visualizar))
                    throw new Exceptions.SemPermissao();

                return obj;
            }
        }


        public void Inserir(BD.Fornecedor obj, UtilizadorAutenticado utilizador) {
            if (!Permissoes(utilizador).Contains(Fornecedor.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            if (VerificaNomeRepetido(obj.FornecedorID, obj.Nome))
                throw new Exceptions.EmailRepetido();

            using (BD.Context ctx = new BD.Context()) {

                //verificar se tem utilizador associado para poder activar
                if (obj.Activo && (obj.Utilizador == null || obj.Utilizador.Count == 0)) {
                    throw new Exceptions.FornecedorSemUtilizador();
                }

                if (obj.Activo)
                    obj.DataActivacao = DateTime.Now;

                obj.DataCriacao = DateTime.Now;

                _base.Inserir(obj, ctx);

                if (obj.AvatarID.HasValue)
                    obj.Ficheiro.Temporario = false;

                ctx.SaveChanges();
            }

            if (obj.Activo) {
                DefineAlcanceFornecedor(obj.FornecedorID, null, utilizador);
                DefineKeywordsFornecedor(obj.FornecedorID, utilizador);
            }
        }


        public void Actualizar(BD.Fornecedor obj, UtilizadorAutenticado utilizador) {

            BD.Fornecedor original = _base.Abrir(obj.FornecedorID);

            if (original == null)
                throw new Exceptions.DadosIncorrectos();

            if (!Permissoes(utilizador, original).Contains(Fornecedor.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            if (VerificaNomeRepetido(obj.FornecedorID, obj.Nome))
                throw new Exceptions.NomeRepetido();

            obj.TotalComentarios = original.TotalComentarios;
            obj.ClassificacaoMedia = original.ClassificacaoMedia;

            using (BD.Context ctx = new BD.Context()) {

                if (!original.Activo && obj.Activo) {

                    //verificar se tem utilizador associado (e activo) para poder activar
                    if (obj.Activo && !ctx.Utilizador.Any(u => u.PerfilUtilizadorID == (int)Enum.Perfil.Fornecedor && u.FornecedorID == obj.FornecedorID && u.Activo)) {
                        throw new Exceptions.FornecedorSemUtilizador();
                    }

                    obj.DataActivacao = DateTime.Now;

                }

                //Apagar foto anterior
                if (original.AvatarID.HasValue && original.AvatarID.Value != obj.AvatarID.Value)
                    new Regras.Ficheiro().Apagar(original.AvatarID.Value, ctx);

                //Data de criação não é editavel
                obj.DataCriacao = original.DataCriacao;

                _base.Actualizar(obj, ctx);

                if (obj.AvatarID.HasValue)
                    obj.Ficheiro.Temporario = false;

                ctx.SaveChanges();

                if (!original.DataActivacao.HasValue && obj.Activo) {
                    try {
                        Notificacao.Processa(obj.FornecedorID, Notificacao.Evento.NovoFornecedorActivo, utilizador);
                        new Regras.Pagamento().ProcessaPagamentoFornecedor(obj, utilizador, ctx);
                    } catch { throw; }
                }
            }

            if (obj.Activo) {
                DefineAlcanceFornecedor(obj.FornecedorID, null, utilizador);
                DefineKeywordsFornecedor(obj.FornecedorID, utilizador);
            }
        }


        public void ActualizarPerfil(BD.Fornecedor obj, UtilizadorAutenticado utilizador) {
            BD.Fornecedor original = _base.Abrir(obj.FornecedorID);

            if (original == null)
                throw new Exceptions.DadosIncorrectos();

            if (!Permissoes(utilizador, original).Contains(Fornecedor.Permissao.ActualizarPerfil))
                throw new Exceptions.SemPermissao();

            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            if (VerificaNomeRepetido(obj.FornecedorID, obj.Nome))
                throw new Exceptions.NomeRepetido();

            SubreporCamposPerfilReadonly(obj);

            using (BD.Context ctx = new BD.Context()) {

                //Apagar foto anterior
                if (original.AvatarID.HasValue && original.AvatarID.Value != obj.AvatarID.Value)
                    new Regras.Ficheiro().Apagar(original.AvatarID.Value, ctx);

                //Data de criação não é editavel
                obj.DataCriacao = original.DataCriacao;

                _base.Actualizar(obj, ctx);

                if (obj.AvatarID.HasValue)
                    obj.Ficheiro.Temporario = false;

                ctx.SaveChanges();
            }

            DefineKeywordsFornecedor(obj.FornecedorID, utilizador);
        }


        public void Apagar(long id, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {
                Regras.BD.Fornecedor obj = _base.Abrir(id, ctx);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(Fornecedor.Permissao.Apagar))
                    throw new Exceptions.SemPermissao();

                if (obj.FornecedorClassificacao.Any() ||
                    obj.FornecedorPagamento.Any() || obj.Publicidade.Any())
                    throw new Exceptions.TemDependencias();

                if (obj.AvatarID.HasValue)
                    new Regras.Ficheiro().Apagar(obj.AvatarID.Value, ctx);

                foreach (var ut in ctx.Utilizador.Where(ut => ut.FornecedorID.HasValue && ut.FornecedorID.Value == id).ToList()) {
                    foreach (var msgDest in ut.MensagemDestinatario.ToList()) {
                        ctx.DeleteObject(msgDest);
                    }
                    foreach (var msg in ut.Mensagem.ToList()) {
                        ctx.DeleteObject(msg);
                    }
                    ctx.DeleteObject(ut);
                }

                //Apagar registos das tabelas associadas
                foreach (var alcance in obj.FornecedorAlcance.ToList())
                    ctx.DeleteObject(alcance);
                foreach (var categoria in obj.FornecedorCategoria.ToList())
                    ctx.DeleteObject(categoria);
                foreach (var keyword in obj.FornecedorKeyword.ToList())
                    ctx.DeleteObject(keyword);

                ctx.DeleteObject(obj);
                ctx.SaveChanges();
            }
        }


        public void EnviarMensagem(long fornecedorID, string mensagem, UtilizadorAutenticado utilizador) {

            if (String.IsNullOrEmpty(mensagem))
                throw new Exceptions.DadosIncorrectos();

            using (BD.Context ctx = new BD.Context()) {
                BD.Fornecedor fornecedor = Abrir(fornecedorID, ctx);

                if (!Permissoes(utilizador, fornecedor).Contains(Fornecedor.Permissao.EnviarMensagem))
                    throw new Exceptions.SemPermissao();

                BD.Utilizador fornecedorUtilizador = fornecedor.Utilizador.FirstOrDefault();

                if (fornecedorUtilizador == null)
                    throw new Exceptions.DadosIncorrectos();

                new Mensagem().InserirMensagemFornecedor(mensagem, fornecedorUtilizador.UtilizadorID, utilizador);
            }
        }


        public BD.Fornecedor InserirClassificacao(BD.FornecedorClassificacao obj, UtilizadorAutenticado utilizador) {

            if (obj.FornecedorID < 1 || obj.UtilizadorID != utilizador.ID ||
                obj.DataHora == null || (obj.Classificacao < 1))
                throw new Exceptions.DadosIncorrectos();

            using (BD.Context ctx = new BD.Context()) {
                BD.Fornecedor fornecedor = Abrir(obj.FornecedorID, ctx);

                if (!Permissoes(utilizador, fornecedor).Contains(Fornecedor.Permissao.AdicionarClassificacao))
                    throw new Exceptions.SemPermissao();

                ctx.FornecedorClassificacao.AddObject(obj);
                ctx.SaveChanges();

                CalcularClassificacao(fornecedor, ctx);
                return Abrir(obj.FornecedorID, ctx); //Preciso de voltar a abrir porcausa do utilizador
            }
        }


        public BD.Fornecedor ApagarClassificacao(long id, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {

                BD.FornecedorClassificacao obj = ctx.FornecedorClassificacao
                                                    .FirstOrDefault(o => o.FornecedorClassificacaoID == id);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!PermissoesClassificacao(utilizador, obj).Contains(Enum.Permissao.Apagar))
                    throw new Exceptions.SemPermissao();

                ctx.FornecedorClassificacao.DeleteObject(obj);
                ctx.SaveChanges();

                BD.Fornecedor fornecedor = Abrir(obj.FornecedorID, ctx);
                CalcularClassificacao(fornecedor, ctx);
                return fornecedor;
            }
        }


        internal void CalcularClassificacao(BD.Fornecedor fornecedor, BD.Context ctx) {
            fornecedor.TotalComentarios = fornecedor.FornecedorClassificacao.Count;
            fornecedor.ClassificacaoMedia =
                fornecedor.TotalComentarios == 0 ? (short)0 : (short)fornecedor.FornecedorClassificacao.Average(o => o.Classificacao);

            ctx.ObjectStateManager.ChangeObjectState(fornecedor, System.Data.EntityState.Modified);
            ctx.SaveChanges();
        }


        public string Nome(long id, UtilizadorAutenticado utilizador) {
            if (!utilizador.FornecedorID.HasValue || id != utilizador.FornecedorID.Value)
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context()) {
                return ctx.Fornecedor.FirstOrDefault(f => f.FornecedorID == id).Nome;
            }
        }

        #endregion


        #region Registo

        public void Registar(BD.Fornecedor obj) {
            obj.RaioAccao = 20;

            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            using (BD.Context ctx = new BD.Context()) {
                if (obj.FornecedorID == 0) {
                    if (ctx.Fornecedor.Where(e => e.Nome.Equals(obj.Nome)).ToList().Count > 0)
                        throw new Exceptions.DesignacaoRepetida();

                    obj.DataCriacao = DateTime.Now;

                    _base.Inserir(obj, ctx);

                    if (obj.AvatarID.HasValue)
                        obj.Ficheiro.Temporario = false;

                } else {
                    BD.Fornecedor original = Abrir(obj.FornecedorID, ctx);
                    //actualizar os campos
                    original.Nome = obj.Nome;
                    original.Contribuinte = obj.Contribuinte;
                    original.OpcaoPagamentoID = obj.OpcaoPagamentoID;
                    original.FormaPagamentoID = obj.FormaPagamentoID;

                    //Apagar foto anterior
                    if (original.AvatarID.HasValue && original.AvatarID.Value != obj.AvatarID.Value)
                        new Regras.Ficheiro().Apagar(original.AvatarID.Value, ctx);
                    
                    original.AvatarID = obj.AvatarID;
                    if (original.AvatarID.HasValue) {
                        original.Ficheiro.Temporario = false;
                    }
                    original.Descricao = obj.Descricao;
                    original.Telefone = obj.Telefone;
                    original.Email = obj.Email;
                    original.URL = obj.URL;
                    original.Endereco = obj.Endereco;
                    original.Localidade = obj.Localidade;
                    original.Cidade = obj.Cidade;
                    original.Estado = obj.Estado;
                    original.PaisID = obj.PaisID;
                    original.Latitude = obj.Latitude;
                    original.Longitude = obj.Longitude;
                    original.RaioAccao = obj.RaioAccao;
                    //actualizar as categorias
                    original.FornecedorCategoria.ToList().ForEach(ctx.FornecedorCategoria.DeleteObject);
                    foreach (BD.FornecedorCategoria fc in obj.FornecedorCategoria) {
                        original.FornecedorCategoria.Add(new BD.FornecedorCategoria { CategoriaID = fc.CategoriaID });
                    }
                }

                ctx.SaveChanges();

            }

            DefineAlcanceFornecedor(obj.FornecedorID, null, null);
            DefineKeywordsFornecedor(obj.FornecedorID, null);
        }


        public BD.Fornecedor RegistarRetroceder(long id) {
            Regras.BD.Fornecedor obj = Abrir(id);

            if (obj == null || obj.Activo)
                throw new Regras.Exceptions.DadosIncorrectos();

            return obj;
        }

        #endregion


        #region Categorias

        public BD.FornecedorCategoria FornecedorCategoriaAbrir(long id, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {
                BD.FornecedorCategoria obj = ctx.FornecedorCategoria.Include("Categoria")
                    .FirstOrDefault(fc => fc.FornecedorCategoriaID == id);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!PermissoesCategoria(utilizador, obj).Contains(Enum.Permissao.Visualizar))
                    throw new Exceptions.SemPermissao();

                return obj;
            }
        }


        public void FornecedorCategoriaActualizar(BD.FornecedorCategoria obj, UtilizadorAutenticado utilizador) {
            if (!PermissoesCategoria(utilizador, obj).Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context()) {
                if (VerificaCategoriaRepetida(obj, ctx))
                    throw new Exceptions.CategoriaRepetida();

                new _Base<BD.FornecedorCategoria>().Actualizar(obj, ctx);
                ctx.SaveChanges();

                obj.Categoria.Categoria1.Load();
            }

            DefineKeywordsFornecedor(obj.FornecedorID, utilizador);
        }


        public void FornecedorCategoriaInserir(BD.FornecedorCategoria obj, UtilizadorAutenticado utilizador) {
            if (!PermissoesCategoria(utilizador, obj).Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context()) {
                if (VerificaCategoriaRepetida(obj, ctx))
                    throw new Exceptions.CategoriaRepetida();

                new _Base<BD.FornecedorCategoria>().Inserir(obj, ctx);
                ctx.SaveChanges();

                obj.CategoriaReference.Load();
            }

            DefineKeywordsFornecedor(obj.FornecedorID, utilizador);
        }


        public void FornecedorCategoriaApagar(long id, UtilizadorAutenticado utilizador) {
            _Base<BD.FornecedorCategoria> fornecedorCategoria = new _Base<BD.FornecedorCategoria>();
            Regras.BD.FornecedorCategoria obj = null;

            using (BD.Context ctx = new BD.Context()) {
                obj = fornecedorCategoria.Abrir(id, ctx);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!PermissoesCategoria(utilizador, obj).Contains(Enum.Permissao.Apagar))
                    throw new Exceptions.SemPermissao();

                fornecedorCategoria.Apagar(obj.FornecedorCategoriaID, ctx);
                ctx.SaveChanges();
            }

            if (obj != null)
                DefineKeywordsFornecedor(obj.FornecedorID, utilizador);
        }


        public BD.Categoria AbrirCategoria(int id, UtilizadorAutenticado utilizador) {
            if (!Permissoes(utilizador).Contains(Fornecedor.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context()) {
                return ctx.Categoria.FirstOrDefault(c => c.CategoriaID == id);
            }
        }


        public List<BD.FornecedorClassificacao> ObterClassificacoes(long id, UtilizadorAutenticado utilizador) {
            if (!Permissoes(utilizador).Contains(Fornecedor.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context()) {
                return ctx.FornecedorClassificacao
                    .Include("Utilizador")
                    .Where(fc => fc.FornecedorID == id)
                    .OrderByDescending(fc => fc.DataHora)
                    .ToList();
            }
        }


        #endregion


        #region Métodos auxiliares

        private bool ObjectoValido(BD.Fornecedor obj) {
            return !string.IsNullOrEmpty(obj.Nome) && !string.IsNullOrEmpty(obj.Contribuinte) &&
                obj.OpcaoPagamentoID != 0 && obj.FormaPagamentoID != 0 && !string.IsNullOrEmpty(obj.Descricao) &&
                !string.IsNullOrEmpty(obj.Telefone) && !string.IsNullOrEmpty(obj.Email) &&
                !string.IsNullOrEmpty(obj.Endereco) && !string.IsNullOrEmpty(obj.Cidade) &&
                !string.IsNullOrEmpty(obj.CodigoPostal) && obj.PaisID != 0 && obj.RaioAccao != 0;
        }


        private bool VerificaNomeRepetido(long? id, string nome) {
            using (BD.Context ctx = new BD.Context()) {
                return ctx.Fornecedor.Any(f => (id == null || f.FornecedorID != id) && f.Nome == nome);
            }
        }


        private bool VerificaCategoriaRepetida(BD.FornecedorCategoria obj, BD.Context ctx) {
            return ctx.FornecedorCategoria.FirstOrDefault(fc =>
                (fc.FornecedorCategoriaID == 0 || fc.FornecedorCategoriaID != obj.FornecedorCategoriaID) &&
                fc.FornecedorID == obj.FornecedorID &&
                fc.CategoriaID == obj.CategoriaID
            ) != null;
        }


        private void SubreporCamposPerfilReadonly(BD.Fornecedor obj) {
            BD.Fornecedor objAnterior = _base.Abrir(obj.FornecedorID);
            obj.RaioAccao = objAnterior.RaioAccao;
            obj.Activo = objAnterior.Activo;
            obj.DataActivacao = objAnterior.DataActivacao;
            obj.Endereco = objAnterior.Endereco;
            obj.Localidade = objAnterior.Localidade;
            obj.Cidade = objAnterior.Cidade;
            obj.CodigoPostal = objAnterior.CodigoPostal;
            obj.Estado = objAnterior.Estado;
            obj.PaisID = objAnterior.PaisID;
            obj.Latitude = objAnterior.Latitude;
            obj.Longitude = objAnterior.Longitude;
        }

        #endregion


        #region Permissões


        public static List<Fornecedor.Permissao> Permissoes(UtilizadorAutenticado utilizador) {
            if (utilizador.Perfil == Enum.Perfil.CondoClub) {
                return new List<Fornecedor.Permissao>() { Fornecedor.Permissao.Visualizar, 
                    Fornecedor.Permissao.Gravar, Fornecedor.Permissao.EnviarConvites };
            }

            if (utilizador.Perfil == Enum.Perfil.Fornecedor) {
                return new List<Fornecedor.Permissao>() { Fornecedor.Permissao.Visualizar };
            }

            if (utilizador.Perfil == Enum.Perfil.CondoClub ||
                (utilizador.Perfil == Enum.Perfil.Empresa && utilizador.EmpresaID != null) ||
                (utilizador.Perfil == Enum.Perfil.Síndico && utilizador.CondominioID != null) ||
                (utilizador.Perfil == Enum.Perfil.Morador && utilizador.CondominioID != null) ||
                (utilizador.Perfil == Enum.Perfil.Consulta && utilizador.CondominioID != null) ||
                (utilizador.Perfil == Enum.Perfil.Portaria && utilizador.CondominioID != null)) {
                return new List<Fornecedor.Permissao>() { Fornecedor.Permissao.Visualizar };
            }

            return new List<Fornecedor.Permissao>();
        }


        public static List<Fornecedor.Permissao> Permissoes(UtilizadorAutenticado utilizador, BD.Fornecedor obj) {
            if (utilizador.Perfil == Enum.Perfil.CondoClub) {
                return new List<Fornecedor.Permissao>() { 
                    Fornecedor.Permissao.Visualizar, Fornecedor.Permissao.EnviarMensagem, 
                    Fornecedor.Permissao.AdicionarClassificacao, Fornecedor.Permissao.Gravar, 
                    Fornecedor.Permissao.Apagar
                };
            }

            if (utilizador.Perfil == Enum.Perfil.Fornecedor && utilizador.FornecedorID.HasValue &&
                utilizador.FornecedorID.Value == obj.FornecedorID) {
                return new List<Fornecedor.Permissao>() { Fornecedor.Permissao.Visualizar, Fornecedor.Permissao.ActualizarPerfil };
            }

            using (BD.Context ctx = new BD.Context()) {
                if (!FornecedoresDisponiveis(utilizador, ctx).Contains(obj.FornecedorID))
                    return new List<Fornecedor.Permissao>();
            }


            if (
                (utilizador.Perfil == Enum.Perfil.Empresa && utilizador.EmpresaID != null) ||
                (utilizador.Perfil == Enum.Perfil.Síndico && utilizador.CondominioID != null) ||
                (utilizador.Perfil == Enum.Perfil.Morador && utilizador.CondominioID != null)) {
                //Cada utilizador apenas pode classificar uma vez o fornecedor
                if (obj.FornecedorClassificacao.Any(o => o.UtilizadorID == utilizador.ID)) {
                    return new List<Fornecedor.Permissao>() { 
                        Fornecedor.Permissao.Visualizar, 
                        Fornecedor.Permissao.EnviarMensagem };
                } else {
                    return new List<Fornecedor.Permissao>() { 
                        Fornecedor.Permissao.Visualizar, 
                        Fornecedor.Permissao.EnviarMensagem, 
                        Fornecedor.Permissao.AdicionarClassificacao };
                }
            } else if (
                  (utilizador.Perfil == Enum.Perfil.Consulta && utilizador.CondominioID != null) ||
                  (utilizador.Perfil == Enum.Perfil.Portaria && utilizador.CondominioID != null)) {
                return new List<Fornecedor.Permissao>() { Fornecedor.Permissao.Visualizar };
            } else
                return new List<Fornecedor.Permissao>();
        }


        public static List<Fornecedor.Permissao> PermissoesBO(UtilizadorAutenticado utilizador) {
            if (utilizador.Perfil == Enum.Perfil.CondoClub) {
                return new List<Fornecedor.Permissao>() { Fornecedor.Permissao.Visualizar, 
                    Fornecedor.Permissao.Gravar, Fornecedor.Permissao.Apagar, Fornecedor.Permissao.EnviarConvites };
            }

            return new List<Fornecedor.Permissao>();
        }


        public static List<Enum.Permissao> PermissoesClassificacao(UtilizadorAutenticado utilizador, BD.FornecedorClassificacao obj) {

            if (utilizador.Perfil == Enum.Perfil.CondoClub ||
                (utilizador.Impersonating && utilizador.PerfilOriginal == Enum.Perfil.CondoClub)) {
                return new List<Enum.Permissao>() { 
                    Enum.Permissao.Visualizar, 
                    Enum.Permissao.Gravar, 
                    Enum.Permissao.Apagar };
            } else if (
                  utilizador.Perfil == Enum.Perfil.Empresa ||
                  utilizador.Perfil == Enum.Perfil.Síndico ||
                  utilizador.Perfil == Enum.Perfil.Morador) {

                if (obj.UtilizadorID == utilizador.ID)
                    return new List<Enum.Permissao>() { 
                        Enum.Permissao.Visualizar, 
                        Enum.Permissao.Gravar, 
                        Enum.Permissao.Apagar };
                else
                    return new List<Enum.Permissao>() { 
                        Enum.Permissao.Visualizar };
            } else if (
                  utilizador.Perfil == Enum.Perfil.Consulta ||
                  utilizador.Perfil == Enum.Perfil.Portaria) {
                return new List<Enum.Permissao>() { 
                    Enum.Permissao.Visualizar };
            } else
                return new List<Enum.Permissao>();
        }


        public static List<Enum.Permissao> PermissoesClassificacaoBO(UtilizadorAutenticado utilizador, long? fornecedorID) {
            if (utilizador.Perfil == Enum.Perfil.CondoClub) {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar, Enum.Permissao.Apagar };
            }

            if (utilizador.Perfil == Enum.Perfil.Fornecedor && utilizador.FornecedorID.HasValue &&
                utilizador.FornecedorID.Value == fornecedorID) {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar };
            }

            return new List<Enum.Permissao>();
        }


        public static List<Enum.Permissao> PermissoesCategoria(UtilizadorAutenticado utilizador, BD.FornecedorCategoria obj) {
            if (utilizador.Perfil == Enum.Perfil.CondoClub) {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar, Enum.Permissao.Apagar };
            }

            if (utilizador.Perfil == Enum.Perfil.Empresa && utilizador.FornecedorID.HasValue &&
                obj.FornecedorID == utilizador.FornecedorID.Value) {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar, Enum.Permissao.Apagar };
            }

            return new List<Enum.Permissao>();
        }

        #endregion
    }

    public class Categoria {


        public long ID { get; set; }
        public string Designacao { get; set; }


    }

}