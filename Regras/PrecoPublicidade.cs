using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CondoClub.Regras {
    public class PrecoPublicidade {

        private _Base<BD.PrecoPublicidade> _base = new _Base<BD.PrecoPublicidade>();

        #region Seleccionar

        public IEnumerable<BD.PrecoPublicidade> Lista(int? paisID, int? zonaPublicidadeID, UtilizadorAutenticado utilizador) {

            if (!Permissoes(utilizador).Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (BD.Context ctx = new BD.Context()) {
                return ctx.PrecoPublicidade.
                            Include("Pais").
                            Include("ZonaPublicidade").
                            Where(pp =>
                                (paisID == null || pp.PaisID == paisID) &&
                                (zonaPublicidadeID == null || pp.ZonaPublicidadeID  == zonaPublicidadeID)).ToList();
            }

        }

        public BD.PrecoPublicidade Abrir(long id, UtilizadorAutenticado utilizador) {
            using (BD.Context ctx = new BD.Context()) {
                BD.PrecoPublicidade obj = ctx.PrecoPublicidade.
                                            Include("Pais").
                                            Include("ZonaPublicidade").
                                            FirstOrDefault(pp => pp.PrecoPublicidadeID == id);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exceptions.SemPermissao();

                return obj;
            }
        }

        #endregion

        #region Inserir/Actualizar

        public void Inserir(BD.PrecoPublicidade obj, UtilizadorAutenticado utilizador) {

            if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Gravar)) {
                throw new Exceptions.SemPermissao();
            }

            if (!ObjectoValido(obj)) {
                throw new Exceptions.DadosIncorrectos();
            }

            using (BD.Context ctx = new BD.Context()) {

                if (ctx.PrecoPublicidade.
                    Any(pp =>
                            pp.PaisID == obj.PaisID &&
                            pp.ZonaPublicidadeID == obj.ZonaPublicidadeID &&
                            pp.FraccoesAte == obj.FraccoesAte)) {

                    throw new Exceptions.PrecoRepetidoRange();

                }

                _base.Inserir(obj, ctx);
                ctx.SaveChanges();
            }

        }


        public void Actualizar(BD.PrecoPublicidade obj, UtilizadorAutenticado utilizador) {

            if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Gravar)) {
                throw new Exceptions.SemPermissao();
            }

            if (!ObjectoValido(obj)) {
                throw new Exceptions.DadosIncorrectos();
            }

            using (BD.Context ctx = new BD.Context()) {

                if (ctx.PrecoPublicidade.
                    Any(pp =>
                            pp.PrecoPublicidadeID != obj.PrecoPublicidadeID &&
                            pp.PaisID == obj.PaisID &&
                            pp.ZonaPublicidadeID == obj.ZonaPublicidadeID &&
                            pp.FraccoesAte == obj.FraccoesAte)) {

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
                Regras.BD.PrecoPublicidade obj = _base.Abrir(id, ctx);

                if (obj == null)
                    throw new Exceptions.DadosIncorrectos();

                if (!Permissoes(utilizador, obj).Contains(Enum.Permissao.Apagar))
                    throw new Exceptions.SemPermissao();

                _base.Apagar(obj.PrecoPublicidadeID, ctx);
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

        public static List<Enum.Permissao> Permissoes(UtilizadorAutenticado utilizador, BD.PrecoPublicidade obj) {
            if (utilizador.Perfil == Regras.Enum.Perfil.CondoClub) {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar, Enum.Permissao.Apagar };
            }

            return new List<Enum.Permissao>();
        }

        #endregion

        #region Métodos auxiliares

        private bool ObjectoValido(BD.PrecoPublicidade obj) {
            return obj.PaisID > 0 && obj.ZonaPublicidadeID > 0 &&
                obj.FraccoesAte > 0 && obj.Valor >= 0;

        }

        #endregion

    }
}
