using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.WebPages;
using System.Web.Security;
using System.Web;

namespace CondoClub.Web {
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication {

        protected void Application_Start() {
            DisplayModeProvider.Instance.Modes.Insert(0, new DefaultDisplayMode() {
                ContextCondition = (context => Util.GetDeviceType(context.GetOverriddenUserAgent()) == "tablet")
            });

            DisplayModeProvider.Instance.Modes.Insert(1, new DefaultDisplayMode("tv") {
                ContextCondition = (context => Util.GetDeviceType(context.GetOverriddenUserAgent()) == "tv")
            }); 



            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            
            ContentTypeConfig.RegisterContentTypes();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    Controllers.UtilizadorController utilizador = new Controllers.UtilizadorController();
                    ControladorSite.Utilizador = utilizador.DadosUtilizadorAutenticado(Convert.ToInt64(User.Identity.Name));
                    Util.ReadCookie(Context);
                }
                //forçar a cultar a ser pt-br, futuramente se a cultura for definida pelo utilizador pode ser forçada aqui também
                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("pt-BR");
                System.Threading.Thread.CurrentThread.CurrentCulture = ci;
                System.Threading.Thread.CurrentThread.CurrentUICulture = ci;

            }
            catch (Exception ex)
            {
                String erro = Regras.Util.TratamentoErro(null, GetType().FullName, ex, null);
            }
        }

    }
}