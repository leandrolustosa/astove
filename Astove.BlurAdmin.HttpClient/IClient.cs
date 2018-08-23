using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AInBox.Astove.Core.Model;
using System.Threading.Tasks;
using WebApiDoodle.Net.Http.Client.Model;

namespace Empresa.Projeto.HttpApiClient
{
    public interface IClient<TModel, TRequestModel, TUpdateRequestModel> 
        where TModel : IModel, IDto, new()
        where TRequestModel : class, IBindingModel, new()
        where TUpdateRequestModel : class, IBindingModel, new()
    {
        Task<PaginatedDto<TModel>> GetEntitiesAsync(PaginatedRequestCommand request);

        Task<TModel> GetEntityAsync(int id);

        Task AddEntityAsync(TRequestModel requestModel);

        Task ExecuteActionPostAsync<T>(string baseUriTemplate, T requestModel) where T : IBindingModel;

        Task UpdateEntityAsync(int id, TUpdateRequestModel requestModel);

        Task ExecuteActionPutAsync<T>(string baseUriTemplate, T requestModel) where T : IBindingModel;

        Task RemoveEntityAsync(int id);
    }
}