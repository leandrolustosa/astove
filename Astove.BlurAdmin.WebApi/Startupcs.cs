using Microsoft.Owin;
using Owin;

//[assembly: OwinStartup(typeof(Astove.BlurAdmin.WebApi.Startup))]

namespace Astove.BlurAdmin.WebApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}