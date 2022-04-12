using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CondoClub.Web.Controllers
{
    [Authorize]
    public class ErroController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index(string aspxerrorpath)
        {
            return View("Geral");
        }


        [AllowAnonymous]
        public ActionResult PaginaNaoEncontrada(string aspxerrorpath)
        {
            return View("PaginaNaoEncontrada");
        }

    }
}
