using System.Web.Mvc;

namespace Astove.BlurAdmin.Web.Controllers
{
    [OutputCache(Duration = 24 * 3600)]
    [Authorize]
    public class EmpresaController : Controller
    {
        // GET: /Empresa
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Update()
        {
            return View();
        }
    }
}