using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;
using CondoClub.Regras.Exceptions;

namespace CondoClub.Regras {

    public enum PublicidadePermissao {
        Visualizar,
        Gravar,
        Apagar,
        Aprovar,
        Duplicar
    }

    public class Publicidade  {

        private _Base<BD.Publicidade> _base = new _Base<BD.Publicidade>();

        #region seleccionar

        public static IEnumerable<BD.Publicidade> ListaPublicidadeMostrar(UtilizadorAutenticado utilizador, Enum.ZonaPublicidade zona, int take) {

            using (BD.Context ctx = new BD.Context()) {

                //As publicidades são mostradas de acordo com os fornecedores a que o utilizador tem acesso
                IEnumerable<long> fornecedores = Fornecedor.FornecedoresDisponiveis(utilizador, ctx);
                List<BD.Publicidade> listaOrdenada = new List<BD.Publicidade>();
                DateTime hoje = DateTime.Now.Date;

                IEnumerable<BD.Publicidade> lista = ctx.Publicidade
                    .Where(o =>
                        o.Aprovado == true && o.Pago == true &&
                        o.Inicio <= hoje && o.Fim >= hoje &&
                        o.ZonaPublicidadeID == (int)zona &&
                        fornecedores.Contains(o.FornecedorID))
                    .OrderBy(o => o.Inicio).ThenBy(o => o.PublicidadeID) //o mais antigo é sempre o primeiro a ser mostrado
                    .ToList();

                if (lista.Count() > 0) {
                    string cacheID = String.Concat("Pub_", utilizador.ID, "_", zona);
                    object publicidadeID = MemoryCache.Default.Get(cacheID);

                    if (publicidadeID != null) {
                        long id = (long)publicidadeID;
                        int pos = -1;

                        //Reordena a lista começando a partir do elemento a seguir ao id em cache (cria um circulo)
                        foreach (var item in lista) {
                            if (pos == -1) {
                                listaOrdenada.Add(item);
                                if (item.PublicidadeID == id)
                                    pos = 0;
                            } else {
                                listaOrdenada.Insert(pos, item);
                                pos++;
                            }
                        }

                        MemoryCache.Default.Remove(cacheID);
                    } else
                        listaOrdenada = (List<BD.Publicidade>)lista;

                    //Ultima publicidade mostrada ao utilizador fica em cache durante 24 horas
                    MemoryCache.Default.Add(cacheID, listaOrdenada.First().PublicidadeID, DateTimeOffset.Now.AddMinutes(1440));

                    listaOrdenada = (listaOrdenada.Count > take ? listaOrdenada.Take(take).ToList() : listaOrdenada);

                    //actualizar o numero de impressões das publicidades
                    IEnumerable<long> pubIds = listaOrdenada.Select(lo => lo.PublicidadeID);
                    IEnumerable<BD.Publicidade> pubs = ctx.Publicidade.Where(p => pubIds.Contains(p.PublicidadeID));
                    foreach (BD.Publicidade p in pubs) {
                        ctx.PublicidadeImpressao.AddObject(new BD.PublicidadeImpressao {
                            PublicidadeID = p.PublicidadeID,
                            CondominioID = utilizador.CondominioID.Value,
                            DataHora = DateTime.Now
                        });
                    }

                    ctx.SaveChanges();
                }

                return listaOrdenada;
            }

        }

        public static IEnumerable<BD.Publicidade> Lista(string termoPesquisa, bool? Aprovado, int skip, int take, UtilizadorAutenticado utilizador) {

            using (BD.Context ctx = new BD.Context()) {

                if (utilizador.Perfil == Enum.Perfil.CondoClub) {

                    return ctx.Publicidade.
                            Include("Fornecedor").
                            Include("ZonaPublicidade").
                            Where(p =>
                                (String.IsNullOrEmpty(termoPesquisa) ||
                                p.Fornecedor.Nome.Contains(termoPesquisa) ||
                                p.Titulo.Contains(termoPesquisa)) &&
                                (!Aprovado.HasValue || p.Aprovado == Aprovado.Value)).
                            OrderByDescending(p => p.DataCriacao).
                            Skip(skip).
                            Take(take).ToList();

                } else if (utilizador.Perfil == Enum.Perfil.Fornecedor) {
                    return ctx.Publicidade.
                            Include("Fornecedor").
                            Include("ZonaPublicidade").
                            Where(p =>
                                p.FornecedorID == utilizador.FornecedorID &&
                                (!Aprovado.HasValue || p.Aprovado == Aprovado.Value)).
                            OrderByDescending(p => p.DataCriacao).
                            Skip(skip).
                            Take(take).ToList();
                } else {
                    throw new SemPermissao();
                }

            }

        }

