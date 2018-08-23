using System;
using AInBox.Astove.Core.Model;
using WebApiDoodle.Net.Http.Client.Model;
using AInBox.Astove.Core.Logging;

namespace Empresa.Projeto.HttpApiClient
{
    public static class WebApiClientContextExtensions
    {
        public static IClient<TModel, TRequestModel, TUpdateRequestModel> GetEntityClient<TModel, TRequestModel, TUpdateRequestModel>(this WebApiClientContext apiClientContext)
            where TModel : class, IModel, IDto, new()
            where TRequestModel : class, IModel, IBindingModel, new()
            where TUpdateRequestModel : class, IModel, IBindingModel, new()
        {
            return apiClientContext.GetClient<TModel, TRequestModel, TUpdateRequestModel>(() => new Client<TModel, TRequestModel, TUpdateRequestModel>(apiClientContext.HttpClient, new NLogLogger<TModel>()));
        }

        internal static IClient<TModel, TRequestModel, TUpdateRequestModel> GetClient<TModel, TRequestModel, TUpdateRequestModel>(this WebApiClientContext apiClientContext, Func<IClient<TModel, TRequestModel, TUpdateRequestModel>> valueFactory)
            where TModel : class, IModel, IDto, new()
            where TRequestModel : class, IModel, IBindingModel, new()
            where TUpdateRequestModel : class, IModel, IBindingModel, new()
        {
            return (IClient<TModel, TRequestModel, TUpdateRequestModel>)apiClientContext.Clients.GetOrAdd(typeof(IClient<TModel, TRequestModel, TUpdateRequestModel>), k => valueFactory());
        }
    }
}
