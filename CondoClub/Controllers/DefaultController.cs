using System.Web.Mvc;

namespace CondoClub.Web.Controllers {

    public class DefaultController : Controller {

        public ActionResult Index() {
            if (ControladorSite.Utilizador == null) {
                return RedirectToAction("Login", "Utilizador");
            }
            if (ControladorSite.Utilizador.Perfil == Regras.Enum.Perfil.Fornecedor) {
                return RedirectToAction("Index", "Mensagem");
            }

            return RedirectToAction("Index", "Comunicado");

        }

    }
}
