using System;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using System.Data.Objects;

namespace CondoClub.Regras {

    public enum PagamentoPermissao {
        Visualizar,
        Pagar,
        FinalizarPagamento,
        ImprimirBoleto,
        MarcarPago
    }

    public class Pagamento {


        #region constantes e configurações

        private int nrDiasOfertaCondominio = Convert.ToInt32(ConfigurationManager.AppSettings["NumeroDiasOfertaCondominio"]);
        private int nrDiasOfertaFornecedor = Convert.ToInt32(ConfigurationManager.AppSettings["NumeroDiasOfertaFornecedor"]);
        private int nrDiasNotificacaoAtrasoPagamento = Convert.ToInt32(ConfigurationManager.AppSettings["NumeroDiasNotificacaoAtrasoPagamento"]);
        private int nrDiasNotificacaoFinalAtrasoPagamento = Convert.ToInt32(ConfigurationManager.AppSettings["NumeroDiasNotificacaoFinalAtrasoPagamento"]);
        private int nrDiasInactivacaoAtrasoPagamento = Convert.ToInt32(ConfigurationManager.AppSettings["NumeroDiasInactivacaoAtrasoPagamento"]);
        private const int nrDiasAno = 365;
        private const int nrDiasMes = 30;


        private static string urlWebServiceCielo = ConfigurationManager.AppSettings["UrlWebServiceCielo"];
        private static string numeroRegistoCielo = ConfigurationManager.AppSettings["NumeroRegistoCielo"];
        private static string chaveRegistoCielo = ConfigurationManager.AppSettings["ChaveRegistoCielo"];

        private static string chaveRegistoBoleto = ConfigurationManager.AppSettings["ChaveRegistoBoleto"];

        private static readonly List<Enum.TipoCartaoCredito> _tiposCartoesAceites =
          new List<Enum.TipoCartaoCredito>(new[]{
                    Enum.TipoCartaoCredito.Visa,
                    Enum.TipoCartaoCredito.MasterCard,
                    Enum.TipoCartaoCredito.Diners,
                    Enum.TipoCartaoCredito.Amex
                });


        #endregion


        #region Lista


