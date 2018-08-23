using AInBox.Astove.Core.Controllers;
using AInBox.Astove.Core.Data;
using AInBox.Astove.Core.Options;
using Autofac;
using Astove.BlurAdmin.Data;
using Astove.BlurAdmin.Model;
using Astove.BlurAdmin.Services;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using AInBox.Astove.Core.Model;
using AInBox.Astove.Core.Service;

namespace Astove.BlurAdmin.WebApi.Controllers
{
    [Authorize]
    public class CidadesController : WebApiCustomController<Cidade, BaseModel>
    {
        public CidadesController(IComponentContext container, IEntityService<Cidade> service) : base(container, service)
        {
        }

        [ActionName("Options")]
        public async Task<IHttpActionResult> GetOptionsByParentId(string parentId)
        {
            var result = await this.Service.GetOptionsByEstadoId(parentId);
            if (result.IsValid)
                return Ok<DropDownStringOptions>(result.Options);
            else
                return GetErrorHttpActionResult(result);
        }
    }
}