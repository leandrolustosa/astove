using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Astove.BlurAdmin.Web.Controllers
{
    public class CodeController : Controller
    {
        //
        // GET: /Code/

        public ActionResult Index()
        {
            if (!(this.Request.Url.Host.Contains("localhost") && this.User.Identity.Name.Equals("leandro@ainbox.com.br")))
                throw new Exception("Recurso indisponível");

            return View();
        }

    }
}