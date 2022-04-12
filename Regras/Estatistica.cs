using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Objects;

namespace CondoClub.Regras
{
    public class Estatistica {


        public class Categoria {
            public string Designacao { get; set; }
            public string Grupo1 { get; set; }
            public string Grupo2 { get; set; }
            public DateTime Data { get; set; }
            public long Total { get; set; }
        }


        public IEnumerable<Estatistica.Categoria> Lista(
            DateTime inicio, DateTime fim, Enum.TipoEstatistica tipo, long? empresaID, UtilizadorAutenticado utilizador) {

            if (utilizador.EmpresaID != null)
                empresaID = utilizador.EmpresaID;

            fim = fim.AddDays(1);

            if (!Permissoes(utilizador, tipo).Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context()) {

                switch (tipo) {
                    case CondoClub.Regras.Enum.TipoEstatistica.NovoComunicado:
                        return NovoComunicado(inicio, fim, empresaID, ctx);
                    case CondoClub.Regras.Enum.TipoEstatistica.NovaMensagem:
                        return NovaMensagem(inicio, fim, empresaID, ctx);
                    case CondoClub.Regras.Enum.TipoEstatistica.NovoQuestionario:
                        return NovoQuestionario(inicio, fim, empresaID, ctx);
                    case CondoClub.Regras.Enum.TipoEstatistica.NovaReserva:
                        return NovaReserva(inicio, fim, empresaID, ctx);
                    case CondoClub.Regras.Enum.TipoEstatistica.NovoArquivo:
                        return NovoArquivo(inicio, fim, empresaID, ctx);
                    case CondoClub.Regras.Enum.TipoEstatistica.NovoMorador:
                        return NovoMorador(inicio, fim, empresaID, ctx);
                    case CondoClub.Regras.Enum.TipoEstatistica.NovoCondominio:
                        return NovoCondominio(inicio, fim, empresaID, ctx);
                    case CondoClub.Regras.Enum.TipoEstatistica.NovoFornecedor:
                        return NovoFornecedor(inicio, fim, empresaID, ctx);
                    case CondoClub.Regras.Enum.TipoEstatistica.NovaAdministradora:
                        return NovaAdministradora(inicio, fim, ctx);
                    case CondoClub.Regras.Enum.TipoEstatistica.NovaPublicidade:
                        return NovaPublicidade(inicio, fim, empresaID, ctx);
                    case CondoClub.Regras.Enum.TipoEstatistica.ImpressaoPublicidade:
                        return ImpressaoPublicidade(inicio, fim, empresaID, utilizador.FornecedorID, ctx);
                    case CondoClub.Regras.Enum.TipoEstatistica.CliquePublicidade:
                        return CliquePublicidade(inicio, fim, empresaID, utilizador.FornecedorID, ctx);
                    case CondoClub.Regras.Enum.TipoEstatistica.PagamentoPublicidade:
                        return PagamentoPublicidade(inicio, fim, ctx);
                    case CondoClub.Regras.Enum.TipoEstatistica.PagamentoFornecedor:
                        return PagamentoFornecedor(inicio, fim, ctx);
                    case CondoClub.Regras.Enum.TipoEstatistica.PagamentoCondominio:
                        return PagamentoCondominio(inicio, fim, empresaID, ctx);
                    case CondoClub.Regras.Enum.TipoEstatistica.Erro:
                        return Erro(inicio, fim, ctx);
                    default:
                        return null;
                }
                
            }
        }


        #region Geração de Dados


        private IEnumerable<Estatistica.Categoria> NovoComunicado(DateTime inicio, DateTime fim, long? empresaID, BD.Context ctx) {
            
            return ctx.Comunicado
                    .Where(o => o.DataHora >= inicio && o.DataHora <= fim && 
                        o.CondominioID != null && (empresaID == null || o.Condominio.EmpresaID == empresaID))
                    .GroupBy(o => new { Designacao = o.Condominio.Nome, 
                        Grupo1 = o.Condominio.Empresa.Nome, Data = EntityFunctions.TruncateTime(o.DataHora).Value })
                    .Select(o => new Categoria() {
                        Designacao = o.Key.Designacao,
                        Grupo1 = o.Key.Grupo1,
                        Data = o.Key.Data,
                        Total = o.Count()
                    })
                    .OrderByDescending(o => o.Data)
                    .ToList();

        }
        

