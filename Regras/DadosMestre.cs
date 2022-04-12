using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;

namespace CondoClub.Regras.DadosMestre {

    public class ExtratoSocial {

        private _Base<BD.ExtratoSocial> _base = new _Base<BD.ExtratoSocial>();

        public IEnumerable<BD.ExtratoSocial> Lista() {
            return _base.Lista();        
        }

        public IEnumerable<BD.ExtratoSocial> Lista(string designacao) {
            using (BD.Context ctx = new BD.Context()) {
                return ctx.ExtratoSocial.Where(o => (designacao == null || o.Designacao.Contains(designacao))).ToList();
            };
        }

        public void Inserir(BD.ExtratoSocial obj) {
            if (Lista(obj.Designacao).Count() > 0)
                throw new Exceptions.DesignacaoRepetida();
            _base.Inserir(obj);
        }

        public void Actualizar(BD.ExtratoSocial obj) {
            using (BD.Context ctx = new BD.Context()) {
                if (ctx.ExtratoSocial.Any(o => o.ExtratoSocialID != obj.ExtratoSocialID && o.Designacao == obj.Designacao))
                    throw new Exceptions.DesignacaoRepetida();
            };
            _base.Actualizar(obj);
        }

    }

    public class Pais {

        private _Base<BD.Pais> _base = new _Base<BD.Pais>();

        public IEnumerable<BD.Pais> Lista() {
            return _base.Lista();
        }

    }

    public class TabelaLocalizada { }

    public class PerfilUtilizador {

        private _Base<BD.PerfilUtilizador> _base = new _Base<BD.PerfilUtilizador>();

        public IEnumerable<BD.PerfilUtilizador> Lista(Enum.Pais pais) {
            IEnumerable<BD.PerfilUtilizador> perfis = _base.Lista();
            new Localizacao().TraduzirLista(Enum.TabelaLocalizada.PerfilUtilizador, pais, perfis);
            return perfis;
        }

    }

    public class OpcaoPagamento {

        private _Base<BD.OpcaoPagamento> _base = new _Base<BD.OpcaoPagamento>();

        public IEnumerable<BD.OpcaoPagamento> Lista() {
            return _base.Lista();
        }

    }

    public class FormaPagamento {

        private _Base<BD.FormaPagamento> _base = new _Base<BD.FormaPagamento>();

        public IEnumerable<BD.FormaPagamento> Lista() {
            return _base.Lista();
        }

    }

    public class Categoria {

        private _Base<BD.Categoria> _base = new _Base<BD.Categoria>();

        public IEnumerable<BD.Categoria>Lista(){
            return _base.Lista();
        }

        public static IEnumerable<Fornecedor.Categoria> ObterCategoriasNaoRaiz() {

            string chaveCache = "FornecedorCategoraDropDown";

            List<Fornecedor.Categoria> ret;

            //validar se existe em cache
            ret = MemoryCache.Default.Get(chaveCache) as List<Fornecedor.Categoria>;
            if (ret != null) {
                return ret;
            }

            ret = new List<Fornecedor.Categoria>();

            using (BD.Context ctx = new BD.Context()) {

                IEnumerable<BD.Categoria> cats = ctx.Categoria.Where(c => c.CategoriaPaiID.HasValue);
                foreach (BD.Categoria c in cats) {

                    ret.Add(new Fornecedor.Categoria {
                        CategoriaID = c.CategoriaID,
                        Designacao = String.Concat(c.Categoria2.Designacao, " | ", c.Designacao)
                    });

                }

            }

            //guardar em cache
            DateTimeOffset duracaoCache = DateTimeOffset.Now.AddHours(24);
            MemoryCache.Default.Add(chaveCache, ret.ToList(), duracaoCache);

            return ret;

        }

        public bool TemFilhas(long id)
        {
            using (BD.Context ctx = new BD.Context())
            {
                return ctx.Categoria.FirstOrDefault(c =>
                    c.CategoriaPaiID.HasValue && c.CategoriaPaiID.Value == id) != null;
            }
        }
    
    }

    public class ZonaPublicidade {

        private _Base<BD.ZonaPublicidade> _base = new _Base<BD.ZonaPublicidade>();

        public IEnumerable<BD.ZonaPublicidade> Lista() {
            return _base.Lista();
        }
    
    }
}
