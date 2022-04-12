using System.Web.Mvc;
using System.Web;

namespace CondoClub.Web {
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());

            //desligar cache para todas as acções (tem o problema de desligar a cache no controlador de imagens)
            filters.Add(new OutputCacheAttribute {
                VaryByParam = "*",
                Duration = 0,
                NoStore = true,
            });

        }
    }

}