        public static IEnumerable<RegistoPagamento> Lista(string termoPesquisa, DateTime? dataPagamentoInicio, DateTime? dataPagamentoFim, 
            bool? pago, int skip, int take, UtilizadorAutenticado utilizador) {

            using (BD.Context ctx = new BD.Context()) {

                if (utilizador.Perfil == Enum.Perfil.CondoClub) {
                    //visualiza pagamentos de todos os condominios, Fornecedores e publicidade
                    IEnumerable<RegistoPagamento> condominioPagamento = ctx.CondominioPagamento.
                            Include("Condominio").
                            Where(cp =>
                                (String.IsNullOrEmpty(termoPesquisa) || cp.Condominio.Nome.Contains(termoPesquisa)) &&
                                (dataPagamentoInicio == null || EntityFunctions.TruncateTime(cp.DataPagamento) >= dataPagamentoInicio) &&
                                (dataPagamentoFim == null || EntityFunctions.TruncateTime(cp.DataPagamento) <= dataPagamentoFim) &&
                                (pago == null || cp.Pago == pago)).
                            Select(cp => new RegistoPagamento {
                                ID = cp.CondominioPagamentoID,
                                OrigemInt = (int)Enum.OrigemPagamento.Condominio,
                                Inicio = cp.Inicio,
                                Fim = cp.Fim,
                                Valor = cp.Valor,
                                Pago = cp.Pago,
                                DataEmissao = cp.DataEmissao,
                                FormaPagamento = (Enum.FormaPagamento)cp.FormaPagamentoID,
                                ReferenciaPagamento = cp.ReferenciaPagamento,
                                IdTransacaoCielo = cp.IdTransacaoCielo,
                                IdTransacaoBoleto = cp.IdTransacaoBoleto,
                                DataPagamento = cp.DataPagamento,
                                UtilizadorPagamento = cp.Utilizador,
                                Condominio = cp.Condominio,
                                CondominioPagamento = cp
                            });

                    IEnumerable<RegistoPagamento> fornecedorPagamento = ctx.FornecedorPagamento.
                            Include("Fornecedor").
                            Where(fp =>
                                (String.IsNullOrEmpty(termoPesquisa) || fp.Fornecedor.Nome.Contains(termoPesquisa)) &&
                                (dataPagamentoInicio == null || EntityFunctions.TruncateTime(fp.DataPagamento) >= dataPagamentoInicio) &&
                                (dataPagamentoFim == null || EntityFunctions.TruncateTime(fp.DataPagamento) <= dataPagamentoFim) &&
                                (pago == null || fp.Pago == pago)).
                            Select(fp => new RegistoPagamento {
                                ID = fp.FornecedorPagamentoID,
                                OrigemInt = (int)Enum.OrigemPagamento.Fornecedor,
                                Inicio = fp.Inicio,
                                Fim = fp.Fim,
                                Valor = fp.Valor,
                                Pago = fp.Pago,
                                DataEmissao = fp.DataEmissao,
                                FormaPagamento = (Enum.FormaPagamento)fp.FormaPagamentoID,
                                ReferenciaPagamento = fp.ReferenciaPagamento,
                                IdTransacaoCielo = fp.IdTransacaoCielo,
                                IdTransacaoBoleto = fp.IdTransacaoBoleto,
                                DataPagamento = fp.DataPagamento,
                                UtilizadorPagamento = fp.Utilizador,
                                Fornecedor = fp.Fornecedor,
                                FornecedorPagamento = fp
                            });

                    IEnumerable<RegistoPagamento> publicidadePagamento = ctx.Publicidade.
                            Include("Fornecedor").
                            Where(p =>
                                p.Aprovado &&
                                (String.IsNullOrEmpty(termoPesquisa) || p.Fornecedor.Nome.Contains(termoPesquisa)) &&
                                (dataPagamentoInicio == null || EntityFunctions.TruncateTime(p.DataPagamento) >= dataPagamentoInicio) &&
                                (dataPagamentoFim == null || EntityFunctions.TruncateTime(p.DataPagamento) <= dataPagamentoFim) &&
                                (pago == null || p.Pago == pago)).
                            Select(p => new RegistoPagamento {
                                ID = p.PublicidadeID,
                                OrigemInt = (int)Enum.OrigemPagamento.Publicidade,
                                Inicio = p.Inicio,
                                Fim = p.Fim,
                                Valor = p.Valor,
                                Pago = p.Pago,
                                DataEmissao = p.DataEmissao,
                                FormaPagamento = (Enum.FormaPagamento)p.FormaPagamentoID,
                                ReferenciaPagamento = p.ReferenciaPagamento,
                                IdTransacaoCielo = p.IdTransacaoCielo,
                                IdTransacaoBoleto = p.IdTransacaoBoleto,
                                DataPagamento = p.DataPagamento,
                                UtilizadorPagamento = p.Utilizador,
                                Publicidade = p,
                                Fornecedor = p.Fornecedor
                            });

                    return condominioPagamento.
                            Union(fornecedorPagamento).
                            Union(publicidadePagamento).
                            OrderByDescending(m => m.DataEmissao).
                            Skip(skip).
                            Take(take).ToList();
                }

                if (utilizador.Perfil == Enum.Perfil.Fornecedor) {
                    //visualiza pagamentos do seu fornecedor e publicidade
                    IEnumerable<RegistoPagamento> fornecedorPagamento = ctx.FornecedorPagamento.
                            Include("Fornecedor").
                            Where(fp =>
                                fp.FornecedorID == utilizador.FornecedorID &&
                                (dataPagamentoInicio == null || EntityFunctions.TruncateTime(fp.DataPagamento) >= dataPagamentoInicio) &&
                                (dataPagamentoFim == null || EntityFunctions.TruncateTime(fp.DataPagamento) <= dataPagamentoFim) &&
                                (pago == null || fp.Pago == pago)).
                            Select(fp => new RegistoPagamento {
                                ID = fp.FornecedorPagamentoID,
                                OrigemInt = (int)Enum.OrigemPagamento.Fornecedor,
                                Inicio = fp.Inicio,
                                Fim = fp.Fim,
                                Valor = fp.Valor,
                                Pago = fp.Pago,
                                DataEmissao = fp.DataEmissao,
                                FormaPagamento = (Enum.FormaPagamento)fp.FormaPagamentoID,
                                ReferenciaPagamento = fp.ReferenciaPagamento,
                                IdTransacaoCielo = fp.IdTransacaoCielo,
                                IdTransacaoBoleto = fp.IdTransacaoBoleto,
                                DataPagamento = fp.DataPagamento,
                                UtilizadorPagamento = fp.Utilizador,
                                Fornecedor = fp.Fornecedor,
                                FornecedorPagamento = fp
                            });

                    IEnumerable<RegistoPagamento> publicidadePagamento = ctx.Publicidade.
                            Include("Fornecedor").
                            Where(p =>
                                p.Aprovado &&
                                p.FornecedorID == utilizador.FornecedorID &&
                                (dataPagamentoInicio == null || EntityFunctions.TruncateTime(p.DataPagamento) >= dataPagamentoInicio) &&
                                (dataPagamentoFim == null || EntityFunctions.TruncateTime(p.DataPagamento) <= dataPagamentoFim) &&
                                (pago == null || p.Pago == pago)).
                            Select(p => new RegistoPagamento {
                                ID = p.PublicidadeID,
                                OrigemInt = (int)Enum.OrigemPagamento.Publicidade,
                                Inicio = p.Inicio,
                                Fim = p.Fim,
                                Valor = p.Valor,
                                Pago = p.Pago,
                                DataEmissao = p.DataEmissao,
                                FormaPagamento = (Enum.FormaPagamento)p.FormaPagamentoID,
                                ReferenciaPagamento = p.ReferenciaPagamento,
                                IdTransacaoCielo = p.IdTransacaoCielo,
                                IdTransacaoBoleto = p.IdTransacaoBoleto,
                                DataPagamento = p.DataPagamento,
                                UtilizadorPagamento = p.Utilizador,
                                Publicidade = p,
                                Fornecedor = p.Fornecedor,
                            });


                    return fornecedorPagamento.
                                Union(publicidadePagamento).
                                OrderBy(m => m.DataEmissao).
                                Skip(skip).
                                Take(take).ToList();
                }

                if (utilizador.Perfil == Enum.Perfil.Empresa) {
                    //visualiza pagamentos de todos os seus condominios
                    return ctx.CondominioPagamento.
                            Include("Condominio").
                            Where(cp =>
                                cp.Condominio.EmpresaID == utilizador.EmpresaID &&
                                (String.IsNullOrEmpty(termoPesquisa) || cp.Condominio.Nome.Contains(termoPesquisa)) &&
                                (dataPagamentoInicio == null || EntityFunctions.TruncateTime(cp.DataPagamento) >= dataPagamentoInicio) &&
                                (dataPagamentoFim == null || EntityFunctions.TruncateTime(cp.DataPagamento) <= dataPagamentoFim) &&
                                (pago == null || cp.Pago == pago)).
                            Select(cp => new RegistoPagamento {
                                ID = cp.CondominioPagamentoID,
                                OrigemInt = (int)Enum.OrigemPagamento.Condominio,
                                Inicio = cp.Inicio,
                                Fim = cp.Fim,
                                Valor = cp.Valor,
                                Pago = cp.Pago,
                                DataEmissao = cp.DataEmissao,
                                FormaPagamento = (Enum.FormaPagamento)cp.FormaPagamentoID,
                                ReferenciaPagamento = cp.ReferenciaPagamento,
                                IdTransacaoCielo = cp.IdTransacaoCielo,
                                IdTransacaoBoleto = cp.IdTransacaoBoleto,
                                DataPagamento = cp.DataPagamento,
                                UtilizadorPagamento = cp.Utilizador,
                                Condominio = cp.Condominio,
                                CondominioPagamento = cp
                            }).
                            OrderBy(m => m.DataEmissao).
                            Skip(skip).
                            Take(take).ToList();
                }

                if (utilizador.Perfil == Enum.Perfil.Síndico) {
                    //visualiza pagamentos do seu condominio
                    return ctx.CondominioPagamento.
                            Include("Condominio").
                            Where(cp =>
                                cp.CondominioID == utilizador.CondominioID &&
                                (dataPagamentoInicio == null || EntityFunctions.TruncateTime(cp.DataPagamento) >= dataPagamentoInicio) &&
                                (dataPagamentoFim == null || EntityFunctions.TruncateTime(cp.DataPagamento) <= dataPagamentoFim) &&
                                (pago == null || cp.Pago == pago)).
                            Select(cp => new RegistoPagamento {
                                ID = cp.CondominioPagamentoID,
                                OrigemInt = (int)Enum.OrigemPagamento.Condominio,
                                Inicio = cp.Inicio,
                                Fim = cp.Fim,
                                Valor = cp.Valor,
                                Pago = cp.Pago,
                                DataEmissao = cp.DataEmissao,
                                FormaPagamento = (Enum.FormaPagamento)cp.FormaPagamentoID,
                                IdTransacaoCielo = cp.IdTransacaoCielo,
                                IdTransacaoBoleto = cp.IdTransacaoBoleto,
                                ReferenciaPagamento = cp.ReferenciaPagamento,
                                DataPagamento = cp.DataPagamento,
                                UtilizadorPagamento = cp.Utilizador,
                                Condominio = cp.Condominio,
                                CondominioPagamento = cp
                            }).
                            OrderBy(m => m.DataEmissao).
                            Skip(skip).
                            Take(take).ToList();

                }

                throw new CondoClub.Regras.Exceptions.SemPermissao();

            }

        }


