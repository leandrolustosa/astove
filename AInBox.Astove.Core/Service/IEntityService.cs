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

namespace AInBox.Astove.Core.Service
{
    public interface IEntityService<TEntity> 
        where TEntity : class, IEntity, new()
    {
        IComponentContext Container { get; set; }
        IEntityRepository<TEntity> Repository { get; }
        IMongoDatabase MongoDatabase { get; }
        IMongoService MongoService { get; set; }

        HashSet<TEntity> GetEntities();

        PaginatedList<TEntity> GetEntities(IComponentContext container, IRequestFilter cmd, Type modelType, string[] ordersBy, string[] directions, string[] includeProperties);
        Task<PaginatedList<TEntity>> GetEntitiesAsync(IComponentContext container, IRequestFilter cmd, Type modelType, string[] ordersBy, string[] directions, string[] includeProperties);

        List<KeyValue> GetEntities(string select, object[] selectParameters, string whereClause, object[] parameters, string[] ordersBy, string[] includeProperties);
        Task<List<KeyValue>> GetEntitiesAsync(string select, object[] selectParameters, string whereClause, object[] parameters, string[] ordersBy, string[] includeProperties);

        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate, bool noTracking = false, params Expression<Func<TEntity, object>>[] includeProperties);
        IQueryable<TEntity> Where<TModel>(IComponentContext container, string[] ordersBy = null, string[] includeProperties = null);

        TEntity GetSingle(int id, bool noTracking = false, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TEntity> GetSingleAsync(int id, bool noTracking = false, params Expression<Func<TEntity, object>>[] includeProperties);
        TEntity GetSingle(int id, string[] includeProperties, bool noTracking = false);
        Task<TEntity> GetSingleAsync(int id, string[] includeProperties, bool noTracking = false);
        TEntity GetSingle(IComponentContext container, IRequestFilter cmd, int id, bool noTracking = false);
        Task<TEntity> GetSingleAsync(IComponentContext container, IRequestFilter cmd, int id, bool noTracking = false);
        TEntity GetSingle(int id, bool noTracking = false);
        Task<TEntity> GetSingleAsync(int id, bool noTracking = false);

        int Add(TEntity entity);
        Task<int> AddAsync(TEntity entity);
        int[] AddMany(IEnumerable<TEntity> entities);
        Task<int[]> AddManyAsync(IEnumerable<TEntity> entities);

        void Edit(int id, TEntity entity);
        Task EditAsync(int id, TEntity entity);
        void EditMany(IEnumerable<TEntity> entities);
        Task EditManyAsync(IEnumerable<TEntity> entities);

        void Delete(int id);
        Task DeleteAsync(int id);
        void DeleteMany(IEnumerable<int> ids);
        Task DeleteManyAsync(IEnumerable<int> ids);

        Task<BaseResultModel> ReloadMongoCollection<TMongoModel>(bool loadParents = true, StringBuilder sb = null, params Expression<Func<TEntity, object>>[] includeProperties) where TMongoModel : class, IMongoModel, new();
    }
}
