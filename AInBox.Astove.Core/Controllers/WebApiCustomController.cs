using System.Web.Http;
using AInBox.Astove.Core.Model;
using AInBox.Astove.Core.Service;
using AInBox.Astove.Core.Data;
using WebApiDoodle.Net.Http.Client.Model;
using Autofac;

namespace AInBox.Astove.Core.Controllers
{
    [Authorize]
    public class WebApiCustomController<TEntity, TModel> : BaseApiController<TEntity, TModel>
        where TEntity : class, IEntity, new()
        where TModel : class, IModel, IDto, new()
    {
        public WebApiCustomController()
        {
        }

        public WebApiCustomController(IComponentContext container, IEntityService<TEntity> service)
            : base(container, service)
        {
        }
    }
}