        public static IEnumerable<Enum.TipoCartaoCredito> ListaTipoCartaoAceite() {
            return _tiposCartoesAceites;
        }


        #endregion


        #region Abrir


        public RegistoPagamento Abrir(Enum.OrigemPagamento origem, long id, UtilizadorAutenticado utilizador) {

            using (BD.Context ctx = new BD.Context()) {

                return Abrir(origem, id, utilizador, ctx);

            }


        }


        internal RegistoPagamento Abrir(Enum.OrigemPagamento origem, long id, UtilizadorAutenticado utilizador, BD.Context ctx) {

            RegistoPagamento ret = null;

            if (origem == Enum.OrigemPagamento.Condominio) {
                ret = ctx.CondominioPagamento.
                       Include("Condominio").
                       Where(cp => cp.CondominioPagamentoID == id).
                       Select(cp => new RegistoPagamento {
                           ID = cp.CondominioPagamentoID,
                           OrigemInt = (int)Enum.OrigemPagamento.Condominio,
                           Inicio = cp.Inicio,
                           Fim = cp.Fim,
                           Valor = cp.Valor,
                           Pago = cp.Pago,
                           DataEmissao = cp.DataEmissao,
                           FormaPagamento = (Enum.FormaPagamento)cp.FormaPagamentoID,
                           ReferenciaPagamento = cp.ReferenciaPagamento,
                           IdTransacaoCielo = cp.IdTransacaoCielo,
                           IdTransacaoBoleto = cp.IdTransacaoBoleto,
                           DataPagamento = cp.DataPagamento,
                           UtilizadorPagamento = cp.Utilizador,
                           Condominio = cp.Condominio,
                           CondominioPagamento = cp
                       }).FirstOrDefault();
            } else if (origem == Enum.OrigemPagamento.Fornecedor) {

                ret = ctx.FornecedorPagamento.
                        Include("Fornecedor").
                        Where(fp => fp.FornecedorPagamentoID == id).
                        Select(fp => new RegistoPagamento {
                            ID = fp.FornecedorPagamentoID,
                            OrigemInt = (int)Enum.OrigemPagamento.Fornecedor,
                            Inicio = fp.Inicio,
                            Fim = fp.Fim,
                            Valor = fp.Valor,
                            Pago = fp.Pago,
                            DataEmissao = fp.DataEmissao,
                            FormaPagamento = (Enum.FormaPagamento)fp.FormaPagamentoID,
                            ReferenciaPagamento = fp.ReferenciaPagamento,
                            IdTransacaoCielo = fp.IdTransacaoCielo,
                            IdTransacaoBoleto = fp.IdTransacaoBoleto,
                            DataPagamento = fp.DataPagamento,
                            UtilizadorPagamento = fp.Utilizador,
                            Fornecedor = fp.Fornecedor,
                            FornecedorPagamento = fp
                        }).FirstOrDefault();

            } else if (origem == Enum.OrigemPagamento.Publicidade) {

                ret = ctx.Publicidade.
                        Include("Fornecedor").
                        Where(p => p.PublicidadeID == id).
                        Select(p => new RegistoPagamento {
                            ID = p.PublicidadeID,
                            OrigemInt = (int)Enum.OrigemPagamento.Publicidade,
                            Inicio = p.Inicio,
                            Fim = p.Fim,
                            Valor = p.Valor,
                            Pago = p.Pago,
                            DataEmissao = p.DataEmissao,
                            FormaPagamento = (Enum.FormaPagamento)p.FormaPagamentoID,
                            ReferenciaPagamento = p.ReferenciaPagamento,
                            IdTransacaoCielo = p.IdTransacaoCielo,
                            IdTransacaoBoleto = p.IdTransacaoBoleto,
                            DataPagamento = p.DataPagamento,
                            UtilizadorPagamento = p.Utilizador,
                            Publicidade = p,
                            Fornecedor = p.Fornecedor,
                        }).FirstOrDefault();

            }

            if (!Permissoes(utilizador, ret).Contains(PagamentoPermissao.Visualizar))
                throw new Exceptions.SemPermissao();

            return ret;

        }


        #endregion


        #region operações


        public void MarcarPago(Enum.OrigemPagamento origem, long id, UtilizadorAutenticado utilizador) {

            using (BD.Context ctx = new BD.Context()) { 
            
                RegistoPagamento obj = Abrir(origem, id, utilizador, ctx);

                if(obj == null){
                    throw new Exceptions.DadosIncorrectos();
                }

                if(!Permissoes(utilizador, obj).Contains(PagamentoPermissao.MarcarPago)){
                    throw new Exceptions.SemPermissao();
                }

                if(origem == Enum.OrigemPagamento.Condominio){
                    BD.CondominioPagamento pag = ctx.CondominioPagamento.Where(cp => cp.CondominioPagamentoID == id).First();
                    pag.Pago = true;
                    pag.DataPagamento = DateTime.Now;
                    pag.UtilizadorPagamentoID = utilizador.ID;
                    ctx.SaveChanges();
                    return;
                }else if(origem == Enum.OrigemPagamento.Fornecedor){
                    BD.FornecedorPagamento pag = ctx.FornecedorPagamento.Where(fp => fp.FornecedorPagamentoID == id).First();
                    pag.Pago = true;
                    pag.DataPagamento = DateTime.Now;
                    pag.UtilizadorPagamentoID = utilizador.ID;
                    ctx.SaveChanges();
                    return;
                }else if(origem == Enum.OrigemPagamento.Publicidade){
                    BD.Publicidade pag = ctx.Publicidade.Where(p => p.PublicidadeID== id).First();
                    pag.Pago = true;
                    pag.DataPagamento = DateTime.Now;
                    pag.UtilizadorPagamentoID = utilizador.ID;
                    ctx.SaveChanges();
                    Notificacao.Processa(pag.PublicidadeID, Notificacao.Evento.NovaPublicidadeActiva, utilizador);
                    return;
                }else{
                    throw new Exceptions.DadosIncorrectos();
                }
            }

        }


