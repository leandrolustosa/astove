using AInBox.Astove.Core.Model;
using AInBox.Astove.Core.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Astove.BlurAdmin.Web.Controllers
{
    [OutputCache(Duration = 24 * 3600)]
    public class CommonController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AstoveToolsClose()
        {
            return View();
        }

        public ActionResult BaSideBar()
        {
            return View("ba-sidebar");
        }

        public ActionResult BackTop()
        {
            return View("backtop");
        }

        public ActionResult BaWizard()
        {
            return View("baWizard");
        }

        public ActionResult BaWizardStep()
        {
            return View("baWizardStep");
        }

        public ActionResult ContentTop()
        {
            return View("contentTop");
        }

        public ActionResult Content()
        {
            return View("content");
        }

        public ActionResult MsgCenter()
        {
            return View("msgCenter");
        }

        public ActionResult PageTop()
        {
            return View("pageTop");
        }

        public ActionResult Widgets()
        {
            return View("widgets");
        }

        public ActionResult Navigation()
        {
            return View("navigation");
        }

        public ActionResult TopNavigation()
        {
            return View("top_navigation");
        }

        public ActionResult TopNavBar()
        {
            return View("topnavbar");
        }

        public ActionResult SkinConfig()
        {
            return View("skin-config");
        }

        [HttpPost]
        [Authorize]
        public ContentResult Upload(HttpPostedFileBase file)
        {
            FileModel model = null;
            if (Request.Form["model"]!=null)
                model = JsonConvert.DeserializeObject<FileModel>(Request.Form["model"]);
            else
                model = new FileModel
                {
                    Directory = Request.Form["directory"],
                    PropertyName = Request.Form["propertyName"],
                    Type = Request.Form["type"]
                };
            
            FileManager upFile = new FileManager(file, model.Directory, model.Type);
            var result = upFile.Upload();

            var modelResult = new FileModel
            {
                Directory = model.Directory,
                PropertyName = model.PropertyName,
                FileName = result["filename"],
                Url = result["url"]
            };

            if (result != null)
            {
                return new ContentResult
                {
                    ContentType = "json/application",
                    Content = modelResult.ParseToJson(),
                    ContentEncoding = Encoding.UTF8
                };
            }

            return null;
        }
    }
}