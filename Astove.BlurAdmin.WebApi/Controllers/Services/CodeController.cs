using System;
using System.Linq;
using System.Web.Http;
using System.Reflection;
using AInBox.Astove.Core.Model;
using AInBox.Astove.Core.Attributes;
using AInBox.Astove.Core.Service;

namespace Astove.BlurAdmin.WebApi.Controllers.Services
{
    public class CodeController : ApiController
    {
        private readonly ICodeService codeService;
        private Type[] Types
        {
            get
            {
                Assembly assembly = Assembly.LoadFrom(System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/"), "bin") + "\\Astove.BlurAdmin.Model.dll");
                var types = (from type in assembly.GetTypes()
                             where Attribute.IsDefined(type, typeof(DataEntityAttribute))
                             select type).ToArray();
                return types;
            }
        }

        public CodeController(ICodeService codeService)
        {
            this.codeService = codeService;
        }

        [ActionName("GetModel")]
        public IHttpActionResult GetModel()
        {
            return Ok(new { Success = true, Message = "", Model = codeService.GetCodeModel(this.Types) });
        }

        [ActionName("GetModels")]
        public IHttpActionResult GetModels(string id)
        {
            return Ok(new { Success = true, Message = "", Models = codeService.GetCodeModels(this.Types, id) });
        }

        [ActionName("GenerateHtml")]
        public IHttpActionResult PostGenerateHtml(GenerateHtmlModel requestModel)
        {
            var typeName = string.Concat(requestModel.ModelId, ", Astove.BlurAdmin.Model");
            var type = Type.GetType(typeName);
            var ignoreColumns = new[] { "Id", "AngularConditions" };
            var props = type.GetProperties().Where(p => ignoreColumns.Any(c => !c.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase))).ToArray();

            var model = codeService.GenerateHtml(this.Types, requestModel, type, props);

            return Ok(new { Success = true, Message = "", Model = model });
        }
    }
}