        private IEnumerable<Estatistica.Categoria> NovaMensagem(DateTime inicio, DateTime fim, long? empresaID, BD.Context ctx){

            return ctx.Mensagem
                    .Where(o => o.DataHora >= inicio && o.DataHora <= fim &&
                        o.Utilizador.CondominioID != null && (empresaID == null || o.Utilizador.Condominio.EmpresaID == empresaID))
                    .GroupBy(o => new {
                        Designacao = o.Utilizador.Condominio.Nome,
                        Grupo1 = o.Utilizador.Condominio.Empresa.Nome,
                        Data = EntityFunctions.TruncateTime(o.DataHora).Value
                    })
                    .Select(o => new Categoria() {
                        Designacao = o.Key.Designacao,
                        Grupo1 = o.Key.Grupo1,
                        Data = o.Key.Data,
                        Total = o.Count()
                    })
                    .OrderByDescending(o => o.Data)
                    .ToList();

        }


        private IEnumerable<Estatistica.Categoria> NovoQuestionario(DateTime inicio, DateTime fim, long? empresaID, BD.Context ctx){

            return ctx.Questionario
                    .Where(o => o.Inicio >= inicio && o.Inicio <= fim &&
                        o.CondominioID != null && (empresaID == null || o.Condominio.EmpresaID == empresaID))
                    .GroupBy(o => new {
                        Designacao = o.Condominio.Nome,
                        Grupo1 = o.Condominio.Empresa.Nome,
                        Data = EntityFunctions.TruncateTime(o.Inicio).Value
                    })
                    .Select(o => new Categoria() {
                        Designacao = o.Key.Designacao,
                        Grupo1 = o.Key.Grupo1,
                        Data = o.Key.Data,
                        Total = o.Count()
                    })
                    .OrderByDescending(o => o.Data)
                    .ToList();

        }


        private IEnumerable<Estatistica.Categoria> NovaReserva(DateTime inicio, DateTime fim, long? empresaID, BD.Context ctx){

            return ctx.RecursoReserva
                    .Where(o => o.DataHoraInicio >= inicio && o.DataHoraInicio <= fim &&
                        o.Utilizador.CondominioID != null && (empresaID == null || o.Utilizador.Condominio.EmpresaID == empresaID) &&
                        o.Aprovado == true)
                    .GroupBy(o => new {
                        Designacao = o.Utilizador.Condominio.Nome,
                        Grupo1 = o.Recurso.Designacao,
                        Grupo2 = o.Utilizador.Nome,
                        Data = EntityFunctions.TruncateTime(o.DataHoraInicio).Value
                    })
                    .Select(o => new Categoria() {
                        Designacao = o.Key.Designacao,
                        Grupo1 = o.Key.Grupo1,
                        Grupo2 = o.Key.Grupo2,
                        Data = o.Key.Data,
                        Total = o.Sum(a => a.NumeroSlots)
                    })
                    .OrderByDescending(o => o.Data)
                    .ToList();

        }


        private IEnumerable<Estatistica.Categoria> NovoArquivo(DateTime inicio, DateTime fim, long? empresaID, BD.Context ctx){

            return ctx.ArquivoFicheiro
                    .Where(o => o.DataHora >= inicio && o.DataHora <= fim &&
                        o.CondominioID != null && (empresaID == null || o.Condominio.EmpresaID == empresaID))
                    .GroupBy(o => new {
                        Designacao = o.Condominio.Nome,
                        Grupo1 = o.Condominio.Empresa.Nome,
                        Data = EntityFunctions.TruncateTime(o.DataHora).Value
                    })
                    .Select(o => new Categoria() {
                        Designacao = o.Key.Designacao,
                        Grupo1 = o.Key.Grupo1,
                        Data = o.Key.Data,
                        Total = o.Count()
                    })
                    .OrderByDescending(o => o.Data)
                    .ToList();

        }


