using System;
using System.Collections.Generic;
using AInBox.Astove.Core.Options;
using System.Linq;
using System.Linq.Expressions;
using Autofac;
using AInBox.Astove.Core.Filter;
using System.Threading.Tasks;
using AInBox.Astove.Core.Data;
using MongoDB.Driver;
using AInBox.Astove.Core.Model;
using System.Text;
using WebApiDoodle.Net.Http.Client.Model;

namespace AInBox.Astove.Core.Service
{
    public interface IMongoService
    {
        IMongoDatabase MongoDatabase { get; }

        Task<DropDownStringOptions> GetMongoOptions<TMongoModel>(List<SortDefinition<KeyValueString>> sorts = null, string parentId = null, string collection = null) where TMongoModel : class, IMongoModel, new();
        Task<TMongoModel> GetMongoObject<TMongoModel>(string id, string collection = null) where TMongoModel : class, IMongoModel, new();
        Task<TMongoModel> GetMongoObjectByKey<TMongoModel>(string key, string collection = null) where TMongoModel : class, IMongoModel, new();
        Task<TMongoModel> GetMongoObjectByValue<TMongoModel>(string value, string collection = null) where TMongoModel : class, IMongoModel, new();
        Task<TMongoModel> GetMongoObjectByParentId<TMongoModel>(string parentId, string collection = null) where TMongoModel : class, IMongoModel, new();
        Task<TMongoModel> GetMongoObjectByParentId<TMongoModel>(int entityId, string collection = null) where TMongoModel : class, IMongoModel, new();
        Task<string> InsertMongoObject<TMongoModel>(TMongoModel obj, string collection = null) where TMongoModel : class, IMongoModel, new();
        Task<string> InsertMongoObject<TMongoModel>(int parentId, IBindingModel model, string collection = null) where TMongoModel : class, IMongoModel, new();
        Task<string> InsertMongoObject<TMongoModel, TEntity>(int parentId, TEntity model, string collection = null) where TMongoModel : class, IMongoModel, new() where TEntity : class, IEntity, new();
        Task<string> InsertMongoObject<TMongoModel, TEntity>(IComponentContext container, int parentId, TEntity model, string collection = null) where TMongoModel : class, IMongoModel, new() where TEntity : class, IEntity, new();
        Task InsertManyMongoObjects<TMongoModel>(List<TMongoModel> list, FilterDefinition<TMongoModel> filter = null, string collection = null, bool append = false) where TMongoModel : class, IMongoModel, new();
        Task InsertManyMongoObjects<TMongoModel, TEntity>(List<TEntity> list, string collection = null, bool append = false) where TMongoModel : class, IMongoModel, new() where TEntity : class, IEntity, new();
        Task<long> UpdateMongoObject<TMongoModel>(string id, TMongoModel obj, string collection = null) where TMongoModel : class, IMongoModel, new();
        Task<long> UpdateMongoObject<TMongoModel>(string id, IBindingModel obj, string collection = null) where TMongoModel : class, IMongoModel, new();
        Task<long> DeleteMongoObject<TMongoModel>(string id, string collection = null) where TMongoModel : class, IMongoModel, new();
        Task<long> DeleteMongoObjectByKey<TMongoModel>(string key, string collection = null) where TMongoModel : class, IMongoModel, new();
        Task<long> DeleteMongoObjectByParentId<TMongoModel>(string parentId, string collection = null) where TMongoModel : class, IMongoModel, new();
        Task<PaginatedMongoList<TMongoModel, R>> GetMongoListAsync<TMongoModel, R>(IComponentContext container, PaginatedRequestCommand model, string collection = null) where TMongoModel : class, IMongoModel, new() where R : class, IMongoModel, IDto, new();
        Task<List<TMongoModel>> GetMongoListAsync<TMongoModel>(List<FilterDefinition<TMongoModel>> filters = null, List<SortDefinition<TMongoModel>> sorts = null, string collection = null) where TMongoModel : class, IMongoModel, new();
        void AddFilterDefinitionToList<TMongoModel>(List<FilterDefinition<TMongoModel>> filters, string property, string value, int operatorValue = 1) where TMongoModel : class, IMongoModel, new();
        void AddFilterDefinitionToList<TMongoModel>(List<FilterDefinition<TMongoModel>> filters, Expression<Func<TMongoModel, object>> property, string value, int operatorValue = 1) where TMongoModel : class, IMongoModel, new();
        FilterDefinition<TMongoModel> GetFilter<TMongoModel>(string property, int operatorValue, string value) where TMongoModel : class, IMongoModel, new();
        FilterDefinition<TMongoModel> GetFilter<TMongoModel>(Expression<Func<TMongoModel, object>> property, int operatorValue, string value) where TMongoModel : class, IMongoModel, new();
        Task<BaseResultModel> ReloadMongoCollection<TMongoModel, TEntity>(IEntityService<TEntity> service, StringBuilder sb = null, params Expression<Func<TEntity, object>>[] includeProperties) where TMongoModel : class, IMongoModel, new() where TEntity : class, IEntity, new();
        Task<BaseResultModel> ReloadMongoCollection<TMongoModel, TEntity>(List<TEntity> list, StringBuilder sb = null) where TMongoModel : class, IMongoModel, new() where TEntity :  class, IEntity, new();

        Task<BaseResultModel> ReloadMongoCollection<TMongoModel, TEntity>(IComponentContext container, IEntityService<TEntity> service, StringBuilder sb = null, params Expression<Func<TEntity, object>>[] includeProperties) where TMongoModel : class, IMongoModel, new() where TEntity : class, IEntity, new();
        Task<BaseResultModel> ReloadMongoCollection<TMongoModel, TEntity>(IComponentContext container, List<TEntity> list, StringBuilder sb = null) where TMongoModel : class, IMongoModel, new() where TEntity : class, IEntity, new();

        Task SetDropDownOptions<TModel>(TModel model, string parentKey = null);
        Task SetDropDownOptions<TModel>(List<TModel> list, string parentKey = null);

        string GetCollectionNameForOptions<TMongoModel>() where TMongoModel : class, IMongoModel, new();
        string GetCollectionName<TMongoModel>() where TMongoModel : class, IMongoModel, new();
    }
}
