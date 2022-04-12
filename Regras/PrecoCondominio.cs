using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CondoClub.Regras {

    public class PrecoCondominio {

        private _Base<BD.PrecoCondominio> _base = new _Base<BD.PrecoCondominio>();

        #region Seleccionar

        public IEnumerable<BD.PrecoCondominio> Lista(int? paisID, int? opcaoPagamentoID, int? extratoSocialID, UtilizadorAutenticado utilizador) {

            if (!Permissoes(utilizador).Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context()) {
                return ctx.PrecoCondominio.
                            Include("Pais").
                            Include("OpcaoPagamento").
                            Include("ExtratoSocial").
                            Where(pc => 
                                (paisID == null || pc.PaisID == paisID) && 
                                (opcaoPagamentoID == null || pc.OpcaoPagamentoID == opcaoPagamentoID) && 
                                (extratoSocialID == null || pc.ExtratoSocialID == extratoSocialID)).ToList();
            }

        }

        public BD.PrecoCondominio Abrir(long id, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {
                BD.PrecoCondominio obj = ctx.PrecoCondominio.
                                            Include("Pais").
                                            Include("OpcaoPagamento").
                                            Include("ExtratoSocial").
                                            FirstOrDefault(pc => pc.PrecoCondominioID == id);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exceptions.SemPermissao();

                return obj;
            }
        }

        #endregion

        #region Inserir/Actualizar

        public void Inserir(BD.PrecoCondominio obj, UtilizadorAutenticado utilizador) {

            if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Gravar)) { 
                    throw new Exceptions.SemPermissao();
            }

            if (!ObjectoValido(obj)) { 
                throw new Exceptions.DadosIncorrectos();
            }

            using (BD.Context ctx = new BD.Context()) {

                if (ctx.PrecoCondominio.
                    Any(pc =>
                            pc.PaisID == obj.PaisID &&
                            pc.OpcaoPagamentoID == obj.OpcaoPagamentoID &&
                            pc.ExtratoSocialID == obj.ExtratoSocialID &&
                            pc.FraccoesAte == obj.FraccoesAte)) {

                                throw new Exceptions.PrecoRepetidoRange();

                }

                _base.Inserir(obj, ctx);
                ctx.SaveChanges();
            }

        }


        public void Actualizar(BD.PrecoCondominio obj, UtilizadorAutenticado utilizador) {

            if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Gravar)) {
                throw new Exceptions.SemPermissao();
            }

            if (!ObjectoValido(obj)) {
                throw new Exceptions.DadosIncorrectos();
            }

            using (BD.Context ctx = new BD.Context()) {

                if (ctx.PrecoCondominio.
                    Any(pc =>
                            pc.PrecoCondominioID != obj.PrecoCondominioID &&
                            pc.PaisID == obj.PaisID &&
                            pc.OpcaoPagamentoID == obj.OpcaoPagamentoID &&
                            pc.ExtratoSocialID == obj.ExtratoSocialID &&
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
                Regras.BD.PrecoCondominio obj = _base.Abrir(id, ctx);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Apagar))
                    throw new Exceptions.SemPermissao();

                _base.Apagar(obj.PrecoCondominioID, ctx);
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

        public static List<Enum.Permissao> Permissoes(UtilizadorAutenticado utilizador, BD.PrecoCondominio obj) {
            if (utilizador.Perfil == Regras.Enum.Perfil.CondoClub) {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar, Enum.Permissao.Apagar };
            }

            return new List<Enum.Permissao>();
        }

        #endregion

        #region Métodos auxiliares

        private bool ObjectoValido(BD.PrecoCondominio obj) {
            return obj.PaisID > 0 && obj.OpcaoPagamentoID > 0 &&
                obj.ExtratoSocialID > 0 && obj.FraccoesAte > 0 && obj.Valor >= 0;

        }

        #endregion

    }
}