        private IEnumerable<Estatistica.Categoria> NovoMorador(DateTime inicio, DateTime fim, long? empresaID, BD.Context ctx){

            return ctx.Utilizador
                    .Where(o => o.DataCriacao >= inicio && o.DataCriacao <= fim &&
                        o.CondominioID != null && (empresaID == null || o.Condominio.EmpresaID == empresaID))
                    .GroupBy(o => new {
                        Designacao = o.Condominio.Nome,
                        Grupo1 = o.Condominio.Empresa.Nome,
                        Data = EntityFunctions.TruncateTime(o.DataCriacao).Value
                    })
                    .Select(o => new Categoria() {
                        Designacao = o.Key.Designacao,
                        Grupo1 = o.Key.Grupo1,
                        Data = o.Key.Data,
                        Total = o.Count()
                    })
                    .OrderByDescending(o => o.Data)
                    .ToList();

        }


        private IEnumerable<Estatistica.Categoria> NovoCondominio(DateTime inicio, DateTime fim, long? empresaID, BD.Context ctx){

            return ctx.Condominio
                    .Where(o => o.DataCriacao >= inicio && o.DataCriacao <= fim &&
                        (empresaID == null || o.EmpresaID == empresaID))
                    .GroupBy(o => new {
                        Designacao = o.Empresa.Nome,
                        Grupo1 = o.Nome,
                        Data = EntityFunctions.TruncateTime(o.DataCriacao).Value
                    })
                    .Select(o => new Categoria() {
                        Designacao = o.Key.Designacao,
                        Grupo1 = o.Key.Grupo1,
                        Data = o.Key.Data,
                        Total = o.Count()
                    })
                    .OrderByDescending(o => o.Data)
                    .ToList();

        }


        private IEnumerable<Estatistica.Categoria> NovoFornecedor(DateTime inicio, DateTime fim, long? empresaID, BD.Context ctx) {

            return ctx.FornecedorCategoria
                    .Where(o => o.Fornecedor.DataCriacao >= inicio && o.Fornecedor.DataCriacao <= fim && 
                        (empresaID == null || o.Fornecedor.FornecedorAlcance.Any(a => a.Condominio.EmpresaID == empresaID)))
                    .GroupBy(o => new {
                        Designacao = o.Categoria.Designacao,
                        Grupo1 = o.Fornecedor.Nome,
                        Data = EntityFunctions.TruncateTime(o.Fornecedor.DataCriacao).Value
                    })
                    .Select(o => new Categoria() {
                        Designacao = o.Key.Designacao,
                        Grupo1 = o.Key.Grupo1,
                        Data = o.Key.Data,
                        Total = o.Count()
                    })
                    .OrderByDescending(o => o.Data)
                    .ToList();

        }


        private IEnumerable<Estatistica.Categoria> NovaAdministradora(DateTime inicio, DateTime fim, BD.Context ctx){

            return ctx.Empresa
                    .Where(o => o.DataCriacao >= inicio && o.DataCriacao <= fim)
                    .GroupBy(o => new {
                        Designacao = o.Nome,
                        Data = EntityFunctions.TruncateTime(o.DataCriacao).Value
                    })
                    .Select(o => new Categoria() {
                        Designacao = o.Key.Designacao,
                        Data = o.Key.Data,
                        Total = o.Count()
                    })
                    .OrderByDescending(o => o.Data)
                    .ToList();

        }


