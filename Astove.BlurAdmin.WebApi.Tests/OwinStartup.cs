using System;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using AInBox.Astove.Core.Model;
using Microsoft.Owin.Security.OAuth;
using Astove.BlurAdmin.WebApi.Config;
using AInBox.Astove.Core.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using AInBox.Astove.Core.Providers;

[assembly: OwinStartup(typeof(Astove.BlurAdmin.WebApi.Tests.OwinStartup))]

namespace Astove.BlurAdmin.WebApi.Tests
{
    public class OwinStartup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }
        
        public void Configuration(IAppBuilder app)
        {
            //ApplicationUserManager.DataProtectionProvider = new DpapiDataProtectionProvider("api.yesfeiras");
            ApplicationUserManager.DataProtectionProvider = new MachineKeyProtectionProvider();

            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            HttpConfiguration config = new HttpConfiguration();

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            config.Services.Replace(typeof(IAssembliesResolver), new TestWebApiResolver());

            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            
            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                // In production mode set AllowInsecureHttp = false
                AllowInsecureHttp = true
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);

            WebApiConfig.Register(config);
            AutofacWebApi.Initialize(config, false);

            app.UseWebApi(config);
        }
    }
}
