using System.Collections.Generic;
using System.Reflection;
using System.Web.Http.Dispatcher;

namespace Astove.BlurAdmin.WebApi.Tests
{
    class TestWebApiResolver : DefaultAssembliesResolver
    {
        public override ICollection<Assembly> GetAssemblies()
        {
            return new List<Assembly> { typeof(Astove.BlurAdmin.WebApi.Controllers.PessoasController).Assembly };

        }
    }
}