        private IEnumerable<Estatistica.Categoria> NovaPublicidade(DateTime inicio, DateTime fim, long? empresaID, BD.Context ctx) {

            return ctx.Publicidade
                    .Where(o => o.DataCriacao >= inicio && o.DataCriacao <= fim &&
                        o.Aprovado == true && o.Pago == true &&
                        (empresaID == null || o.Fornecedor.FornecedorAlcance.Any(a => a.Condominio.EmpresaID == empresaID)))
                    .GroupBy(o => new {
                        Designacao = o.Fornecedor.Nome,
                        Grupo1 = o.ZonaPublicidade.Designacao,
                        Data = EntityFunctions.TruncateTime(o.DataCriacao).Value
                    })
                    .Select(o => new Categoria() {
                        Designacao = o.Key.Designacao,
                        Grupo1 = o.Key.Grupo1,
                        Data = o.Key.Data,
                        Total = o.Count()
                    })
                    .OrderByDescending(o => o.Data)
                    .ToList();

        }


        private IEnumerable<Estatistica.Categoria> ImpressaoPublicidade(DateTime inicio, DateTime fim, long? empresaID, long? fornecedorID, BD.Context ctx){

            return ctx.PublicidadeImpressao
                    .Where(o => o.DataHora >= inicio && o.DataHora <= fim &&
                            (empresaID == null || o.Condominio.EmpresaID == empresaID) &&
                            (fornecedorID == null || o.Publicidade.FornecedorID == fornecedorID))
                    .GroupBy(o => new {
                        Designacao = o.Publicidade.Titulo,
                        Grupo1 = o.Publicidade.Fornecedor.Nome,
                        Grupo2 = o.Condominio.Nome,
                        Data = EntityFunctions.TruncateTime(o.DataHora).Value
                    })
                    .Select(o => new Categoria() {
                        Designacao = o.Key.Designacao,
                        Grupo1 = o.Key.Grupo1,
                        Grupo2 = o.Key.Grupo2,
                        Data = o.Key.Data,
                        Total = o.Count()
                    })
                    .OrderByDescending(o => o.Data)
                    .ToList();

        }


        private IEnumerable<Estatistica.Categoria> CliquePublicidade(DateTime inicio, DateTime fim, long? empresaID, long? fornecedorID, BD.Context ctx) {

            return ctx.PublicidadeVisualizacao
                    .Where(o => o.DataHora >= inicio && o.DataHora <= fim &&
                            (empresaID == null || o.Condominio.EmpresaID == empresaID) &&
                            (fornecedorID == null || o.Publicidade.FornecedorID == fornecedorID))
                    .GroupBy(o => new {
                        Designacao = o.Publicidade.Titulo,
                        Grupo1 = o.Publicidade.Fornecedor.Nome,
                        Grupo2 = o.Condominio.Nome,
                        Data = EntityFunctions.TruncateTime(o.DataHora).Value
                    })
                    .Select(o => new Categoria() {
                        Designacao = o.Key.Designacao,
                        Grupo1 = o.Key.Grupo1,
                        Grupo2 = o.Key.Grupo2,
                        Data = o.Key.Data,
                        Total = o.Count()
                    })
                    .OrderByDescending(o => o.Data)
                    .ToList();

        }


        private IEnumerable<Estatistica.Categoria> PagamentoPublicidade(DateTime inicio, DateTime fim, BD.Context ctx) {

            return ctx.Publicidade
                    .Where(o => o.DataCriacao >= inicio && o.DataCriacao <= fim &&
                        o.Aprovado == true && o.Pago == true)
                    .GroupBy(o => new {
                        Designacao = o.Fornecedor.Nome,
                        Grupo1 = o.ZonaPublicidade.Designacao,
                        Data = EntityFunctions.TruncateTime(o.DataCriacao).Value
                    })
                    .Select(o => new Categoria() {
                        Designacao = o.Key.Designacao,
                        Grupo1 = o.Key.Grupo1,
                        Data = o.Key.Data,
                        Total = (long)o.Sum(a => a.Valor)
                    })
                    .OrderByDescending(o => o.Data)
                    .ToList();

        }


