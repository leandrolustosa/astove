using System;
using System.Web;
using System.Web.Http;
using System.Net.Http;
//using WebApiDoodle.Web.Filters;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using System.Web.Http.Validation;
using System.Web.Http.Validation.Providers;
using AInBox.Astove.Core.Model;
using Microsoft.Owin.Security.OAuth;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Astove.BlurAdmin.WebApi.Config
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            TraceConfig.Register(config);

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            //BsonSerializer.RegisterSerializer(typeof(DateTime), DateTimeSerializer.LocalInstance);

            config.Services.RemoveAll(typeof(ModelValidatorProvider), (provider) => provider is InvalidModelValidatorProvider);
            //config.Filters.Add(new InvalidModelStateFilterAttribute());

            config.ParameterBindingRules
                .Insert(0, descriptor =>
                    typeof(IRequestCommand).IsAssignableFrom(descriptor.ParameterType)
                        ? new FromUriAttribute().GetBinding(descriptor)
                        : null);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/ac/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints: new { action = @"(?i)^[a-z]+" }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/v1/{controller}/{id}",
                defaults: new { action = "DefaultAction", id = RouteParameter.Optional }
            );

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api
            //config.EnableSystemDiagnosticsTracing();
        }
    }
}