        public BD.Publicidade Abrir(long id, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {
                return Abrir(id, utilizador, ctx);
            }
        }

        internal BD.Publicidade Abrir(long id, UtilizadorAutenticado utilizador, BD.Context ctx) {

            BD.Publicidade obj = ctx.Publicidade.
                        Include("Fornecedor").
                        Include("ZonaPublicidade")
                        .FirstOrDefault(p => p.PublicidadeID == id);

            if (obj == null)
                throw new Exceptions.DadosIncorrectos();

            if (!Permissoes(utilizador, obj).Contains(Regras.PublicidadePermissao.Visualizar))
                throw new Exceptions.SemPermissao();

            return obj;

        }

        #endregion

        #region Inserir/Actualizar

        public void Inserir(BD.Publicidade publicidade, UtilizadorAutenticado utilizador) {

            if (!Permissoes(utilizador).Contains(Regras.Enum.Permissao.Gravar)) {
                throw new Exceptions.SemPermissao();
            }


            using (BD.Context ctx = new BD.Context()) {

                publicidade.FornecedorID = utilizador.FornecedorID.Value;

                Regras.Pagamento rPagamento = new Pagamento();

                //por defeito o raio de acção é de 20kms
                publicidade.RaioAccao = 20;

                publicidade.AlcanceCondominios = rPagamento.TotalCondominiosFornecedor(publicidade.FornecedorID, ctx);
                publicidade.AlcanceFraccoes = rPagamento.TotalFraccoesFornecedor(publicidade.FornecedorID, ctx);
                publicidade.DataCriacao = DateTime.Now;
                publicidade.UtilizadorCriacaoID = utilizador.ID;
                publicidade.Aprovado = false;

                if (!ObjectoValido(publicidade))
                    throw new Exceptions.DadosIncorrectos();

                _base.Inserir(publicidade, ctx);

                publicidade.Valor = rPagamento.CalculoPrecoPublicidade(publicidade, ctx);

                if (publicidade.ImagemID.HasValue) {
                    publicidade.Ficheiro.Temporario = false;
                }

                ctx.SaveChanges();
            }

            Notificacao.Processa(publicidade.PublicidadeID, Notificacao.Evento.NovaPublicidade, utilizador);
        }

