using AInBox.Astove.Core.Data;
using AInBox.Astove.Core.Logging;
using AInBox.Astove.Core.Service;
using AInBox.Astove.Core.UnitTest;
using Autofac;
using Astove.BlurAdmin.Data;

namespace Astove.BlurAdmin.Services.Tests
{
    public class Bootstrap
    {
        public static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(System.AppDomain.CurrentDomain.GetAssemblies()).As<IAstoveUnitTest>();

            builder.RegisterGeneric(typeof(EntityRepository<>))
                .As(typeof(IEntityRepository<>))
                .InstancePerLifetimeScope();

            builder.RegisterType<FakeAstoveContext>()
               .As<IAstoveContext>()
               .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(NLogLogger<>)).As(typeof(ILog<>)).InstancePerLifetimeScope();
            
            builder.RegisterAssemblyTypes(System.AppDomain.CurrentDomain.GetAssemblies()).As<IEntity>();

            return builder.Build();
        }
    }
}