        #endregion


        #region Integração Cielo


        public string ExecutarPagamento(Enum.OrigemPagamento origem, long id, Enum.TipoCartaoCredito tipoCartaoCredito, string urlRetornoFimPagamento, UtilizadorAutenticado utilizador) {

            string urlRedirect;

            using (BD.Context ctx = new BD.Context()) {

                RegistoPagamento pag = Abrir(origem, id, utilizador, ctx);

                if (!Permissoes(utilizador, pag).Contains(PagamentoPermissao.Pagar)) {
                    throw new Exceptions.SemPermissao();
                }

                string idTransacao;
                string numeroPedido = String.Concat(pag.Origem.ToString().Substring(0,1), pag.ID.ToString());
                ProxyPagamento.Cielo.TipoCartao tipoCartao = ConverterTipoCartao(tipoCartaoCredito);

                ProxyPagamento.Cielo proxyCielo = new ProxyPagamento.Cielo(urlWebServiceCielo, numeroRegistoCielo, chaveRegistoCielo);
                proxyCielo.CriarTransacao(pag.Valor.Value, tipoCartao, Resources.Pagamento.SoftDescriptor, urlRetornoFimPagamento, numeroPedido, out idTransacao, out urlRedirect);

                if (String.IsNullOrEmpty(idTransacao)) {
                    throw new Exceptions.IntegracaoCielo("A integração com a cielo não retornou um id de transacao");
                }

                if (String.IsNullOrEmpty(urlRedirect)) {
                    throw new Exceptions.IntegracaoCielo("A integração com a cielo não retornou um url para redireccionamento");
                }

                if (pag.Origem == Enum.OrigemPagamento.Condominio) {
                    pag.CondominioPagamento.IdTransacaoCielo = idTransacao;
                    ctx.ObjectStateManager.ChangeObjectState(pag.CondominioPagamento, System.Data.EntityState.Modified);
                } else if (origem == Enum.OrigemPagamento.Fornecedor) {
                    pag.FornecedorPagamento.IdTransacaoCielo = idTransacao;
                    ctx.ObjectStateManager.ChangeObjectState(pag.FornecedorPagamento, System.Data.EntityState.Modified);
                } else {
                    pag.Publicidade.IdTransacaoCielo = idTransacao;
                    ctx.ObjectStateManager.ChangeObjectState(pag.Publicidade, System.Data.EntityState.Modified);
                }

                ctx.SaveChanges();


            }
            return urlRedirect;

        }


        public RegistoPagamento FinalizarPagamento(Enum.OrigemPagamento origem, long id, UtilizadorAutenticado utilizador) {

            bool notificarPublicidadePaga = false, notificarPagamentoEfectuado = false;
            long idPagamento = 0;
            Notificacao.Evento? evento = null;

            using (BD.Context ctx = new BD.Context()) {

                RegistoPagamento pag = Abrir(origem, id, utilizador, ctx);

                if (!Permissoes(utilizador, pag).Contains(PagamentoPermissao.FinalizarPagamento)) {
                    throw new Exceptions.SemPermissao();
                }

                string idTransacao = pag.IdTransacaoCielo;
                if (String.IsNullOrEmpty(idTransacao)) {
                    throw new Exceptions.IntegracaoCielo("O pagamento não tem IdTransacaoCielo associado. Não é possivel finalizar o pagamento");
                }

                ProxyPagamento.Cielo proxyCielo = new ProxyPagamento.Cielo(urlWebServiceCielo, numeroRegistoCielo, chaveRegistoCielo);
                ProxyPagamento.Cielo.EstadoTransacao estado = proxyCielo.ConsultarEstadoTransacao(idTransacao);

                if (estado == ProxyPagamento.Cielo.EstadoTransacao.Capturada) {
                    if (pag.Origem == Enum.OrigemPagamento.Condominio) {
                        pag.CondominioPagamento.Pago = true;
                        pag.CondominioPagamento.DataPagamento = DateTime.Now;
                        pag.CondominioPagamento.UtilizadorPagamentoID = utilizador.ID;
                        ctx.ObjectStateManager.ChangeObjectState(pag.CondominioPagamento, System.Data.EntityState.Modified);
                        idPagamento = pag.CondominioPagamento.CondominioPagamentoID;
                        notificarPagamentoEfectuado = true;
                        evento = Notificacao.Evento.PagamentoCondominioEfectuado;
                    } else if (origem == Enum.OrigemPagamento.Fornecedor) {
                        pag.FornecedorPagamento.Pago = true;
                        pag.FornecedorPagamento.DataPagamento = DateTime.Now;
                        pag.FornecedorPagamento.UtilizadorPagamentoID = utilizador.ID;
                        ctx.ObjectStateManager.ChangeObjectState(pag.FornecedorPagamento, System.Data.EntityState.Modified);
                        idPagamento = pag.FornecedorPagamento.FornecedorPagamentoID;
                        notificarPagamentoEfectuado = true;
                        evento = Notificacao.Evento.PagamentoFornecedorEfectuado;
                    } else {
                        pag.Publicidade.Pago = true;
                        pag.Publicidade.DataPagamento = DateTime.Now;
                        pag.Publicidade.UtilizadorPagamentoID = utilizador.ID;
                        ctx.ObjectStateManager.ChangeObjectState(pag.Publicidade, System.Data.EntityState.Modified);
                        notificarPublicidadePaga = true;
                        idPagamento = pag.Publicidade.PublicidadeID;
                        notificarPagamentoEfectuado = true;
                        evento = Notificacao.Evento.PagamentoPublicidadeEfectuado;
                    }

                    ctx.SaveChanges();

                    if (notificarPublicidadePaga) {
                        Notificacao.Processa(pag.Publicidade.PublicidadeID, Notificacao.Evento.NovaPublicidadeActiva, utilizador);
                    }
                    if (notificarPagamentoEfectuado) {
                        Notificacao.Processa(idPagamento, evento.Value, utilizador);
                    }

                    //obrigar novamente a preencher o campo pago e data de pagamento e utilizador pagamento, independentemente do registo de origem do pagamento
                    pag = Abrir(origem, id, utilizador, ctx);
                }

                return pag;

            }


        }


