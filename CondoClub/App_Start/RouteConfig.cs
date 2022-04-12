using System.Web.Mvc;
using System.Web.Routing;

namespace CondoClub.Web {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //rename de Comunicados para Novidades
            routes.MapRoute(
                name: "Novidade",
                url: "Novidade/{action}/{id}",
                defaults: new { controller = "Comunicado", action = "Index", id = UrlParameter.Optional }
            );

            //rename de Agenda para Contatos
            routes.MapRoute(
                name: "Agenda",
                url: "Contatos/{action}/{id}",
                defaults: new { controller = "Agenda", action = "Index", id = UrlParameter.Optional }
            );

            //rename de Empresas para Administradoras
            routes.MapRoute(
                name: "Empresa",
                url: "Administradoras/{action}/{id}",
                defaults: new { controller = "Empresa", action = "Index", id = UrlParameter.Optional }
            );
            
            routes.MapRoute(
                name: "RegistoEmpresa",
                url: "Registo/Administradoras/{id}",
                defaults: new { controller = "Registo", action = "Empresa", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Pesquisa",
                url: "Servico/Pesquisa/{*TermoPesquisa}",
                defaults: new { controller = "Servico", action = "Pesquisa", TermoPesquisa = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Fornecedor",
                url: "Servico/Fornecedor/{*Nome}",
                defaults: new { controller = "Servico", action = "Fornecedor", Nome = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ArquivoGeral",
                url: "Arquivo/{action}/{id}",
                defaults: new { controller = "Arquivo", id = UrlParameter.Optional },
                constraints: new { action = "^_Lista|_DetalheVisualizarDirectoria|_DetalheEditarDirectoria|GravarDirectoria|" +
                    "ApagarDirectoria|_DetalheVisualizarFicheiro|_DetalheEditarFicheiro|GravarFicheiros|ActualizarFicheiro|ApagarFicheiro$"
                }
            );

            routes.MapRoute(
                name: "Arquivo",
                url: "Arquivo/{*Caminho}",
                defaults: new { controller = "Arquivo", action = "Index", Caminho = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Enquete",
                url: "Enquete/{action}/{id}",
                defaults: new { controller = "Questionario", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Calendario",
                url: "Calendario/Reserva/{id}",
                defaults: new { controller = "Calendario", action = "Index" }
            );

            routes.MapRoute(
                name: "ValidarPagamento",
                url: "Pagamento/Validar/{idCifrado}",
                defaults: new { controller = "Pagamento", action = "Validar" }
            );

            routes.MapRoute(
                name: "FinalizarPagamento",
                url: "Pagamento/Finalizar/{idCifrado}",
                defaults: new { controller = "Pagamento", action = "Finalizar" }
            );

            routes.MapRoute(
                name: "ImprimirBoleto",
                url: "Pagamento/ImprimirBoleto/{idCifrado}",
                defaults: new { controller = "Pagamento", action = "ImprimirBoleto" }
            );

            //Precos

            routes.MapRoute(
                name: "Geral",
                url: "{controller}/{action}/{id}",
                defaults: new { action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{*url}",
                defaults: new { controller = "Default", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}