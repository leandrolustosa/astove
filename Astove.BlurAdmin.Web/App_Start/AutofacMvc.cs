//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using Autofac;
//using System.Web.Mvc;
//using Autofac.Integration.Mvc;
//using Aldabra.Astove.HttpApiClient;
//using Aldabra.Astove.Core.Model;
//using Aldabra.Model;
//using Aldabra.Model.Request.VendaProduto;
//using Aldabra.Model.Request.VendaProdutoArquivo;
//using Aldabra.Model.Request.PessoaPresenteada;
//using WebApiDoodle.Net.Http.Client.Model;
//using Aldabra.Astove.Core.Logging;
//using Aldabra.Model.Request.ProdutoPersonalizado;

//namespace Site
//{
//    internal static class AutofacMvc
//    {
//        internal static void Initialize()
//        {
//            var builder = new ContainerBuilder();
//            DependencyResolver.SetResolver(new AutofacDependencyResolver(RegisterServices(builder)));
//        }

//        private static IContainer RegisterServices(ContainerBuilder builder)
//        {
//            builder.RegisterControllers(typeof(MvcApplication).Assembly);

//            //WebApiClientContext apiClientContext = WebApiClientContext.Create(cfg =>
//            //    cfg.SetCredentialsFromAppSetting("username", "pasword").ConnectTo("http://localhost:1734"));

//            //WebApiClientContext apiClientContext = WebApiClientContext.Create(cfg =>
//            //    cfg.SetCredentialsFromAppSetting("username", "pasword").ConnectTo("http://unicacomovoce.aldabra.com.br"));

//            WebApiClientContext apiClientContext = WebApiClientContext.Create(cfg =>
//                cfg.SetCredentialsFromAppSetting("username", "pasword").ConnectTo(System.Configuration.ConfigurationManager.AppSettings["SiteUnicaComoVoceUrl"]));

//            //WebApiClientContext apiClientContext = WebApiClientContext.Create(cfg =>
//            //    cfg.SetCredentialsFromAppSetting("username", "pasword").ConnectTo("http://win21.hospedagemdesites.ws"));

//            builder.Register(c => apiClientContext.GetEntityClient<VendaProduto, VendaProdutoRequestModel, VendaProdutoUpdateRequestModel>())
//                .As(typeof(IClient<VendaProduto, VendaProdutoRequestModel, VendaProdutoUpdateRequestModel>))
//                .InstancePerHttpRequest();

//            builder.Register(c => apiClientContext.GetEntityClient<VendaProdutoArquivo, VendaProdutoArquivoRequestModel, VendaProdutoArquivoUpdateRequestModel>())
//                .As(typeof(IClient<VendaProdutoArquivo, VendaProdutoArquivoRequestModel, VendaProdutoArquivoUpdateRequestModel>))
//                .InstancePerHttpRequest();

//            builder.Register(c => apiClientContext.GetEntityClient<PessoaPresenteada, PessoaPresenteadaRequestModel, PessoaPresenteadaUpdateRequestModel>())
//                .As(typeof(IClient<PessoaPresenteada, PessoaPresenteadaRequestModel, PessoaPresenteadaUpdateRequestModel>))
//                .InstancePerHttpRequest();

//            builder.Register(c => apiClientContext.GetEntityClient<PessoaPresenteadaLista, PessoaPresenteadaRequestModel, ListaPessoaPresenteadaUpdateRequestModel>())
//                .As(typeof(IClient<PessoaPresenteadaLista, PessoaPresenteadaRequestModel, ListaPessoaPresenteadaUpdateRequestModel>))
//                .InstancePerHttpRequest();

//            builder.RegisterGeneric(typeof(NLogLogger<>)).As(typeof(ILog<>)).InstancePerHttpRequest();

//            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies()).As<IModel>();

//            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies()).As<IDto>();

//            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies()).As<IRequestModel>();

//            return builder.Build();
//        }
//    }
//}