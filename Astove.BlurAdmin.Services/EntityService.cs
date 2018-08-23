using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AInBox.Astove.Core.Data;
using AInBox.Astove.Core.Options;
using AInBox.Astove.Core.Service;
using Autofac;
using AInBox.Astove.Core.Filter;
using AInBox.Astove.Core.Logging;
using AInBox.Astove.Core.Extensions;
using System.Threading.Tasks;
using MongoDB.Driver;
using AInBox.Astove.Core.Model;
using AInBox.Astove.Core.Attributes;
using System.Data.Entity;
using System.Text;
using AInBox.Astove.Core.Enums;

namespace Astove.BlurAdmin.Services
{
    public class EntityService<TEntity> : IEntityService<TEntity>
        where TEntity : class, IEntity, new()
    {
        private readonly IComponentContext container;
        private readonly IEntityRepository<TEntity> context;
        private readonly IMongoClient mongoClient;
        private readonly IMongoDatabase mongoDatabase;
        private readonly ILog<IEntityService<TEntity>> logger;

        public IMongoDatabase MongoDatabase { get { return mongoDatabase; } }
        public IMongoService MongoService { get; set; }

        public IEntityRepository<TEntity> Repository { get { return context; } }

        public IComponentContext Container { get; set; }

        public EntityService(IEntityRepository<TEntity> context, IComponentContext container, IMongoClient mongoClient, IMongoService mongoService, ILog<IEntityService<TEntity>> logger)
        {
            this.context = context;
            this.container = container;
            this.mongoClient = mongoClient;
            this.MongoService = mongoService;
            this.mongoDatabase = mongoClient.GetDatabase(System.Configuration.ConfigurationManager.AppSettings["MongoDatabase"]);
            this.logger = logger;
        }

        public HashSet<TEntity> GetEntities()
        {
            return context.GetAll().ToHashSet();
        }

        public PaginatedList<TEntity> GetEntities(IComponentContext container, IRequestFilter cmd, Type modelType, string[] ordersBy, string[] directions, string[] includeProperties)
        {
            Filter filter = FilterFactory.GenerateFilter(container, cmd, modelType, new KeyValue { Key = (!string.IsNullOrEmpty(cmd.ParentKey)) ? int.Parse(cmd.ParentKey) : 0, Value = cmd.ParentValue });
            return context.Paginate(container, filter, cmd, modelType, cmd.Page, cmd.Take, ordersBy, directions, includeProperties);
        }

        public async Task<PaginatedList<TEntity>> GetEntitiesAsync(IComponentContext container, IRequestFilter cmd, Type modelType, string[] ordersBy, string[] directions, string[] includeProperties)
        {
            Filter filter = FilterFactory.GenerateFilter(container, cmd, modelType, new KeyValue { Key = (!string.IsNullOrEmpty(cmd.ParentKey)) ? int.Parse(cmd.ParentKey) : 0, Value = cmd.ParentValue });
            return await context.PaginateAsync(container, filter, cmd, modelType, cmd.Page, cmd.Take, ordersBy, directions, includeProperties);
        }

        public List<KeyValue> GetEntities(string select, object[] selectParametrs, string whereClause, object[] parameters, string[] ordersBy, string[] includeProperties)
        {
            logger.Info("Select {0} from {1} where {2} order by {3} including {4}", new[] { select, typeof(TEntity).Name, whereClause, (ordersBy == null) ? "" : string.Join(", ", ordersBy), (includeProperties == null) ? "" : string.Join(", ", includeProperties) });
            return context.GetList(select, selectParametrs, whereClause, parameters, ordersBy, includeProperties);
        }

        public async Task<List<KeyValue>> GetEntitiesAsync(string select, object[] selectParametrs, string whereClause, object[] parameters, string[] ordersBy, string[] includeProperties)
        {
            logger.Info("Select {0} from {1} where {2} order by {3} including {4}", new[] { select, typeof(TEntity).Name, whereClause, (ordersBy == null) ? "" : string.Join(", ", ordersBy), (includeProperties == null) ? "" : string.Join(", ", includeProperties) });
            return await context.GetListAsync(select, selectParametrs, whereClause, parameters, ordersBy, includeProperties);
        }

        public virtual IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate, bool noTracking = false, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return context.Where(predicate, noTracking, includeProperties);
        }

        public virtual IQueryable<TEntity> Where<TModel>(IComponentContext container, string[] ordersBy = null, string[] includeProperties = null)
        {
            return context.Where<TModel>(container, ordersBy, includeProperties);
        }

        public TEntity GetSingle(int id, bool noTracking = false, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            TEntity obj = context.GetSingle(id, noTracking, includeProperties);
            return obj;
        }

        public async Task<TEntity> GetSingleAsync(int id, bool noTracking = false, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            TEntity obj = await context.GetSingleAsync(id, noTracking, includeProperties);
            return obj;
        }

        public TEntity GetSingle(int id, string[] includeProperties, bool noTracking = false)
        {
            TEntity obj = context.GetSingle(id, includeProperties, noTracking);
            return obj;
        }

        public async Task<TEntity> GetSingleAsync(int id, string[] includeProperties, bool noTracking = false)
        {
            TEntity obj = await context.GetSingleAsync(id, includeProperties, noTracking);
            return obj;
        }

        public virtual TEntity GetSingle(int id, bool noTracking = false)
        {
            TEntity obj = context.GetSingle(id, noTracking);
            return obj;
        }

        public async virtual Task<TEntity> GetSingleAsync(int id, bool noTracking = false)
        {
            TEntity obj = await context.GetSingleAsync(id, noTracking);
            return obj;
        }

