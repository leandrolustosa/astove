using System.Web.Http;
using AInBox.Astove.Core.Data;
using AInBox.Astove.Core.Service;
using Autofac;

namespace AInBox.Astove.Core.Controllers
{
    [Authorize]
    public class WebApiMongoController<TEntity> : BaseMongoApiController<TEntity>
        where TEntity : class, IEntity, new()
    {
        public WebApiMongoController()
        {
        }

        public WebApiMongoController(IComponentContext container, IEntityService<TEntity> service)
            : base(container, service)
        {
        }
    }
}
