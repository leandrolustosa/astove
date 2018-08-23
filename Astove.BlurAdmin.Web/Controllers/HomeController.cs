using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Astove.BlurAdmin.Web.Controllers
{
    [OutputCache(Duration = 24 * 3600)]
    [Authorize]
    public class HomeController : Controller
    {
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