        public virtual TEntity GetSingle(IComponentContext container, IRequestFilter cmd, int id, bool noTracking = false)
        {
            TEntity obj = context.GetSingle(container, cmd, id, noTracking);
            return obj;
        }

        public async virtual Task<TEntity> GetSingleAsync(IComponentContext container, IRequestFilter cmd, int id, bool noTracking = false)
        {
            TEntity obj = await context.GetSingleAsync(container, cmd, id, noTracking);
            return obj;
        }

        public virtual int[] AddMany(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                context.Add(entity);
            context.Save();
            return entities.Select(p => p.Id).ToArray();
        }

        public virtual int Add(TEntity entity)
        {
            context.Add(entity);
            context.Save();
            return entity.Id;
        }

        public async virtual Task<int[]> AddManyAsync(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                context.Add(entity);
            await context.SaveAsync();
            return entities.Select(p => p.Id).ToArray();
        }

        public async virtual Task<int> AddAsync(TEntity entity)
        {
            context.Add(entity);
            await context.SaveAsync();
            return entity.Id;
        }

        public virtual void EditMany(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                context.Edit(entity);
            context.Save();
        }

        public virtual void Edit(int id, TEntity entity)
        {
            context.Edit(entity);
            context.Save();
        }

        public async virtual Task EditManyAsync(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                context.Edit(entity);
            await context.SaveAsync();
        }

        public async virtual Task EditAsync(int id, TEntity entity)
        {
            context.Edit(entity);
            await context.SaveAsync();
        }

        public virtual void DeleteMany(IEnumerable<int> ids)
        {
            foreach (var id in ids)
            {
                TEntity obj = context.GetSingle(id);
                context.Delete(obj);
            }
            context.Save();
        }

        public virtual void Delete(int id)
        {
            TEntity obj = context.GetSingle(id);
            context.Delete(obj);
            context.Save();
        }

        public async virtual Task DeleteManyAsync(IEnumerable<int> ids)
        {
            foreach (var id in ids)
            {
                TEntity obj = await context.GetSingleAsync(id);
                context.Delete(obj);
            }
            await context.SaveAsync();
        }

        public async virtual Task DeleteAsync(int id)
        {
            TEntity obj = await context.GetSingleAsync(id);
            context.Delete(obj);
            await context.SaveAsync();
        }

        public async Task<BaseResultModel> ReloadMongoCollection<TMongoModel>(bool loadParents = true, StringBuilder sb = null, params Expression<Func<TEntity, object>>[] includeProperties)
            where TMongoModel : class, IMongoModel, new()
        {
            try
            {
                var type = typeof(TMongoModel);
                var properties = type.GetProperties().Where(prop => prop.IsDefined(typeof(ColumnDefinitionAttribute), false));
                var attr = type.GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault();

                var builderFilter = Builders<KeyValueString>.Filter;
                var filter = builderFilter.Empty;

                var list = await this.Where(p => true, false, includeProperties).ToListAsync();
                var options = list.Where(p => true).Select(o => new KeyValueString { ParentId = (attr == null || string.IsNullOrEmpty(attr.ParentColumn)) ? o.Id.ToString() : Convert.ToString(o.GetNestedPropertyValue(attr.ParentColumn)), Key = (attr == null || string.IsNullOrEmpty(attr.KeyColumn)) ? o.Id.ToString() : Convert.ToString(o.GetNestedPropertyValue(attr.KeyColumn)), Value = (attr == null || string.IsNullOrEmpty(attr.ValueColumn)) ? o.ToString() : Convert.ToString(o.GetNestedPropertyValue(attr.ValueColumn)) }).ToList();
                var collectionName = this.MongoService.GetCollectionNameForOptions<TMongoModel>();
                await this.MongoService.InsertManyMongoObjects<KeyValueString>(options, collection: collectionName);

                if (attr == null || !attr.LoadOnlyOptions)
                {
                    var mongoList = new List<TMongoModel>();
                    foreach (var item in list)
                    {
                        TMongoModel obj;
                        if (loadParents)
                            obj = item.ToModel<TMongoModel, IMongoModel>(container, false, 0);
                        else
                            obj = item.CreateInstanceOf<TMongoModel>();

                        obj.ParentId = item.Id.ToString();
                        if (properties != null)
                        {
                            foreach (var propInfo in properties)
                            {
                                var colAttr = propInfo.GetCustomAttributes(true).OfType<ColumnDefinitionAttribute>().FirstOrDefault();
                                if (colAttr != null && !string.IsNullOrEmpty(colAttr.EntityProperty) && colAttr.EnumType != null)
                                {
                                    var enumProperty = obj.GetType().GetProperty(colAttr.EntityProperty);
                                    if (enumProperty != null)
                                    {
                                        var value = (int)enumProperty.GetValue(obj, null);
                                        propInfo.SetValue(obj, EnumUtility.GetEnumText(colAttr.EnumType, value), null);
                                    }
                                }
                                else if (colAttr != null && !string.IsNullOrEmpty(colAttr.EntityProperty))
                                {
                                    item.SetValue(obj, propInfo, colAttr.EntityProperty);
                                }
                            }
                        }
                        mongoList.Add(obj);
                    }
                    await this.MongoService.InsertManyMongoObjects<TMongoModel>(mongoList);
                }

                return new BaseResultModel { IsValid = true };
            }
            catch (Exception ex)
            {
                if (sb != null)
                    sb.AppendLine(string.Format("Atualizar {0}: {1} - {2}", typeof(TMongoModel).Name, 500, ex.GetExceptionMessageWithStackTrace()));

                return new BaseResultModel { IsValid = false, Message = ex.GetExceptionMessageWithStackTrace() };
            }
        }
    }
}
