using System.Web.Mvc;

namespace Astove.BlurAdmin.Web.Controllers
{
    [OutputCache(Duration = 24 * 3600)]
    [Authorize]
    public class ModalController : Controller
    {
        public ActionResult AddOrEditIndicacao()
        {
            return View();
        }

        public ActionResult AddOrEditTipoQuarto()
        {
            return View();
        }

        public ActionResult AddOrEditAgenda()
        {
            return View();
        }

        public ActionResult AddOrEditHospede()
        {
            return View();
        }

        public ActionResult AssociarHotelEvento()
        {
            return View();
        }

        public ActionResult EditIndicacaoAgencia()
        {
            return View();
        }

        public ActionResult EditIndicacaoExpositorAgencia()
        {
            return View();
        }

        public ActionResult AssociarIndicacao()
        {
            return View();
        }

        public ActionResult EnviarEmail()
        {
            return View();
        }
    }
}