        public void Actualizar(BD.Publicidade publicidade, UtilizadorAutenticado utilizador) {
            bool aprovado = false;
            using (BD.Context ctx = new BD.Context()) {

                BD.Publicidade PublicidadeOriginal = ctx.Publicidade.Where(r => r.PublicidadeID == publicidade.PublicidadeID).FirstOrDefault();
                if (PublicidadeOriginal == null) {
                    throw new DadosIncorrectos();
                }

                List<PublicidadePermissao> permissoes = Permissoes(utilizador, PublicidadeOriginal);
                if (!permissoes.Contains(PublicidadePermissao.Gravar)) {
                    throw new Exceptions.SemPermissao();
                }

                PublicidadeOriginal.Titulo = publicidade.Titulo;
                PublicidadeOriginal.Texto = publicidade.Texto;
                if (!PublicidadeOriginal.ImagemID.HasValue & publicidade.ImagemID.HasValue) {
                    PublicidadeOriginal.ImagemID = publicidade.ImagemID;
                    PublicidadeOriginal.Ficheiro.Temporario = false;
                } else if (PublicidadeOriginal.ImagemID.HasValue &&
                     (!publicidade.ImagemID.HasValue || (PublicidadeOriginal.ImagemID != publicidade.ImagemID))) {
                    //a imagem alterou
                    new Regras.Ficheiro().Apagar(PublicidadeOriginal.ImagemID.Value, ctx);
                    PublicidadeOriginal.ImagemID = publicidade.ImagemID;
                    PublicidadeOriginal.Ficheiro.Temporario = false;
                }
                PublicidadeOriginal.URL = publicidade.URL;
                
                //por defeito o raio de acção é de 20kms
                //if (PublicidadeOriginal.RaioAccao != publicidade.RaioAccao) {
                //    //o raio de acção foi alterado, recalcular o numero de condominios e de frações de alcance
                //    Regras.Pagamento rPagamento = new Pagamento();
                //    publicidade.AlcanceCondominios = rPagamento.TotalCondominiosFornecedor(publicidade.FornecedorID, ctx);
                //    publicidade.AlcanceFraccoes = rPagamento.TotalFraccoesFornecedor(publicidade.FornecedorID, ctx);
                //}
                PublicidadeOriginal.RaioAccao = 20;
                PublicidadeOriginal.Inicio = publicidade.Inicio;
                PublicidadeOriginal.Fim = publicidade.Fim;


                if (permissoes.Contains(PublicidadePermissao.Aprovar)) {
                    //se foi aprovado pela primeira vez
                    if ((!PublicidadeOriginal.Aprovado && publicidade.Aprovado) && !PublicidadeOriginal.DataAprovacao.HasValue) {
                        PublicidadeOriginal.DataAprovacao = DateTime.Now;
                        PublicidadeOriginal.UtilizadorAprovacaoID = utilizador.ID;
                        aprovado = true;
                    }
                    PublicidadeOriginal.Aprovado = publicidade.Aprovado;
                }

                if (!ObjectoValido(PublicidadeOriginal))
                    throw new Exceptions.DadosIncorrectos();

                Regras.Pagamento rPagamento = new Pagamento();
                PublicidadeOriginal.Valor = rPagamento.CalculoPrecoPublicidade(PublicidadeOriginal, ctx);

                if(aprovado){
                    new Regras.Pagamento().ProcessaPagamentoPublicidade(PublicidadeOriginal, utilizador, ctx);                
                }

                ctx.SaveChanges();

            }

        }

        public BD.Publicidade Duplicar(long id, UtilizadorAutenticado utilizador) {

            BD.Ficheiro novaImagem = null;
            BD.Publicidade publicidadeDuplicar = null;
            using (BD.Context ctx = new BD.Context()) {

                ctx.Publicidade.MergeOption = System.Data.Objects.MergeOption.NoTracking;

                publicidadeDuplicar = Abrir(id, utilizador, ctx);
                if (publicidadeDuplicar == null) {
                    throw new DadosIncorrectos();
                }

                List<PublicidadePermissao> permissoes = Permissoes(utilizador, publicidadeDuplicar);
                if (!permissoes.Contains(PublicidadePermissao.Duplicar)) {
                    throw new Exceptions.SemPermissao();
                }

                //inserir imagem como temporaria
                if (publicidadeDuplicar.ImagemID.HasValue) {
                    BD.Ficheiro imagemDuplicar = ctx.Ficheiro.First(f => f.FicheiroID == publicidadeDuplicar.ImagemID);
                    novaImagem = new BD.Ficheiro();
                    novaImagem.Nome = imagemDuplicar.Nome;
                    novaImagem.FicheiroConteudo = new BD.FicheiroConteudo { Conteudo = imagemDuplicar.FicheiroConteudo.Conteudo };
                    novaImagem.Extensao = imagemDuplicar.Extensao;
                    novaImagem.DataHora = DateTime.Now;
                    novaImagem.Temporario = true;
                    novaImagem.Tamanho = imagemDuplicar.Tamanho;
                    novaImagem.UtilizadorID = utilizador.ID;
                    ctx.Ficheiro.AddObject(novaImagem);
                }

                ctx.SaveChanges();
            }

            if (novaImagem != null) {
                publicidadeDuplicar.ImagemID = novaImagem.FicheiroID;
            }
            //limpar o id, para quando for gravado ser tratado como uma nova publicidade
            publicidadeDuplicar.PublicidadeID = 0;
            return publicidadeDuplicar;

        }

        #endregion

        #region Apagar

        public void Apagar(long id, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {
                Regras.BD.Publicidade obj = _base.Abrir(id, ctx);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(PublicidadePermissao.Apagar)) {
                    throw new Exceptions.SemPermissao();
                }

                if (obj.ImagemID.HasValue) {
                    new Regras.Ficheiro().Apagar(obj.ImagemID.Value, ctx);
                }

                _base.Apagar(obj.PublicidadeID, ctx);

                ctx.SaveChanges();
            }
        }

