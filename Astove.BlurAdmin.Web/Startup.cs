using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Astove.BlurAdmin.Web.Startup))]

namespace Astove.BlurAdmin.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
