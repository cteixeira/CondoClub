using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CondoClub.Regras {

    public class PrecoFornecedor {

        private _Base<BD.PrecoFornecedor> _base = new _Base<BD.PrecoFornecedor>();

        #region Seleccionar

        public IEnumerable<BD.PrecoFornecedor> Lista(int? paisID, int? opcaoPagamentoID, UtilizadorAutenticado utilizador) {

            if (!Permissoes(utilizador).Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context()) {
                return ctx.PrecoFornecedor.
                            Include("Pais").
                            Include("OpcaoPagamento").
                            Where(pf =>
                                (paisID == null || pf.PaisID == paisID) &&
                                (opcaoPagamentoID == null || pf.OpcaoPagamentoID == opcaoPagamentoID)).ToList();
            }

        }

        public BD.PrecoFornecedor Abrir(long id, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {
                BD.PrecoFornecedor obj = ctx.PrecoFornecedor.
                                            Include("Pais").
                                            Include("OpcaoPagamento").
                                            FirstOrDefault(pf => pf.PrecoFornecedorID == id);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exceptions.SemPermissao();

                return obj;
            }
        }

        #endregion

        #region Inserir/Actualizar

        public void Inserir(BD.PrecoFornecedor obj, UtilizadorAutenticado utilizador) {

            if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Gravar)) {
                throw new Exceptions.SemPermissao();
            }

            if (!ObjectoValido(obj)) {
                throw new Exceptions.DadosIncorrectos();
            }

            using (BD.Context ctx = new BD.Context()) {

                if (ctx.PrecoFornecedor.
                    Any(pc =>
                            pc.PaisID == obj.PaisID &&
                            pc.OpcaoPagamentoID == obj.OpcaoPagamentoID &&
                            pc.FraccoesAte == obj.FraccoesAte)) {

                    throw new Exceptions.PrecoRepetidoRange();

                }

                _base.Inserir(obj, ctx);
                ctx.SaveChanges();
            }

        }


        public void Actualizar(BD.PrecoFornecedor obj, UtilizadorAutenticado utilizador) {

            if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Gravar)) {
                throw new Exceptions.SemPermissao();
            }

            if (!ObjectoValido(obj)) {
                throw new Exceptions.DadosIncorrectos();
            }

            using (BD.Context ctx = new BD.Context()) {

                if (ctx.PrecoFornecedor.
                    Any(pc =>
                            pc.PrecoFornecedorID != obj.PrecoFornecedorID &&
                            pc.PaisID == obj.PaisID &&
                            pc.OpcaoPagamentoID == obj.OpcaoPagamentoID &&
                            pc.FraccoesAte == obj.FraccoesAte)) {

                    throw new Exceptions.PrecoRepetidoRange();

                }

                _base.Actualizar(obj, ctx);
                ctx.SaveChanges();
            }

        }

        #endregion

        #region Apagar

        public void Apagar(long id, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {
                Regras.BD.PrecoFornecedor obj = _base.Abrir(id, ctx);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Apagar))
                    throw new Exceptions.SemPermissao();

                _base.Apagar(obj.PrecoFornecedorID, ctx);
                ctx.SaveChanges();
            }
        }

        #endregion

        #region Permissoes

        public static List<Enum.Permissao> Permissoes(UtilizadorAutenticado utilizador) {

            if (utilizador.Perfil == Regras.Enum.Perfil.CondoClub) {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar };
            }

            return new List<Enum.Permissao>();
        }

        public static List<Enum.Permissao> Permissoes(UtilizadorAutenticado utilizador, BD.PrecoFornecedor obj) {
            if (utilizador.Perfil == Regras.Enum.Perfil.CondoClub) {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar, Enum.Permissao.Apagar };
            }

            return new List<Enum.Permissao>();
        }

        #endregion

        #region Métodos auxiliares

        private bool ObjectoValido(BD.PrecoFornecedor obj) {
            return obj.PaisID > 0 && obj.OpcaoPagamentoID > 0 &&
                obj.FraccoesAte > 0 && obj.Valor >= 0;

        }

        #endregion

    }
}