        private ProxyPagamento.Cielo.TipoCartao ConverterTipoCartao(Enum.TipoCartaoCredito tipoCartao) {
            switch (tipoCartao) {
                case CondoClub.Regras.Enum.TipoCartaoCredito.Visa:
                    return ProxyPagamento.Cielo.TipoCartao.Visa;
                case CondoClub.Regras.Enum.TipoCartaoCredito.MasterCard:
                    return ProxyPagamento.Cielo.TipoCartao.MasterCard;
                case CondoClub.Regras.Enum.TipoCartaoCredito.Diners:
                    return ProxyPagamento.Cielo.TipoCartao.Diners;
                case CondoClub.Regras.Enum.TipoCartaoCredito.Discover:
                    return ProxyPagamento.Cielo.TipoCartao.Discover;
                case CondoClub.Regras.Enum.TipoCartaoCredito.Elo:
                    return ProxyPagamento.Cielo.TipoCartao.Elo;
                case CondoClub.Regras.Enum.TipoCartaoCredito.Amex:
                    return ProxyPagamento.Cielo.TipoCartao.Amex;
                case CondoClub.Regras.Enum.TipoCartaoCredito.Jcb:
                    return ProxyPagamento.Cielo.TipoCartao.Jcb;
                case CondoClub.Regras.Enum.TipoCartaoCredito.Aura:
                    return ProxyPagamento.Cielo.TipoCartao.Aura;
                default:
                    throw new Exception("Tipo de cartão inválido");
            }
        }


        //public string ConsultarEstadoTransacao(string idTransacao){
        //    ProxyPagamento.Cielo proxyCielo = new ProxyPagamento.Cielo(urlWebServiceCielo, numeroRegistoCielo, chaveRegistoCielo);
        //    ProxyPagamento.Cielo.EstadoTransacao estado = proxyCielo.ConsultarEstadoTransacao(idTransacao);
        //    return estado.ToString();
        //}


        #endregion


        #region Integração Boleto


        public void GerarBoleto(Enum.OrigemPagamento origem, long idPagamento, decimal valor, string contribuinte, string nome, string endereco, string localidade, string cidade, string codigoPostal, string estado, out string idTrasacao, out string referenciaPagamento) {
            idTrasacao = String.Empty;
            referenciaPagamento = String.Empty;

            string numeroDocumento = String.Concat(origem.ToString().Substring(0, 1), idPagamento.ToString());
            ProxyPagamento.Boleto pBoleto = new ProxyPagamento.Boleto(chaveRegistoBoleto);
            ProxyPagamento.BoletoResult boleto = pBoleto.GerarBoleto(valor, contribuinte, nome, endereco, localidade, cidade, codigoPostal, estado, numeroDocumento);
            idTrasacao = boleto.IdTransacao;
            referenciaPagamento = boleto.ReferenciaPagamento;
        }
        
        
        public string GerarHtmlBoleto(Enum.OrigemPagamento origem, long id, UtilizadorAutenticado utilizador) {

            RegistoPagamento pag = Abrir(origem, id, utilizador);

            if (!Permissoes(utilizador, pag).Contains(PagamentoPermissao.Visualizar)) {
                throw new Exceptions.SemPermissao();
            }

            string idTransacao = pag.IdTransacaoBoleto;
            if (String.IsNullOrEmpty(idTransacao)) {
                throw new Exceptions.IntegracaoCielo("O pagamento não tem IdTransacaoBoleto associado. Não é possivel gerar o Html.");
            }

            ProxyPagamento.Boleto proxyBoleto = new ProxyPagamento.Boleto(chaveRegistoBoleto);
            ProxyPagamento.BoletoResult res = proxyBoleto.ImprimirBoleto(idTransacao);

            return res.BoletoHtml;

        }


        #endregion


        #region Processos Pagamento


        public void ProcessaTodosPagamentos(UtilizadorAutenticado utilizador) {

            //****************************************************************************************
            //** Este método corre diariamente e gera os pagamentos para condominios e fornecedores **
            //****************************************************************************************

            using (BD.Context ctx = new BD.Context()) {

                //Condominios
                IEnumerable<BD.Condominio> condominios = ctx.Condominio.Where(o => o.Activo == true).ToList();
                foreach (var item in condominios)
                    ProcessaPagamentoCondominio(item, utilizador, ctx);

                //Fornecedores
                IEnumerable<BD.Fornecedor> fornecedores = ctx.Fornecedor.Where(o => o.Activo == true).ToList();
                foreach (var item in fornecedores)
                    ProcessaPagamentoFornecedor(item, utilizador, ctx);
            }

        }


        public void ProcessaAtrasoPagamentos(UtilizadorAutenticado utilizador) {

            //*********************************************************************
            //** Este método corre diariamente e controla o atraso de pagamentos **
            //*********************************************************************

            using (BD.Context ctx = new BD.Context()) {

                DateTime dataActual = DateTime.Now.Date;

                //Condominios
                IEnumerable<BD.CondominioPagamento> pagCondominios = ctx.CondominioPagamento.Where(o => o.Pago == false).ToList();
                foreach (var pagamento in pagCondominios) {
                    if (pagamento.Inicio.AddDays(nrDiasInactivacaoAtrasoPagamento) <= dataActual) {
                        //Desactivar registo se atraso já ultrapassou o limite e enviar notificação
                        pagamento.Condominio.Activo = false;
                        ctx.SaveChanges();
                        Notificacao.Processa(pagamento.CondominioPagamentoID, Notificacao.Evento.InactivacaoCondominio, utilizador);
                    } else if (pagamento.Inicio.AddDays(nrDiasNotificacaoFinalAtrasoPagamento) == dataActual) {
                        //Enviar notificação final de atraso
                        Notificacao.Processa(pagamento.CondominioPagamentoID, Notificacao.Evento.AvisoFinalPagamentoCondominio, utilizador);
                    } else if (pagamento.Inicio.AddDays(nrDiasNotificacaoAtrasoPagamento) == dataActual) {
                        //Enviar notificação de atraso
                        Notificacao.Processa(pagamento.CondominioPagamentoID, Notificacao.Evento.AtrasoPagamentoCondominio, utilizador);
                    }
                }

                //Fornecedores
                IEnumerable<BD.FornecedorPagamento> pagFornecedores = ctx.FornecedorPagamento.Where(o => o.Pago == false).ToList();
                foreach (var pagamento in pagFornecedores) {
                    if (pagamento.Inicio.AddDays(nrDiasInactivacaoAtrasoPagamento) <= dataActual) {
                        //Desactivar registo se atraso já ultrapassou o limite e enviar notificação
                        pagamento.Fornecedor.Activo = false;
                        ctx.SaveChanges();
                        Notificacao.Processa(pagamento.FornecedorPagamentoID, Notificacao.Evento.InactivacaoFornecedor, utilizador);
                    } else if (pagamento.Inicio.AddDays(nrDiasNotificacaoFinalAtrasoPagamento) == dataActual) {
                        //Enviar notificação final de atraso
                        Notificacao.Processa(pagamento.FornecedorPagamentoID, Notificacao.Evento.AvisoFinalPagamentoFornecedor, utilizador);
                    } else if (pagamento.Inicio.AddDays(nrDiasNotificacaoAtrasoPagamento) == dataActual) {
                        //Enviar notificação de atraso
                        Notificacao.Processa(pagamento.FornecedorPagamentoID, Notificacao.Evento.AtrasoPagamentoFornecedor, utilizador);
                    }
                }
            }
        }