        private IEnumerable<Estatistica.Categoria> PagamentoFornecedor(DateTime inicio, DateTime fim, BD.Context ctx) {

            return ctx.FornecedorPagamento
                    .Where(o => o.Pago == true && o.DataPagamento >= inicio && o.DataPagamento <= fim)
                    .GroupBy(o => new {
                        Designacao = o.Fornecedor.FormaPagamento.Designacao,
                        Grupo1 = o.Fornecedor.OpcaoPagamento.Designacao,
                        Data = EntityFunctions.TruncateTime(o.DataPagamento).Value
                    })
                    .Select(o => new Categoria() {
                        Designacao = o.Key.Designacao,
                        Grupo1 = o.Key.Grupo1,
                        Data = o.Key.Data,
                        Total = (long)o.Sum(a => a.Valor)
                    })
                    .OrderByDescending(o => o.Data)
                    .ToList();

        }


        private IEnumerable<Estatistica.Categoria> PagamentoCondominio(DateTime inicio, DateTime fim, long? empresaID, BD.Context ctx) {

            return ctx.CondominioPagamento
                    .Where(o => o.Pago == true && o.DataPagamento >= inicio && o.DataPagamento <= fim)
                    .GroupBy(o => new {
                        Designacao = o.Condominio.FormaPagamento.Designacao,
                        Grupo1 = o.Condominio.OpcaoPagamento.Designacao,
                        Grupo2 = o.Condominio.Empresa.Nome,
                        Data = EntityFunctions.TruncateTime(o.DataPagamento).Value
                    })
                    .Select(o => new Categoria() {
                        Designacao = o.Key.Designacao,
                        Grupo1 = o.Key.Grupo1,
                        Grupo2 = o.Key.Grupo2,
                        Data = o.Key.Data,
                        Total = (long)o.Sum(a => a.Valor)
                    })
                    .OrderByDescending(o => o.Data)
                    .ToList();

        }


        private IEnumerable<Estatistica.Categoria> Erro(DateTime inicio, DateTime fim, BD.Context ctx){

            return ctx.LogErro
                    .Where(o => o.DataHora >= inicio && o.DataHora <= fim)
                    .GroupBy(o => new {
                        Designacao = o.Origem,
                        Data = EntityFunctions.TruncateTime(o.DataHora).Value
                    })
                    .Select(o => new Categoria() {
                        Designacao = o.Key.Designacao,
                        Data = o.Key.Data,
                        Total = o.Count()
                    })
                    .OrderByDescending(o => o.Data)
                    .ToList();

        }


        # endregion


        # region Permissões

        public static List<Enum.Permissao> Permissoes(UtilizadorAutenticado utilizador) {

            if (utilizador.Perfil == Enum.Perfil.CondoClub ||
                utilizador.Perfil == Enum.Perfil.Empresa ||
                utilizador.Perfil == Enum.Perfil.Fornecedor) {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar };
            }
            else
                return new List<Enum.Permissao>();
        }


        public static List<Enum.Permissao> Permissoes(UtilizadorAutenticado utilizador, Enum.TipoEstatistica tipo) {

            if (utilizador.Perfil == Enum.Perfil.CondoClub){
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar };
            }
            else if(utilizador.Perfil == Enum.Perfil.Empresa && (
                tipo == Enum.TipoEstatistica.NovoComunicado ||
                tipo  == Enum.TipoEstatistica.NovaReserva ||
                tipo == Enum.TipoEstatistica.NovoFornecedor ||
                tipo == Enum.TipoEstatistica.NovaPublicidade ||
                tipo == Enum.TipoEstatistica.ImpressaoPublicidade ||
                tipo == Enum.TipoEstatistica.CliquePublicidade)) {
                    return new List<Enum.Permissao>() { Enum.Permissao.Visualizar };
            }
            else if (utilizador.Perfil == Enum.Perfil.Fornecedor && (
                tipo == Enum.TipoEstatistica.ImpressaoPublicidade ||
                tipo == Enum.TipoEstatistica.CliquePublicidade)){
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar };
            }
            else
                return new List<Enum.Permissao>();
        }

        # endregion
    }

}