        #endregion

        #region preco da publicidade

        public static decimal PrecoPublicidade(long? publicidadeID, int ZonaPublicidadeID, DateTime Inicio, DateTime Fim, UtilizadorAutenticado Utilizador) {
            using (BD.Context ctx = new BD.Context()) {

                long fornecedorID = 0;

                if(Utilizador.Perfil == Enum.Perfil.Fornecedor){
                    fornecedorID = Utilizador.FornecedorID.Value;
                } else if (Utilizador.Perfil == Enum.Perfil.CondoClub && publicidadeID.HasValue) {
                    BD.Publicidade publicidade = ctx.Publicidade.Where(p => p.PublicidadeID == publicidadeID.Value).FirstOrDefault();
                    if (publicidade == null) {
                        throw new DadosIncorrectos();
                    }
                    fornecedorID = publicidade.FornecedorID;
                } else {
                    throw new SemPermissao();
                }

                Pagamento rPagamento = new Pagamento();
                return rPagamento.CalculoPrecoPublicidade(fornecedorID, ZonaPublicidadeID, Inicio, Fim);
            }
        }

        #endregion

        #region Registo de Impresssão e Visualização

        public void RegistaVisualizacao(long idPublicidade, UtilizadorAutenticado utilizador) {

            if (idPublicidade <= 0 || utilizador == null || !utilizador.CondominioID.HasValue) {
                throw new DadosIncorrectos();
            }

            using (BD.Context ctx = new BD.Context()) {

                ctx.PublicidadeVisualizacao.AddObject(new BD.PublicidadeVisualizacao {
                    PublicidadeID = idPublicidade,
                    CondominioID = utilizador.CondominioID.Value,
                    UtilizadorID = utilizador.ID,
                    DataHora = DateTime.Now
                });
                ctx.SaveChanges();

            }

        }

        public void RegistaImpressao(long idPublicidade, UtilizadorAutenticado utilizador) {

            if (idPublicidade <= 0 || utilizador == null || !utilizador.CondominioID.HasValue) {
                throw new DadosIncorrectos();
            }

            using (BD.Context ctx = new BD.Context()) {

                ctx.PublicidadeImpressao.AddObject(new BD.PublicidadeImpressao {
                    PublicidadeID = idPublicidade,
                    CondominioID = utilizador.CondominioID.Value,
                    DataHora = DateTime.Now
                });
                ctx.SaveChanges();

            }

        }

        #endregion

        #region Permissoes

        public static List<Enum.Permissao> Permissoes(UtilizadorAutenticado utilizador) {

            if (utilizador.Perfil == Regras.Enum.Perfil.CondoClub) {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar };
            }

            if (utilizador.Perfil == Regras.Enum.Perfil.Fornecedor) {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar };
            }


            return new List<Enum.Permissao>();

        }

        public static List<PublicidadePermissao> Permissoes(UtilizadorAutenticado utilizador, BD.Publicidade publicidade) {
            List<PublicidadePermissao> permissoes = new List<PublicidadePermissao>();

            if (utilizador.Perfil == Enum.Perfil.CondoClub) {
                permissoes.Add(PublicidadePermissao.Visualizar);
                permissoes.Add(PublicidadePermissao.Gravar);
                permissoes.Add(PublicidadePermissao.Aprovar);

                return permissoes;
            }

            if (utilizador.Perfil == Enum.Perfil.Fornecedor && publicidade.FornecedorID == utilizador.FornecedorID) {
                permissoes.Add(PublicidadePermissao.Visualizar);
                permissoes.Add(PublicidadePermissao.Duplicar);
                if (!publicidade.Aprovado) {
                    permissoes.Add(PublicidadePermissao.Gravar);
                    permissoes.Add(PublicidadePermissao.Apagar);
                }

                return permissoes;
            }

            //todos os utilizadores abrangidos pela publicidade podem visualizar


            return permissoes;
        }

        #endregion

        #region Métodos auxiliares

        private bool ObjectoValido(BD.Publicidade obj) {
            return obj.FornecedorID > 0 &&
                    !string.IsNullOrEmpty(obj.Titulo) &&
                    obj.RaioAccao > 0 &&
                    obj.Inicio.Date >= DateTime.Now.Date &&
                    obj.Inicio.Date <= obj.Fim.Date;
        }

        #endregion

    }

}
