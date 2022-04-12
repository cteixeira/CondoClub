using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace CondoClub.Web.Controllers {

    [Authorize]
    public class EstatisticaController : Controller {

        #region Acções
        
        public ActionResult Index() {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.Estatistica.Permissoes(ControladorSite.Utilizador);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", permissoes);

                return View();

            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.Erro.Geral));
            }
        }


        public PartialViewResult _Lista(Models.Estatistica.Pesquisa pesquisa) {
            try {
                List<Regras.Enum.Permissao> permissoes = Regras.Estatistica.Permissoes(ControladorSite.Utilizador, pesquisa.TipoEstatistica);

                if (!permissoes.Contains(Regras.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);
                
                return PartialView("_Lista", ConstroiLista(pesquisa));

            } catch (Exception ex) {
                Regras.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }


        #endregion


        #region Funcões Auxiliares


        public IEnumerable<Models.Estatistica.Categoria> ConstroiLista(Models.Estatistica.Pesquisa pesquisa) {
            
            IEnumerable<Regras.Estatistica.Categoria> categorias =
                new Regras.Estatistica().Lista(pesquisa.Inicio, pesquisa.Fim, 
                                            pesquisa.TipoEstatistica, pesquisa.EmpresaID, 
                                            ControladorSite.Utilizador);

            List<Models.Estatistica.Categoria> modelList = new List<Models.Estatistica.Categoria>();
            foreach (var obj in categorias) {
                modelList.Add(new Models.Estatistica.Categoria(obj));
            }

            return modelList;
        }


        public SelectList ConstroiDropDownTipoEstatistica() {

            Dictionary<Regras.Enum.TipoEstatistica, string> tipoEstatistica = new Dictionary<Regras.Enum.TipoEstatistica, string>();

            if (TipoDisponivel(Regras.Enum.TipoEstatistica.NovoComunicado))
                tipoEstatistica.Add(Regras.Enum.TipoEstatistica.NovoComunicado, Resources.Estatistica.NovoComunicado);

            if (TipoDisponivel(Regras.Enum.TipoEstatistica.NovaMensagem))
                tipoEstatistica.Add(Regras.Enum.TipoEstatistica.NovaMensagem, Resources.Estatistica.NovaMensagem);

            if (TipoDisponivel(Regras.Enum.TipoEstatistica.NovoQuestionario))
                tipoEstatistica.Add(Regras.Enum.TipoEstatistica.NovoQuestionario, Resources.Estatistica.NovoQuestionario);

            if (TipoDisponivel(Regras.Enum.TipoEstatistica.NovaReserva))
                tipoEstatistica.Add(Regras.Enum.TipoEstatistica.NovaReserva, Resources.Estatistica.NovaReserva);

            if (TipoDisponivel(Regras.Enum.TipoEstatistica.NovoArquivo))
                tipoEstatistica.Add(Regras.Enum.TipoEstatistica.NovoArquivo, Resources.Estatistica.NovoArquivo);

            if (TipoDisponivel(Regras.Enum.TipoEstatistica.NovoMorador))
                tipoEstatistica.Add(Regras.Enum.TipoEstatistica.NovoMorador, Resources.Estatistica.NovoMorador);

            if (TipoDisponivel(Regras.Enum.TipoEstatistica.NovoCondominio))
                tipoEstatistica.Add(Regras.Enum.TipoEstatistica.NovoCondominio, Resources.Estatistica.NovoCondominio);

            if (TipoDisponivel(Regras.Enum.TipoEstatistica.NovoFornecedor))
                tipoEstatistica.Add(Regras.Enum.TipoEstatistica.NovoFornecedor, Resources.Estatistica.NovoFornecedor);

            if (TipoDisponivel(Regras.Enum.TipoEstatistica.NovaAdministradora))
                tipoEstatistica.Add(Regras.Enum.TipoEstatistica.NovaAdministradora, Resources.Estatistica.NovaAdministradora);

            if (TipoDisponivel(Regras.Enum.TipoEstatistica.NovaPublicidade))
                tipoEstatistica.Add(Regras.Enum.TipoEstatistica.NovaPublicidade, Resources.Estatistica.NovaPublicidade);

            if (TipoDisponivel(Regras.Enum.TipoEstatistica.ImpressaoPublicidade))
                tipoEstatistica.Add(Regras.Enum.TipoEstatistica.ImpressaoPublicidade, Resources.Estatistica.ImpressaoPublicidade);

            if (TipoDisponivel(Regras.Enum.TipoEstatistica.CliquePublicidade))
                tipoEstatistica.Add(Regras.Enum.TipoEstatistica.CliquePublicidade, Resources.Estatistica.CliquePublicidade);

            if (TipoDisponivel(Regras.Enum.TipoEstatistica.PagamentoPublicidade))
                tipoEstatistica.Add(Regras.Enum.TipoEstatistica.PagamentoPublicidade, Resources.Estatistica.PagamentoPublicidade);

            if (TipoDisponivel(Regras.Enum.TipoEstatistica.PagamentoFornecedor))
                tipoEstatistica.Add(Regras.Enum.TipoEstatistica.PagamentoFornecedor, Resources.Estatistica.PagamentoFornecedor);

            if (TipoDisponivel(Regras.Enum.TipoEstatistica.PagamentoCondominio))
                tipoEstatistica.Add(Regras.Enum.TipoEstatistica.PagamentoCondominio, Resources.Estatistica.PagamentoCondominio);

            if (TipoDisponivel(Regras.Enum.TipoEstatistica.Erro))
                tipoEstatistica.Add(Regras.Enum.TipoEstatistica.Erro, Resources.Estatistica.Erro);

            return new SelectList(
                tipoEstatistica.Select(x => new { ID = x.Key, Name = x.Value }).ToList(),
                "ID",
                "Name"
            );
        }


        private bool TipoDisponivel(Regras.Enum.TipoEstatistica tipo) {
            if (Regras.Estatistica.Permissoes(ControladorSite.Utilizador, tipo).Contains(Regras.Enum.Permissao.Visualizar))
                return true;
            else
                return false;
        }

        #endregion

    }

}