        internal void ProcessaPagamentoCondominio(BD.Condominio condominio, UtilizadorAutenticado utilizador, BD.Context ctx) {

            if (condominio.CondominioPagamento.Count > 0) {
                DateTime dataUltimoPagamento = condominio.CondominioPagamento.Last().Fim.Date;

                if (dataUltimoPagamento < DateTime.Now.Date) {
                    //Adiciona novo registo de pagamento
                    AdicionaRegistoPagamentoCondominio(condominio, dataUltimoPagamento.AddDays(1), utilizador, ctx);
                }
            } else if (condominio.DataActivacao.Value.Date.AddDays(nrDiasOfertaCondominio) <= DateTime.Now.Date) {
                //Insere primeiro registo de pagamento
                AdicionaRegistoPagamentoCondominio(condominio,
                        condominio.DataActivacao.Value.Date.AddDays(nrDiasOfertaCondominio + 1), utilizador, ctx);
            }

        }


        internal void ProcessaPagamentoFornecedor(BD.Fornecedor fornecedor, UtilizadorAutenticado utilizador, BD.Context ctx) {

            if (fornecedor.FornecedorPagamento.Count > 0) {
                DateTime dataUltimoPagamento = fornecedor.FornecedorPagamento.Last().Fim.Date;

                if (dataUltimoPagamento < DateTime.Now.Date) {
                    //Adiciona novo registo de pagamento
                    AdicionaRegistoPagamentoFornecedor(fornecedor, dataUltimoPagamento.AddDays(1), utilizador, ctx);
                }
            } else if (fornecedor.DataActivacao.Value.Date.AddDays(nrDiasOfertaFornecedor) <= DateTime.Now.Date) {
                //Insere primeiro registo de pagamento
                AdicionaRegistoPagamentoFornecedor(fornecedor,
                    fornecedor.DataActivacao.Value.Date.AddDays(nrDiasOfertaFornecedor + 1), utilizador, ctx);
            }

        }


        internal void ProcessaPagamentoPublicidade(BD.Publicidade publicidade, UtilizadorAutenticado utilizador, BD.Context ctx) {

            //Actualiza dados de pagamento
            publicidade.Valor = CalculoPrecoPublicidade(publicidade, ctx);
            publicidade.Pago = false;
            publicidade.DataEmissao = DateTime.Now.Date;
            publicidade.FormaPagamentoID = publicidade.Fornecedor.FormaPagamentoID;

            if (publicidade.Valor > 0) {
                if (publicidade.FormaPagamentoID == (int)Enum.FormaPagamento.Boleto) {

                    BD.Fornecedor fornecedor = publicidade.Fornecedor;

                    string idTransacao, referenciaPagamento;
                    GerarBoleto(Enum.OrigemPagamento.Publicidade, publicidade.PublicidadeID, publicidade.Valor.Value, fornecedor.Contribuinte, fornecedor.Nome,
                                fornecedor.Endereco, fornecedor.Localidade, fornecedor.Cidade, fornecedor.CodigoPostal,
                                fornecedor.Estado, out idTransacao, out referenciaPagamento);
                    
                    publicidade.IdTransacaoBoleto = idTransacao;
                    publicidade.ReferenciaPagamento = referenciaPagamento;
                }
            } else {
                publicidade.Pago = true;
                publicidade.DataPagamento = DateTime.Now.Date;
                publicidade.UtilizadorPagamentoID = utilizador.ID;
            }

            ctx.ObjectStateManager.ChangeObjectState(publicidade, System.Data.EntityState.Modified);
            ctx.SaveChanges();

            //envia aviso de pagamento
            if (publicidade.Valor > 0)
                Notificacao.Processa(publicidade.PublicidadeID, Notificacao.Evento.AvisoPagamentoPublicidade, utilizador);

        }


        #region Funções Auxiliares


        private void AdicionaRegistoPagamentoCondominio(BD.Condominio condominio, DateTime inicio, UtilizadorAutenticado utilizador, BD.Context ctx) {

            BD.CondominioPagamento pagamento = new BD.CondominioPagamento();
            pagamento.CondominioID = condominio.CondominioID;
            pagamento.Inicio = inicio;
            if ((Enum.OpcaoPagamento)condominio.OpcaoPagamentoID == Enum.OpcaoPagamento.Anual)
                pagamento.Fim = pagamento.Inicio.AddDays(nrDiasAno);
            else
                pagamento.Fim = pagamento.Inicio.AddDays(nrDiasMes);
            pagamento.Valor = CalculoPrecoCondominio(condominio, ctx);
            pagamento.Pago = false;
            pagamento.DataEmissao = DateTime.Now.Date;
            pagamento.FormaPagamentoID = condominio.FormaPagamentoID;

            if (pagamento.Valor > 0) {
                if ((Enum.FormaPagamento)condominio.FormaPagamentoID == Enum.FormaPagamento.Boleto) {

                    string idTransacao, referenciaPagamento;
                    GerarBoleto(Enum.OrigemPagamento.Condominio, pagamento.CondominioPagamentoID, pagamento.Valor, condominio.Contribuinte, condominio.Nome,
                                condominio.Endereco, condominio.Localidade, condominio.Cidade, condominio.CodigoPostal,
                                condominio.Estado, out idTransacao, out referenciaPagamento);

                    pagamento.IdTransacaoBoleto = idTransacao;
                    pagamento.ReferenciaPagamento = referenciaPagamento;
                }
            } else {
                pagamento.Pago = true;
                pagamento.DataPagamento = DateTime.Now.Date;
                pagamento.UtilizadorPagamentoID = utilizador.ID;
            }

            ctx.CondominioPagamento.AddObject(pagamento);
            ctx.SaveChanges();

            //envia aviso de pagamento
            if (pagamento.Valor > 0)
                Notificacao.Processa(pagamento.CondominioPagamentoID, Notificacao.Evento.AvisoPagamentoCondominio, utilizador);

        }


