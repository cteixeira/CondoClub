using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CondoClub.Web.Models.DadosMestre;

namespace CondoClub.Web.Controllers.DadosMestre
{
    [Authorize]
    public class PerfilUtilizadorController : Controller
    {
        public SelectList ConstroiDropDown()
        {
            return Util.ConstroiDropDownDadosMestres(
                new Regras.DadosMestre.PerfilUtilizador().Lista(ControladorSite.Pais),
                typeof(Models.DadosMestre.PerfilUtilizador),
                typeof(Regras.BD.PerfilUtilizador)
            );
        }


        public SelectList ConstroiDropDownMorador()
        {
            List<Regras.BD.PerfilUtilizador> lista = 
                new Regras.DadosMestre.PerfilUtilizador().Lista(ControladorSite.Pais)
                .Where(p => 
                    p.PerfilUtilizadorID == (int)Regras.Enum.Perfil.Síndico ||
                    p.PerfilUtilizadorID == (int)Regras.Enum.Perfil.Morador ||
                    p.PerfilUtilizadorID == (int)Regras.Enum.Perfil.Portaria
                )
                .ToList();

            return Util.ConstroiDropDownDadosMestres(
                lista,
                typeof(Models.DadosMestre.PerfilUtilizador),
                typeof(Regras.BD.PerfilUtilizador)
            );
        }
    }


    [Authorize]
    public class PaisController : Controller
    {
        public SelectList ConstroiDropDown()
        {
            return Util.ConstroiDropDownDadosMestres(
                new Regras.DadosMestre.Pais().Lista(),
                typeof(Models.DadosMestre.Pais),
                typeof(Regras.BD.Pais)
            );
        }
    }


    [Authorize]
    public class ExtratoSocialController : Controller
    {
        public SelectList ConstroiDropDown()
        {
            return Util.ConstroiDropDownDadosMestres(
                new Regras.DadosMestre.ExtratoSocial().Lista(),
                typeof(Models.DadosMestre.ExtratoSocial),
                typeof(Regras.BD.ExtratoSocial)
            );
        }
    }


    [Authorize]
    public class FormaPagamentoController : Controller
    {
        public SelectList ConstroiDropDown()
        {
            return Util.ConstroiDropDownDadosMestres(
                new Regras.DadosMestre.FormaPagamento().Lista(),
                typeof(Models.DadosMestre.FormaPagamento),
                typeof(Regras.BD.FormaPagamento)
            );
        }
    }


    [Authorize]
    public class OpcaoPagamentoController : Controller
    {
        public SelectList ConstroiDropDown()
        {
            return Util.ConstroiDropDownDadosMestres(
                new Regras.DadosMestre.OpcaoPagamento().Lista(),
                typeof(Models.DadosMestre.OpcaoPagamento),
                typeof(Regras.BD.OpcaoPagamento)
            );
        }
    }


    [Authorize]
    public class CategoriaController : Controller
    {
        public SelectList ConstroiDropDown()
        {
            return Util.ConstroiDropDownDadosMestres(
                new Regras.DadosMestre.Categoria().Lista(),
                typeof(Models.DadosMestre.Categoria),
                typeof(Regras.BD.Categoria)
            );
        }


        public SelectList ConstroiDropDownCategoriasFilhas() {
            IEnumerable<Regras.Fornecedor.Categoria> modelList = Regras.DadosMestre.Categoria.ObterCategoriasNaoRaiz();


            return new SelectList(modelList.OrderBy(model => model.Designacao).ToList(), "CategoriaID", "Designacao");
        }

        //public SelectList ConstroiDropDownCategoriasFilhas() {
        //    Regras.DadosMestre.Categoria regras = new Regras.DadosMestre.Categoria();

        //    Dictionary<Regras.BD.Categoria, List<string>> dict = new Dictionary<Regras.BD.Categoria, List<string>>();

        //    List<Regras.BD.Categoria> categorias = new Regras.DadosMestre.Categoria().Lista()
        //        .Where(c => c.CategoriaPaiID.HasValue).ToList();

        //    foreach (Regras.BD.Categoria obj in categorias) {
        //        if (!regras.TemFilhas(obj.CategoriaID))
        //            dict.Add(obj, AdicionarDescricaoPai(obj, regras, new List<string>()));
        //    }


        //    // Criar SelectList
        //    List<SelectListItem> modelList = new List<SelectListItem>();
        //    string designacao;

        //    foreach (var kvp in dict) {
        //        designacao = string.Empty;

        //        kvp.Value.Reverse();

        //        kvp.Value.ForEach(s => designacao += s + " | ");
        //        designacao += kvp.Key.Designacao;

        //        modelList.Add(new SelectListItem() { Value = kvp.Key.CategoriaID.ToString(), Text = designacao });
        //    }

        //    return new SelectList(modelList.OrderBy(model => model.Text).ToList(), "Value", "Text");
        //}


        //private List<string> AdicionarDescricaoPai(Regras.BD.Categoria obj, Regras.DadosMestre.Categoria regras, 
        //    List<string> designacoes)
        //{
        //    if (obj.CategoriaPaiID.HasValue)
        //    {
        //        Regras.BD.Categoria objPai = regras.Abrir(obj.CategoriaPaiID.Value);
        //        designacoes.Add(objPai.Designacao);
        //        AdicionarDescricaoPai(objPai, regras, designacoes);
        //    }

        //    return designacoes;
        //}
    }


    [Authorize]
    public class ZonaPublicidadeController : Controller {
        public SelectList ConstroiDropDown() {
            return Util.ConstroiDropDownDadosMestres(
                new Regras.DadosMestre.ZonaPublicidade().Lista(),
                typeof(Models.DadosMestre.ZonaPublicidade),
                typeof(Regras.BD.ZonaPublicidade)
            );
        }
    }

}