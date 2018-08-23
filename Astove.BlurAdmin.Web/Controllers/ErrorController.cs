using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Astove.BlurAdmin.Web.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Success200()
        {
            return View("200");
        }

        [Authorize]
        public ActionResult Error400()
        {
            return View("400");
        }

        [Authorize]
        public ActionResult Alert()
        {
            return View();
        }

        [Authorize]
        public ActionResult Error401()
        {
            return View("401");
        }

        [Authorize]
        public ActionResult Error404()
        {
            return View("404");
        }

        [Authorize]
        public ActionResult Error408()
        {
            return View("408");
        }

        [Authorize]
        public ActionResult Error500()
        {
            return View("500");
        }
    }
}