        private void AdicionaRegistoPagamentoFornecedor(BD.Fornecedor fornecedor, DateTime inicio, UtilizadorAutenticado utilizador, BD.Context ctx) {

            BD.FornecedorPagamento pagamento = new BD.FornecedorPagamento();
            pagamento.FornecedorID = fornecedor.FornecedorID;
            pagamento.Inicio = inicio;
            if ((Enum.OpcaoPagamento)fornecedor.OpcaoPagamentoID == Enum.OpcaoPagamento.Anual)
                pagamento.Fim = pagamento.Inicio.AddDays(nrDiasAno);
            else
                pagamento.Fim = pagamento.Inicio.AddDays(nrDiasMes);
            pagamento.Valor = CalculoPrecoFornecedor(fornecedor, ctx);
            pagamento.Pago = false;
            pagamento.DataEmissao = DateTime.Now.Date;
            pagamento.FormaPagamentoID = fornecedor.FormaPagamentoID;

            if (pagamento.Valor > 0) {
                if ((Enum.FormaPagamento)fornecedor.FormaPagamentoID == Enum.FormaPagamento.Boleto) {

                    string idTransacao, referenciaPagamento;
                    GerarBoleto(Enum.OrigemPagamento.Fornecedor, pagamento.FornecedorPagamentoID, pagamento.Valor, fornecedor.Contribuinte, fornecedor.Nome,
                                fornecedor.Endereco, fornecedor.Localidade, fornecedor.Cidade, fornecedor.CodigoPostal,
                                fornecedor.Estado, out idTransacao, out referenciaPagamento);

                    pagamento.IdTransacaoBoleto = idTransacao;
                    pagamento.ReferenciaPagamento = referenciaPagamento;
                }
            } else {
                pagamento.Pago = true;
                pagamento.DataPagamento = DateTime.Now.Date;
                pagamento.UtilizadorPagamentoID = utilizador.ID;
            }

            ctx.FornecedorPagamento.AddObject(pagamento);
            ctx.SaveChanges();

            //envia aviso de pagamento
            if (pagamento.Valor > 0)
                Notificacao.Processa(pagamento.FornecedorPagamentoID, Notificacao.Evento.AvisoPagamentoFornecedor, utilizador);

        }


        #endregion


        #region Cálculo de Preço


        internal decimal CalculoPrecoCondominio(BD.Condominio condominio, BD.Context ctx) {

            IEnumerable<BD.PrecoCondominio> precos = ctx.PrecoCondominio
                .Where(o => o.PaisID == condominio.PaisID &&
                        o.OpcaoPagamentoID == condominio.OpcaoPagamentoID &&
                        o.ExtratoSocialID == condominio.ExtratoSocialID)
                .OrderBy(o => o.FraccoesAte);

            foreach (var preco in precos) {
                if (condominio.Fraccoes.Value <= preco.FraccoesAte)
                    return preco.Valor;
                    //return (preco.Valor * condominio.Fraccoes.Value);
            }

            throw new Exceptions.PrecoPorDefinir();
        }


        internal decimal CalculoPrecoFornecedor(BD.Fornecedor fornecedor, BD.Context ctx) {

            long fraccoesTotal = TotalFraccoesFornecedor(fornecedor.FornecedorID, ctx);

            IEnumerable<BD.PrecoFornecedor> precos = ctx.PrecoFornecedor
                .Where(o => o.PaisID == fornecedor.PaisID && o.OpcaoPagamentoID == fornecedor.OpcaoPagamentoID)
                .OrderBy(o => o.FraccoesAte);

            foreach (var preco in precos) {
                if (fraccoesTotal <= preco.FraccoesAte)
                    return preco.Valor;
            }

            throw new Exceptions.PrecoPorDefinir();
        }


        internal decimal CalculoPrecoPublicidade(long fornecedorID, int ZonaPublicidadeID, DateTime Inicio, DateTime Fim) {

            using (BD.Context ctx = new BD.Context()) {
                BD.Fornecedor fornecedor = ctx.Fornecedor.Where(f => f.FornecedorID == fornecedorID).FirstOrDefault();

                if (fornecedor == null) {
                    throw new Exceptions.DadosIncorrectos();
                }

                return CalculoPrecoPublicidade(fornecedor.PaisID, ZonaPublicidadeID, TotalFraccoesFornecedor(fornecedorID, ctx), Inicio, Fim, ctx);
            }
        }
        

        internal decimal CalculoPrecoPublicidade(BD.Publicidade publicidade, BD.Context ctx) {
            return CalculoPrecoPublicidade(publicidade.Fornecedor.PaisID, publicidade.ZonaPublicidadeID, publicidade.AlcanceFraccoes, publicidade.Inicio, publicidade.Fim, ctx);
        }


        internal decimal CalculoPrecoPublicidade(int PaisID, int ZonaPublicidadeID, long AlcanceFraccoes, DateTime Inicio, DateTime Fim , BD.Context ctx) {

            //Multiplica o número total de dias de publicidade pelo preço

            IEnumerable<BD.PrecoPublicidade> precos = ctx.PrecoPublicidade
                .Where(o => o.PaisID == PaisID && o.ZonaPublicidadeID == ZonaPublicidadeID)
                .OrderBy(o => o.FraccoesAte);

            foreach (var preco in precos) {
                if (AlcanceFraccoes <= preco.FraccoesAte)
                    return (preco.Valor * (Fim - Inicio.AddDays(-1)).Days);
            }

            throw new Exceptions.PrecoPorDefinir();
        }


        internal int TotalCondominiosFornecedor(long fornecedorID, BD.Context ctx) {
            //Total de condominios disponiveis para o fornecedor
            int? condominiosTotal = ctx.FornecedorAlcance.Where(o => o.FornecedorID == fornecedorID).Count();
            return condominiosTotal == null ? 0 : condominiosTotal.Value;
        }


        internal long TotalFraccoesFornecedor(long fornecedorID, BD.Context ctx) {
            //Total de fraccoes disponiveis para o fornecedor
            long? fraccoesTotal = ctx.FornecedorAlcance
                .Where(o => o.FornecedorID == fornecedorID)
                .Select(o => o.Condominio)
                .Sum(o => o.Fraccoes);

            return fraccoesTotal == null ? 0 : fraccoesTotal.Value;
        }


        #endregion


        #endregion


        #region Permissoes


