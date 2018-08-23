using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using System.Reflection;
using Astove.BlurAdmin.Services;
using Astove.BlurAdmin.Data;
using System.Data.Entity;
using AInBox.Astove.Core.Data;
using AInBox.Astove.Core.Logging;
using MongoDB.Driver;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security;
using AInBox.Astove.Core.Security;
using Microsoft.AspNet.Identity;
using AInBox.Astove.Core.Model;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using AInBox.Astove.Core.Service;

namespace Astove.BlurAdmin.WebApi.Config
{
    public static class AutofacWebApi
    {
        public static void Initialize(HttpConfiguration config, bool test = false)
        {
            Initialize(config, RegisterServices(new ContainerBuilder(), test));
        }

        public static void Initialize(HttpConfiguration config, IContainer container)
        {
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private static IContainer RegisterServices(ContainerBuilder builder, bool test)
        {
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterGeneric(typeof(EntityService<>))
                .As(typeof(IEntityService<>))
                .InstancePerRequest();

            builder.RegisterType<MongoClient>()
                .WithParameter("connectionString", System.Configuration.ConfigurationManager.AppSettings["MongoHost"])
                .As<IMongoClient>()                
                .InstancePerRequest();

            builder.RegisterType<MongoService>()
                .As<IMongoService>()
                .InstancePerRequest();

            builder.RegisterType<CodeService>()
                .As<ICodeService>()
                .InstancePerRequest();

            builder.RegisterGeneric(typeof(EntityRepository<>))
                .As(typeof(IEntityRepository<>))
                .InstancePerRequest();

            if (!test)
            {
                builder.RegisterType<AstoveContext>()
                   .As<IAstoveContext>()
                   .InstancePerRequest();
            }
            else
            {
                builder.RegisterType<FakeAstoveContext>()
                   .As<IAstoveContext>()
                   .InstancePerRequest();
            }

            builder.RegisterGeneric(typeof(NLogLogger<>)).As(typeof(ILog<>)).InstancePerRequest();

            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies()).As<IEntity>();
            
            return builder.Build();
        }

        private static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return assembly.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
        }
    }
}