        public static List<Enum.Permissao> Permissoes(UtilizadorAutenticado utilizador) {

            if (utilizador.Perfil == Regras.Enum.Perfil.CondoClub ||
                utilizador.Perfil == Regras.Enum.Perfil.Empresa ||
                utilizador.Perfil == Regras.Enum.Perfil.Síndico ||
                utilizador.Perfil == Regras.Enum.Perfil.Fornecedor) {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar };
            }


            return new List<Enum.Permissao>();

        }


        public static List<PagamentoPermissao> Permissoes(UtilizadorAutenticado utilizador, RegistoPagamento pagamento) {
            List<PagamentoPermissao> permissoes = new List<PagamentoPermissao>();

            //o condoclub pode visualizar todos os pagamentos
            if (utilizador.Perfil == Enum.Perfil.CondoClub) {
                permissoes.Add(PagamentoPermissao.Visualizar);
                if (!pagamento.Pago) {
                    permissoes.Add(PagamentoPermissao.MarcarPago);
                }
                return permissoes;
            }

            if (pagamento.Origem == Enum.OrigemPagamento.Condominio) {

                //a empresa pode visualizar os pagamentos dos seus condominios
                if (utilizador.Perfil == Enum.Perfil.Empresa && pagamento.Condominio.EmpresaID.HasValue
                    && pagamento.Condominio.EmpresaID.Value == utilizador.EmpresaID) {
                    permissoes.Add(PagamentoPermissao.Visualizar);
                    return permissoes;
                }


                //o síndico pode visualizar os pagamentos do seu condominio, e pagar os que ainda não estão pagos
                if (utilizador.Perfil == Enum.Perfil.Síndico && pagamento.Origem == Enum.OrigemPagamento.Condominio
                        && pagamento.Condominio.CondominioID == utilizador.CondominioID) {

                    permissoes.Add(PagamentoPermissao.Visualizar);

                    if (!pagamento.Pago) {
                        if (pagamento.FormaPagamento == Enum.FormaPagamento.CartaoCredito) {
                            permissoes.Add(PagamentoPermissao.Pagar);
                            permissoes.Add(PagamentoPermissao.FinalizarPagamento);
                        } else if (pagamento.FormaPagamento == Enum.FormaPagamento.Boleto) {
                            permissoes.Add(PagamentoPermissao.ImprimirBoleto);
                        }
                    }
                    return permissoes;
                }

            } else if (pagamento.Origem == Enum.OrigemPagamento.Fornecedor) {

                //o fornecedor pode visualizar os seus pagamentos do condoclub, e pagar os que ainda não estão pagos
                if (utilizador.Perfil == Enum.Perfil.Fornecedor && pagamento.Origem == Enum.OrigemPagamento.Fornecedor
                    && pagamento.Fornecedor.FornecedorID == utilizador.FornecedorID) {

                    permissoes.Add(PagamentoPermissao.Visualizar);

                    if (!pagamento.Pago) {
                        if (pagamento.FormaPagamento == Enum.FormaPagamento.CartaoCredito) {
                            permissoes.Add(PagamentoPermissao.Pagar);
                            permissoes.Add(PagamentoPermissao.FinalizarPagamento);
                        } else if (pagamento.FormaPagamento == Enum.FormaPagamento.Boleto) {
                            permissoes.Add(PagamentoPermissao.ImprimirBoleto);
                        }
                    }

                    return permissoes;
                }

            } else if (pagamento.Origem == Enum.OrigemPagamento.Publicidade) {

                //o fornecedor pode visualizar os seus pagamentos de publicidade, e pagar as que ainda não estão pagas
                if (utilizador.Perfil == Enum.Perfil.Fornecedor && pagamento.Origem == Enum.OrigemPagamento.Publicidade
                   && pagamento.Publicidade.FornecedorID == utilizador.FornecedorID) {

                    permissoes.Add(PagamentoPermissao.Visualizar);

                    if (!pagamento.Pago && pagamento.Publicidade.Aprovado) {
                        if (pagamento.FormaPagamento == Enum.FormaPagamento.CartaoCredito) {
                            permissoes.Add(PagamentoPermissao.Pagar);
                            permissoes.Add(PagamentoPermissao.FinalizarPagamento);
                        } else if (pagamento.FormaPagamento == Enum.FormaPagamento.Boleto) {
                            permissoes.Add(PagamentoPermissao.ImprimirBoleto);
                        }
                    }

                    return permissoes;

                }
            }

            return permissoes;
        }


        #endregion


    }

    public class RegistoPagamento {

        public long ID { get; set; }
        internal int OrigemInt { get; set; }
        public Enum.OrigemPagamento Origem { get { return (Enum.OrigemPagamento)OrigemInt; } }
        public string Designacao {
            get {
                if (Origem == Enum.OrigemPagamento.Condominio) {
                    return String.Format(Resources.Pagamento.PagamentoCondoclub, Condominio.Nome.Length > 30 ? Condominio.Nome.Substring(0, 30) : Condominio.Nome,
                                            Inicio.ToShortDateString(), Fim.ToShortDateString());
                }
                if (Origem == Enum.OrigemPagamento.Condominio || Origem == Enum.OrigemPagamento.Fornecedor) {
                    return String.Format(Resources.Pagamento.PagamentoCondoclub, Fornecedor.Nome.Length > 30 ? Fornecedor.Nome.Substring(0, 30) : Fornecedor.Nome,
                                            Inicio.ToShortDateString(), Fim.ToShortDateString());
                } else {
                    return String.Format(Resources.Pagamento.PagamentoPublicidade, Publicidade.Titulo,
                                            Publicidade.Inicio.ToShortDateString(), Publicidade.Fim.ToShortDateString());
                }
            }
        }
        public DateTime Inicio { get; set; }
        public DateTime Fim { get; set; }
        public decimal? Valor { get; set; }
        public bool Pago { get; set; }
        public DateTime? DataEmissao { get; set; }
        public Enum.FormaPagamento FormaPagamento { get; set; }
        public string ReferenciaPagamento { get; set; }
        public DateTime? DataPagamento { get; set; }
        public BD.Utilizador UtilizadorPagamento { get; set; }
        public string IdTransacaoCielo { get; set; }
        public string IdTransacaoBoleto { get; set; }

        public BD.Condominio Condominio { get; set; }
        public BD.CondominioPagamento CondominioPagamento { get; set; }
        public BD.Fornecedor Fornecedor { get; set; }
        public BD.FornecedorPagamento FornecedorPagamento { get; set; }
        internal BD.Publicidade Publicidade { get; set; }

    }

}
