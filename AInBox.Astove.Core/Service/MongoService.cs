using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AInBox.Astove.Core.Data;
using AInBox.Astove.Core.Options;
using Autofac;
using AInBox.Astove.Core.Filter;
using AInBox.Astove.Core.Logging;
using AInBox.Astove.Core.Extensions;
using System.Threading.Tasks;
using MongoDB.Driver;
using AInBox.Astove.Core.Model;
using System.Reflection;
using AInBox.Astove.Core.Attributes;
using System.ComponentModel.DataAnnotations;
using AInBox.Astove.Core.Sort;
using MongoDB.Bson;
using System.Data.Entity;
using AInBox.Astove.Core.Enums;
using System.Text;
using WebApiDoodle.Net.Http.Client.Model;

namespace AInBox.Astove.Core.Service
{
    public class MongoService : IMongoService
    {
        private readonly IMongoClient mongoClient;
        private readonly IMongoDatabase mongoDatabase;
        private readonly ILog<IMongoService> logger;

        public IMongoDatabase MongoDatabase { get { return mongoDatabase; } }

        public MongoService(IMongoClient mongoClient, ILog<IMongoService> logger)
        {
            this.mongoClient = mongoClient;
            this.mongoDatabase = mongoClient.GetDatabase(System.Configuration.ConfigurationManager.AppSettings["MongoDatabase"]);
            this.logger = logger;
        }

        public async Task<DropDownStringOptions> GetMongoOptions<TMongoModel>(List<SortDefinition<KeyValueString>> sorts = null, string parentId = null, string collection = null)
            where TMongoModel : class, IMongoModel, new()
        {
            if (collection == null)
                collection = GetCollectionNameForOptions<TMongoModel>();

            var col = this.MongoDatabase.GetCollection<KeyValueString>(collection);
            var sort = Builders<KeyValueString>.Sort.Ascending(o => o.Value);
            if (sorts != null)
                sort = Builders<KeyValueString>.Sort.Combine(sorts);

            var filter = Builders<KeyValueString>.Filter.Empty;
            if (!string.IsNullOrEmpty(parentId))
                filter = Builders<KeyValueString>.Filter.Eq(p => p.ParentId, parentId);

            var list = await col.Find(filter).Sort(sort).ToListAsync();

            return new DropDownStringOptions
            {
                DisplayText = "value",
                DisplayValue = "key",
                Items = list.ToArray()
            };
        }

        public async Task<TMongoModel> GetMongoObject<TMongoModel>(string id, string collection = null)
            where TMongoModel : class, IMongoModel, new()
        {
            if (collection == null)
                collection = GetCollectionName<TMongoModel>();

            var col = this.MongoDatabase.GetCollection<TMongoModel>(collection);
            var filter = Builders<TMongoModel>.Filter.Eq("Id", id);

            var cursor = await col.FindAsync(filter);
            var list = await cursor.ToListAsync();
            var obj = list.FirstOrDefault();

            return obj;
        }

        public async Task<TMongoModel> GetMongoObjectByKey<TMongoModel>(string key, string collection = null)
            where TMongoModel : class, IMongoModel, new()
        {
            if (collection == null)
                collection = GetCollectionName<TMongoModel>();

            var col = this.MongoDatabase.GetCollection<TMongoModel>(collection);
            var filter = Builders<TMongoModel>.Filter.Eq("Key", key);

            var cursor = await col.FindAsync(filter);
            var list = await cursor.ToListAsync();
            var obj = list.FirstOrDefault();

            return obj;
        }

        public async Task<TMongoModel> GetMongoObjectByValue<TMongoModel>(string value, string collection = null)
            where TMongoModel : class, IMongoModel, new()
        {
            if (collection == null)
                collection = GetCollectionName<TMongoModel>();

            var col = this.MongoDatabase.GetCollection<TMongoModel>(collection);
            var filter = Builders<TMongoModel>.Filter.Eq("Value", value);

            var cursor = await col.FindAsync(filter);
            var list = await cursor.ToListAsync();
            var obj = list.FirstOrDefault();

            return obj;
        }

        public async Task<TMongoModel> GetMongoObjectByParentId<TMongoModel>(int entityId, string collection = null)
            where TMongoModel : class, IMongoModel, new()
        {
            return await GetMongoObjectByParentId<TMongoModel>(entityId.ToString(), collection);
        }

        public async Task<TMongoModel> GetMongoObjectByParentId<TMongoModel>(string parentId, string collection = null)
            where TMongoModel : class, IMongoModel, new()
        {
            if (collection == null)
                collection = GetCollectionName<TMongoModel>();

            var col = this.MongoDatabase.GetCollection<TMongoModel>(collection);
            var filter = Builders<TMongoModel>.Filter.Eq("ParentId", parentId);

            var cursor = await col.FindAsync(filter);
            var list = await cursor.ToListAsync();
            var obj = list.FirstOrDefault();

            return obj;
        }

        public async Task<string> InsertMongoObject<TMongoModel>(TMongoModel obj, string collection = null)
            where TMongoModel : class, IMongoModel, new()
        {
            if (collection == null)
                collection = GetCollectionName<TMongoModel>();

            var col = this.MongoDatabase.GetCollection<TMongoModel>(collection);
            await col.InsertOneAsync(obj);
            var id = obj.GetType().GetProperty("Id").GetValue(obj, null);
            return Convert.ToString(id);
        }

        public async Task<string> InsertMongoObject<TMongoModel>(int parentId, IBindingModel model, string collection = null)
            where TMongoModel : class, IMongoModel, new()
        {
            var obj = model.CreateInstanceOf<TMongoModel>();
            obj.ParentId = parentId.ToString();

            return await InsertMongoObject<TMongoModel>(obj, collection);
        }

        public async Task<string> InsertMongoObject<TMongoModel, TEntity>(int parentId, TEntity entity, string collection = null)
            where TMongoModel : class, IMongoModel, new()
            where TEntity : class, IEntity, new()
        {
            var obj = entity.CreateInstanceOf<TMongoModel>();
            obj.ParentId = parentId.ToString();

            var type = typeof(TMongoModel);
            var properties = type.GetProperties().Where(prop => prop.IsDefined(typeof(ColumnDefinitionAttribute), false));
            
            if (properties != null)
            {
                foreach (var propInfo in properties)
                {
                    var colAttr = propInfo.GetCustomAttributes(true).OfType<ColumnDefinitionAttribute>().FirstOrDefault();
                    if (colAttr != null && !string.IsNullOrEmpty(colAttr.EntityProperty))
                    {
                        entity.SetValue(obj, propInfo, colAttr.EntityProperty);
                    }
                }
            }

            return await InsertMongoObject<TMongoModel>(obj, collection);
        }

        public async Task<string> InsertMongoObject<TMongoModel, TEntity>(IComponentContext container, int parentId, TEntity entity, string collection = null)
            where TMongoModel : class, IMongoModel, new()
            where TEntity : class, IEntity, new()
        {
            var obj = entity.ToModel<TMongoModel, IMongoModel>(container);
            obj.ParentId = parentId.ToString();

            var type = typeof(TMongoModel);
            var properties = type.GetProperties().Where(prop => prop.IsDefined(typeof(ColumnDefinitionAttribute), false));

            if (properties != null)
            {
                foreach (var propInfo in properties)
                {
                    var colAttr = propInfo.GetCustomAttributes(true).OfType<ColumnDefinitionAttribute>().FirstOrDefault();
                    if (colAttr != null && !string.IsNullOrEmpty(colAttr.EntityProperty) && propInfo.GetValue(obj, null) == null)
                    {
                        entity.SetValue(obj, propInfo, colAttr.EntityProperty);
                    }
                }
            }

            return await InsertMongoObject<TMongoModel>(obj, collection);
        }

        public async Task InsertManyMongoObjects<TMongoModel>(List<TMongoModel> list, FilterDefinition<TMongoModel> filter = null, string collection = null, bool append = false)
            where TMongoModel : class, IMongoModel, new()
        {
            if (collection == null)
                collection = GetCollectionName<TMongoModel>();

            var col = this.MongoDatabase.GetCollection<TMongoModel>(collection);
            if (filter == null)
                filter = Builders<TMongoModel>.Filter.Empty;

            if (!append)
                await col.DeleteManyAsync(filter);

            if (list.Count > 0)
                await col.InsertManyAsync(list);
        }

        public async Task InsertManyMongoObjects<TMongoModel, TEntity>(List<TEntity> list, string collection = null, bool append = false)
            where TMongoModel : class, IMongoModel, new()
            where TEntity : class, IEntity, new()
        {
            try
            {
                if (collection == null)
                    collection = GetCollectionName<TMongoModel>();

                var type = typeof(TMongoModel);
                var properties = type.GetProperties().Where(prop => prop.IsDefined(typeof(ColumnDefinitionAttribute), false));
                var attr = type.GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault();
                
                await this.InsertManyMongoObjects<KeyValueString>(list.Where(p => true).Select(o => new KeyValueString { ParentId = (attr == null || string.IsNullOrEmpty(attr.ParentColumn)) ? o.Id.ToString() : Convert.ToString(o.GetNestedPropertyValue(attr.ParentColumn)), Key = (attr == null || string.IsNullOrEmpty(attr.KeyColumn)) ? o.Id.ToString() : Convert.ToString(o.GetNestedPropertyValue(attr.KeyColumn)), Value = (attr == null || string.IsNullOrEmpty(attr.ValueColumn)) ? o.ToString() : Convert.ToString(o.GetNestedPropertyValue(attr.ValueColumn)) }).ToList());
                var mongoList = new List<TMongoModel>();
                foreach (var item in list)
                {
                    var obj = item.CreateInstanceOf<TMongoModel>();
                    obj.ParentId = item.Id.ToString();
                    if (properties != null)
                    {
                        foreach (var propInfo in properties)
                        {
                            var colAttr = propInfo.GetCustomAttributes(true).OfType<ColumnDefinitionAttribute>().FirstOrDefault();
                            if (colAttr != null && !string.IsNullOrEmpty(colAttr.EntityProperty))
                            {
                                item.SetValue(obj, propInfo, colAttr.EntityProperty);
                            }
                        }
                    }
                    mongoList.Add(obj);
                }
                await this.InsertManyMongoObjects<TMongoModel>(mongoList, collection: collection, append: append);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<long> UpdateMongoObject<TMongoModel>(string id, TMongoModel obj, string collection = null)
            where TMongoModel : class, IMongoModel, new()
        {
            if (collection == null)
                collection = GetCollectionName<TMongoModel>();

            var col = this.MongoDatabase.GetCollection<TMongoModel>(collection);
            var builderFilter = Builders<TMongoModel>.Filter;
            var filter = builderFilter.Eq("Id", id);

            var result = await col.ReplaceOneAsync(filter, obj);
            return result.ModifiedCount;
        }

        public async Task<long> UpdateMongoObject<TMongoModel>(string id, IBindingModel model, string collection = null)
            where TMongoModel : class, IMongoModel, new()
        {
            var obj = await this.GetMongoObject<TMongoModel>(id);
            model.CopyProperties(obj);

            return await UpdateMongoObject<TMongoModel>(id, obj, collection);
        }

        public async Task<long> DeleteMongoObject<TMongoModel>(string id, string collection = null)
            where TMongoModel : class, IMongoModel, new()
        {
            if (collection == null)
                collection = GetCollectionName<TMongoModel>();

            var col = this.MongoDatabase.GetCollection<TMongoModel>(collection);
            var builderFilter = Builders<TMongoModel>.Filter;
            var filter = builderFilter.Eq("Id", id);

            var result = await col.DeleteOneAsync(filter);
            return result.DeletedCount;
        }

        public async Task<long> DeleteMongoObjectByParentId<TMongoModel>(string parentId, string collection = null)
            where TMongoModel : class, IMongoModel, new()
        {
            if (collection == null)
                collection = GetCollectionName<TMongoModel>();

            var col = this.MongoDatabase.GetCollection<TMongoModel>(collection);
            var builderFilter = Builders<TMongoModel>.Filter;
            var filter = builderFilter.Eq("ParentId", parentId);

            var result = await col.DeleteOneAsync(filter);
            return result.DeletedCount;
        }

        public async Task<long> DeleteMongoObjectByKey<TMongoModel>(string key, string collection = null)
            where TMongoModel : class, IMongoModel, new()
        {
            if (collection == null)
                collection = GetCollectionName<TMongoModel>();

            var col = this.MongoDatabase.GetCollection<TMongoModel>(collection);
            var builderFilter = Builders<TMongoModel>.Filter;
            var filter = builderFilter.Eq("Key", key);

            var result = await col.DeleteOneAsync(filter);
            return result.DeletedCount;
        }

        public async Task<PaginatedMongoList<TMongoModel, TReturnModel>> GetMongoListAsync<TMongoModel, TReturnModel>(IComponentContext container, PaginatedRequestCommand model, string collection = null)
            where TMongoModel : class, IMongoModel, new()
            where TReturnModel : class, IMongoModel, IDto, new()
        {
            if (collection == null)
                collection = GetCollectionName<TMongoModel>();

            var col = this.MongoDatabase.GetCollection<TMongoModel>(collection);
            var builderFilter = Builders<TMongoModel>.Filter;
            var builderSort = Builders<TMongoModel>.Sort;

            var filters = new List<FilterDefinition<TMongoModel>>();

            var dict = new List<FilterDictionary>();
            var modelType = typeof(TMongoModel);
            List<PropertyInfo> filterList = modelType.GetProperties().Where(p => p.GetCustomAttributes(true).OfType<FilterWebapiAttribute>().DefaultIfEmpty().FirstOrDefault() != null).OrderBy(o => o.GetCustomAttributes(true).OfType<FilterWebapiAttribute>().First().GroupOrder).ToList();
            foreach (PropertyInfo prop in filterList)
            {
                foreach (Attribute att in prop.GetCustomAttributes(true).OfType<FilterWebapiAttribute>().OrderBy(f => f.Order))
                {
                    FilterWebapiAttribute filterAtt = att as FilterWebapiAttribute;
                    if (filterAtt == null)
                        continue;

                    IFilter f = Activator.CreateInstance(filterAtt.FilterType) as IFilter;
                    f.Property = prop.Name;
                    if (string.IsNullOrEmpty(filterAtt.DisplayName))
                        f.DisplayName = f.Property;

                    var attLength = prop.GetCustomAttributes(true).OfType<StringLengthAttribute>().FirstOrDefault();
                    if (attLength != null)
                        f.Length = attLength.MaximumLength;
                    f.CopyPropertiesValue(filterAtt);

                    if (f.Internal || (dict.Count(d => d.Property == f.Property && d.Type == f.GetType().Name && d.DefaultOperator == f.DefaultOperator && d.DefaultValue.Equals(f.DefaultValue)) == 0))
                    {
                        var defaultValue = Convert.ToString(f.DefaultValue);
                        if (defaultValue.ToLower().Contains("[today"))
                        {
                            var d = defaultValue.Replace("[today", "").Replace("]", "");
                            int dias = 0;
                            Int32.TryParse(d, out dias);
                            defaultValue = DateTime.Today.AddDays(dias).ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
                        }
                        else if (defaultValue.ToLower().Contains("[now"))
                        {
                            var d = defaultValue.Replace("[now", "").Replace("]", "");
                            int dias = 0;
                            Int32.TryParse(d, out dias);
                            defaultValue = DateTime.Now.Date.AddDays(dias).ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
                        }

                        filters.Add(GetFilter<TMongoModel>(f.Property, f.DefaultOperator, Convert.ToString(defaultValue)));
                        dict.Add(new FilterDictionary { Property = f.Property, Type = f.GetType().Name, DefaultOperator = f.DefaultOperator, DefaultValue = defaultValue });
                    }
                }
            }

            if (model.Fields != null)
            {
                for (int i = 0; i < model.Fields.Length; i++)
                {
                    if (model.Fields[i].Equals("CodigoFilial", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var value = model.Values[i];
                        var args = value.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        if (args.Length == 2)
                        {
                            filters.Add(GetFilter<TMongoModel>(model.Fields[i], int.Parse(model.Operators[i]), args[0]));
                            filters.Add(GetFilter<TMongoModel>("Origem", (int)MongoOperator.In, args[1]));
                        }
                        else
                        {
                            filters.Add(GetFilter<TMongoModel>(model.Fields[i], int.Parse(model.Operators[i]), model.Values[i]));
                        }
                    }
                    else
                    {
                        filters.Add(GetFilter<TMongoModel>(model.Fields[i], int.Parse(model.Operators[i]), model.Values[i]));
                    }
                }
            }

            if (!string.IsNullOrEmpty(model.ParentKey) && !string.IsNullOrEmpty(model.ParentValue))
            {
                filters.Add(GetFilter<TMongoModel>(model.ParentValue, (int)MongoOperator.Igual, model.ParentKey));
            }

            var filter = builderFilter.Empty;
            if (filters.Count > 0)
                filter = builderFilter.And(filters);

            var sorts = new List<SortDefinition<TMongoModel>>();
            if (model.OrdersBy != null)
            {
                for (int i = 0; i < model.OrdersBy.Length; i++)
                {
                    var sortDeff = builderSort.Ascending(model.OrdersBy[i]);
                    if (model.Directions != null && model.Directions.Length > i && !model.Directions[i].Equals("asc"))
                        sortDeff = builderSort.Descending(model.OrdersBy[i]);

                    sorts.Add(sortDeff);
                }
            }

            var find = col.Find(filter);
            if (sorts.Count > 0)
                find = find.Sort(builderSort.Combine(sorts));

            var filterOptions = await FilterFactory.GenerateFilterAsync(container, model, typeof(TReturnModel), null);
            var sortOptions = SortFactory.GenerateSortOptions(model);
            return await find.ToPaginatedMongoListAsync<TMongoModel, TReturnModel>(container, filterOptions, sortOptions, null, model.Page, model.Take);
        }

        public async Task<List<TMongoModel>> GetMongoListAsync<TMongoModel>(List<FilterDefinition<TMongoModel>> filters = null, List<SortDefinition<TMongoModel>> sorts = null, string collection = null)
            where TMongoModel : class, IMongoModel, new()
        {
            if (collection == null)
                collection = GetCollectionName<TMongoModel>();

            var col = this.MongoDatabase.GetCollection<TMongoModel>(collection);
            var filter = Builders<TMongoModel>.Filter.Empty;
            if (filters != null && filters.Count > 0)
                filter = Builders<TMongoModel>.Filter.And(filters);
            var builderSort = Builders<TMongoModel>.Sort;

            var find = col.Find(filter);
            if (sorts != null && sorts.Count > 0)
                find = find.Sort(builderSort.Combine(sorts));

            return await find.ToListAsync();
        }

        public void AddFilterDefinitionToList<TMongoModel>(List<FilterDefinition<TMongoModel>> filters, string property, string value, int operatorValue = 1)
            where TMongoModel : class, IMongoModel, new()
        {
            filters.Add(GetFilter<TMongoModel>(property, operatorValue, value));
        }

        public void AddFilterDefinitionToList<TMongoModel>(List<FilterDefinition<TMongoModel>> filters, Expression<Func<TMongoModel, object>> property, string value, int operatorValue = 1)
            where TMongoModel : class, IMongoModel, new()
        {
            filters.Add(GetFilter<TMongoModel>(property, operatorValue, value));
        }

        public async Task<BaseResultModel> ReloadMongoCollection<TMongoModel, TEntity>(IEntityService<TEntity> service, StringBuilder sb = null, params Expression<Func<TEntity, object>>[] includeProperties)
            where TMongoModel : class, IMongoModel, new()
            where TEntity : class, IEntity, new()
        {
            try
            {
                var type = typeof(TMongoModel);
                var properties = type.GetProperties().Where(prop => prop.IsDefined(typeof(ColumnDefinitionAttribute), false));
                var attr = type.GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault();
                
                var builderFilter = Builders<KeyValueString>.Filter;
                var filter = builderFilter.Empty;
                
                var list = await service.Where(p => true, false, includeProperties).ToListAsync();
                var options = list.Where(p => true).Select(o => new KeyValueString { ParentId = (attr == null || string.IsNullOrEmpty(attr.ParentColumn)) ? o.Id.ToString() : Convert.ToString(o.GetNestedPropertyValue(attr.ParentColumn)), Key = (attr == null || string.IsNullOrEmpty(attr.KeyColumn)) ? o.Id.ToString() : Convert.ToString(o.GetNestedPropertyValue(attr.KeyColumn)), Value = (attr == null || string.IsNullOrEmpty(attr.ValueColumn)) ? o.ToString() : Convert.ToString(o.GetNestedPropertyValue(attr.ValueColumn)) }).ToList();
                var collectionName = this.GetCollectionNameForOptions<TMongoModel>();
                await this.InsertManyMongoObjects<KeyValueString>(options, collection: collectionName);

                if (attr == null || !attr.LoadOnlyOptions)
                {
                    var mongoList = new List<TMongoModel>();
                    foreach (var item in list)
                    {
                        var obj = item.CreateInstanceOf<TMongoModel>();
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
                    await this.InsertManyMongoObjects<TMongoModel>(mongoList);
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

        public async Task<BaseResultModel> ReloadMongoCollection<TMongoModel, TEntity>(List<TEntity> list, StringBuilder sb = null) 
            where TMongoModel : class, IMongoModel, new() 
            where TEntity : class, IEntity, new()
        {
            try
            {
                var type = typeof(TMongoModel);
                var properties = type.GetProperties().Where(prop => prop.IsDefined(typeof(ColumnDefinitionAttribute), false));
                var attr = type.GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault();
                
                var builderFilter = Builders<KeyValueString>.Filter;
                var filter = builderFilter.Empty;

                var options = list.Where(p => true).Select(o => new KeyValueString { ParentId = (attr == null || string.IsNullOrEmpty(attr.ParentColumn)) ? o.Id.ToString() : Convert.ToString(o.GetNestedPropertyValue(attr.ParentColumn)), Key = (attr == null || string.IsNullOrEmpty(attr.KeyColumn)) ? o.Id.ToString() : Convert.ToString(o.GetNestedPropertyValue(attr.KeyColumn)), Value = (attr == null || string.IsNullOrEmpty(attr.ValueColumn)) ? o.ToString() : Convert.ToString(o.GetNestedPropertyValue(attr.ValueColumn)) }).ToList();
                var collectionName = this.GetCollectionNameForOptions<TMongoModel>();
                await this.InsertManyMongoObjects<KeyValueString>(options, collection: collectionName);

                if (attr == null || !attr.LoadOnlyOptions)
                {
                    var mongoList = new List<TMongoModel>();
                    foreach (var item in list)
                    {
                        var obj = item.CreateInstanceOf<TMongoModel>();
                        obj.ParentId = item.Id.ToString();
                        if (properties != null)
                        {
                            foreach (var propInfo in properties)
                            {
                                var colAttr = propInfo.GetCustomAttributes(true).OfType<ColumnDefinitionAttribute>().FirstOrDefault();
                                if (colAttr != null && !string.IsNullOrEmpty(colAttr.EntityProperty))
                                {
                                    item.SetValue(obj, propInfo, colAttr.EntityProperty);
                                }
                            }
                        }
                        mongoList.Add(obj);
                    }
                    await this.InsertManyMongoObjects<TMongoModel>(mongoList);
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

        public async Task<BaseResultModel> ReloadMongoCollection<TMongoModel, TEntity>(IComponentContext container, IEntityService<TEntity> service, StringBuilder sb = null, params Expression<Func<TEntity, object>>[] includeProperties)
            where TMongoModel : class, IMongoModel, new()
            where TEntity : class, IEntity, new()
        {
            try
            {
                var type = typeof(TMongoModel);
                var properties = type.GetProperties().Where(prop => prop.IsDefined(typeof(ColumnDefinitionAttribute), false));
                var attr = type.GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault();

                var builderFilter = Builders<KeyValueString>.Filter;
                var filter = builderFilter.Empty;

                var list = await service.Where(p => true, false, includeProperties).ToListAsync();
                var options = list.Where(p => true).Select(o => new KeyValueString { ParentId = (attr == null || string.IsNullOrEmpty(attr.ParentColumn)) ? o.Id.ToString() : Convert.ToString(o.GetNestedPropertyValue(attr.ParentColumn)), Key = (attr == null || string.IsNullOrEmpty(attr.KeyColumn)) ? o.Id.ToString() : Convert.ToString(o.GetNestedPropertyValue(attr.KeyColumn)), Value = (attr == null || string.IsNullOrEmpty(attr.ValueColumn)) ? o.ToString() : Convert.ToString(o.GetNestedPropertyValue(attr.ValueColumn)) }).ToList();
                var collectionName = this.GetCollectionNameForOptions<TMongoModel>();
                await this.InsertManyMongoObjects<KeyValueString>(options, collection: collectionName);

                if (attr == null || !attr.LoadOnlyOptions)
                {
                    var mongoList = new List<TMongoModel>();
                    foreach (var item in list)
                    {
                        var obj = item.ToModel<TMongoModel, IMongoModel>(container, false, 0);
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
                    await this.InsertManyMongoObjects<TMongoModel>(mongoList);
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

        public async Task<BaseResultModel> ReloadMongoCollection<TMongoModel, TEntity>(IComponentContext container, List<TEntity> list, StringBuilder sb = null)
            where TMongoModel : class, IMongoModel, new()
            where TEntity : class, IEntity, new()
        {
            try
            {
                var type = typeof(TMongoModel);
                var properties = type.GetProperties().Where(prop => prop.IsDefined(typeof(ColumnDefinitionAttribute), false));
                var attr = type.GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault();

                var builderFilter = Builders<KeyValueString>.Filter;
                var filter = builderFilter.Empty;

                var options = list.Where(p => true).Select(o => new KeyValueString { ParentId = (attr == null || string.IsNullOrEmpty(attr.ParentColumn)) ? o.Id.ToString() : Convert.ToString(o.GetNestedPropertyValue(attr.ParentColumn)), Key = (attr == null || string.IsNullOrEmpty(attr.KeyColumn)) ? o.Id.ToString() : Convert.ToString(o.GetNestedPropertyValue(attr.KeyColumn)), Value = (attr == null || string.IsNullOrEmpty(attr.ValueColumn)) ? o.ToString() : Convert.ToString(o.GetNestedPropertyValue(attr.ValueColumn)) }).ToList();
                var collectionName = this.GetCollectionNameForOptions<TMongoModel>();
                await this.InsertManyMongoObjects<KeyValueString>(options, collection: collectionName);

                if (attr == null || !attr.LoadOnlyOptions)
                {
                    var mongoList = new List<TMongoModel>();
                    foreach (var item in list)
                    {
                        var obj = item.ToModel<TMongoModel, IMongoModel>(container);
                        obj.ParentId = item.Id.ToString();
                        if (properties != null)
                        {
                            foreach (var propInfo in properties)
                            {
                                var colAttr = propInfo.GetCustomAttributes(true).OfType<ColumnDefinitionAttribute>().FirstOrDefault();
                                if (colAttr != null && !string.IsNullOrEmpty(colAttr.EntityProperty))
                                {
                                    item.SetValue(obj, propInfo, colAttr.EntityProperty);
                                }
                            }
                        }
                        mongoList.Add(obj);
                    }
                    await this.InsertManyMongoObjects<TMongoModel>(mongoList);
                }

                return new BaseResultModel { IsValid = true };
            }
            catch (Exception ex)
            {
                if (sb != null)
                    sb.AppendLine(string.Format("Atualizar {0}: {1} - {2}", typeof(TMongoModel).Name, 500, ex.GetExceptionMessageWithStackTrace()));

                return new BaseResultModel { IsValid = false, StatusCode = 500, Message = ex.GetExceptionMessageWithStackTrace() };
            }
        }

        public FilterDefinition<TMongoModel> GetFilter<TMongoModel>(string property, int operatorValue, string value)
            where TMongoModel : class, IMongoModel, new()
        {
            var builderFilter = Builders<TMongoModel>.Filter;
            if (operatorValue == (int)MongoOperator.ComecaoCom)
                return builderFilter.Regex(property, new MongoDB.Bson.BsonRegularExpression(string.Concat("^", value), "i"));
            if (operatorValue == (int)MongoOperator.Contem)
                return builderFilter.Regex(property, new MongoDB.Bson.BsonRegularExpression(value, "i"));
            else if (operatorValue == (int)MongoOperator.Diferente)
                return builderFilter.Ne(property, GetParameter<TMongoModel>(property, value));
            else if (operatorValue == (int)MongoOperator.Existe)
                return builderFilter.Ne(property, BsonNull.Value);
            else if (operatorValue == (int)MongoOperator.Igual)
                return builderFilter.Eq(property, GetParameter<TMongoModel>(property, value));
            else if (operatorValue == (int)MongoOperator.Maior)
                return builderFilter.Gt(property, GetParameter<TMongoModel>(property, value));
            else if (operatorValue == (int)MongoOperator.MaiorIgual)
                return builderFilter.Gte(property, GetParameter<TMongoModel>(property, value));
            else if (operatorValue == (int)MongoOperator.Menor)
                return builderFilter.Lt(property, GetParameter<TMongoModel>(property, value));
            else if (operatorValue == (int)MongoOperator.MenorIgual)
                return builderFilter.Lte(property, GetParameter<TMongoModel>(property, value));
            else if (operatorValue == (int)MongoOperator.NaoExiste)
                return builderFilter.Eq(property, BsonNull.Value);
            else if (operatorValue == (int)MongoOperator.TerminaCom)
                return builderFilter.Regex(property, new MongoDB.Bson.BsonRegularExpression(string.Concat(value, "$"), "i"));
            else if (operatorValue == (int)MongoOperator.In)
            {
                var list = value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                return builderFilter.In(property, list);
            }
            else
                return builderFilter.Empty;
        }

        public FilterDefinition<TMongoModel> GetFilter<TMongoModel>(Expression<Func<TMongoModel, object>> property, int operatorValue, string value)
            where TMongoModel : class, IMongoModel, new()
        {
            var builderFilter = Builders<TMongoModel>.Filter;
            if (operatorValue == (int)MongoOperator.ComecaoCom)
                return builderFilter.Regex(property, new MongoDB.Bson.BsonRegularExpression(string.Concat("^", value), "i"));
            if (operatorValue == (int)MongoOperator.Contem)
                return builderFilter.Regex(property, new MongoDB.Bson.BsonRegularExpression(Convert.ToString(value), "i"));
            else if (operatorValue == (int)MongoOperator.Diferente)
                return builderFilter.Ne(property, GetParameter<TMongoModel>(property, value));
            else if (operatorValue == (int)MongoOperator.Existe)
                return builderFilter.Ne(property, BsonNull.Value);
            else if (operatorValue == (int)MongoOperator.Igual)
                return builderFilter.Eq(property, GetParameter<TMongoModel>(property, value));
            else if (operatorValue == (int)MongoOperator.Maior)
                return builderFilter.Gt(property, GetParameter<TMongoModel>(property, value));
            else if (operatorValue == (int)MongoOperator.MaiorIgual)
                return builderFilter.Gte(property, GetParameter<TMongoModel>(property, value));
            else if (operatorValue == (int)MongoOperator.Menor)
                return builderFilter.Lt(property, GetParameter<TMongoModel>(property, value));
            else if (operatorValue == (int)MongoOperator.MenorIgual)
                return builderFilter.Lte(property, GetParameter<TMongoModel>(property, value));
            else if (operatorValue == (int)MongoOperator.NaoExiste)
                return builderFilter.Eq(property, BsonNull.Value);
            else if (operatorValue == (int)MongoOperator.TerminaCom)
                return builderFilter.Regex(property, new MongoDB.Bson.BsonRegularExpression(string.Concat(value, "$"), "i"));
            else if (operatorValue == (int)MongoOperator.In)
            {
                var list = value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                return builderFilter.In(property, list);
            }
            else
                return builderFilter.Empty;
        }

        private object GetParameter<TMongoModel>(string property, string value)
            where TMongoModel : class, IMongoModel, new()
        {
            var modelType = typeof(TMongoModel);
            object parameter = null;

            object obj = value.Replace("\"", "") as object;

            try
            {
                Type propType = modelType.GetProperty(property).PropertyType;
                if (propType == typeof(bool))
                    obj = (value.Equals(Convert.ToString(true)) || value.Equals("1"));

                if (propType.GetGenericArguments() != null && propType.GetGenericArguments().Length > 0)
                    propType = propType.GetGenericArguments()[0];

                parameter = Convert.ChangeType(obj, propType);
            }
            catch
            {
                parameter = null;
            }

            return parameter;
        }

        private object GetParameter<TMongoModel>(Expression<Func<TMongoModel, object>> property, string value)
            where TMongoModel : class, IMongoModel, new()
        {
            var modelType = typeof(TMongoModel);
            object parameter = null;

            object obj = value.Replace("\"", "") as object;

            try
            {
                var propType = GetPropertyType(property);
                if (propType == typeof(bool))
                    obj = (value.Equals("1"));

                if (propType.GetGenericArguments() != null && propType.GetGenericArguments().Length > 0)
                    propType = propType.GetGenericArguments()[0];

                parameter = Convert.ChangeType(obj, propType);
            }
            catch
            {
                parameter = null;
            }

            return parameter;
        }

        private static Type GetPropertyType<T>(Expression<Func<T, object>> expr)
        {
            if ((expr.Body.NodeType == ExpressionType.Convert) ||
                (expr.Body.NodeType == ExpressionType.ConvertChecked))
            {
                var unary = expr.Body as UnaryExpression;
                if (unary != null)
                    return unary.Operand.Type;
            }
            return expr.Body.Type;
        }

        public AInBox.Astove.Core.Options.Filter GenerateFilter<TMongoModel>(IComponentContext container, IRequestFilter cmd, Type modelType, KeyValue parentId)
        {
            FilterOptions options = FilterFactory.GenerateFilterOptions(container, modelType, parentId, cmd);
            FilterCondition[] conditions = null;
            if (options != null)
                conditions = options.GetFilterConditions(cmd);

            return new AInBox.Astove.Core.Options.Filter { Options = options, Conditions = conditions };
        }

        public async Task SetDropDownOptions<TModel>(TModel model, string parentKey = null)
        {
            var list = new List<TModel> { model };
            await SetDropDownOptions<TModel>(list, parentKey);
        }

        public async Task SetDropDownOptions<TModel>(List<TModel> list, string parentKey = null)
        {
            if (list == null || list.Count == 0)
                return;

            var modelType = typeof(TModel);

            var properties = new List<PropertyInfo>();
            properties.AddRange(modelType.GetProperties().Where(p => p.PropertyType == typeof(DropDownOptions)).ToArray());
            properties.AddRange(modelType.GetProperties().Where(p => p.PropertyType == typeof(DropDownStringOptions)).ToArray());

            foreach (var propInfo in properties)
            {
                var attr = propInfo.GetCustomAttributes(true).OfType<ColumnDefinitionAttribute>().FirstOrDefault();
                if (propInfo.PropertyType == typeof(DropDownOptions) && attr != null && attr.EnumType != null)
                {
                    var dict = EnumUtility.GetEnumTexts(attr.EnumType);
                    var values = new List<KeyValue>();
                    foreach (var keyValue in dict)
                        values.Add(new KeyValue { Key = keyValue.Key, Value = keyValue.Value });
                    
                    foreach (var model in list)
                    {
                        var options = new DropDownOptions { DisplayText = !string.IsNullOrEmpty(attr.DisplayValue) ? attr.DisplayValue : "Key", DisplayValue = !string.IsNullOrEmpty(attr.DisplayText) ? attr.DisplayText : "Value", Items = values.ToArray() };
                        if (propInfo.GetValue(model, null) == null)
                        {
                            propInfo.SetValue(model, options, null);

                            var prop = model.GetType().GetProperty(attr.EntityProperty);
                            var propKV = model.GetType().GetProperty(string.Concat(propInfo.Name, "KV"));
                            if (prop != null)
                            {
                                var key = -1;
                                try
                                {
                                    key = Convert.ToInt32(prop.GetValue(model, null));
                                }
                                catch
                                {
                                    key = ((int?)prop.GetValue(model, null)).GetValueOrDefault(-1);
                                }
                                var kv = values.Where(p => p.Key == key).FirstOrDefault();
                                options.Selected = kv;

                                if (propKV != null)
                                    propKV.SetValue(model, kv);
                            }
                        }
                    }
                }
                else if (propInfo.PropertyType == typeof(DropDownStringOptions) && attr != null && !string.IsNullOrEmpty(attr.EntityName))
                {
                    var collectionName = string.Concat(attr.EntityName.ToLower(), "options");
                    if (!string.IsNullOrEmpty(attr.TableOptions))
                        collectionName = attr.TableOptions;

                    var col = this.MongoDatabase.GetCollection<KeyValueString>(collectionName);
                    var filterBuilder = Builders<KeyValueString>.Filter;

                    var filter = filterBuilder.Empty;
                    if (!string.IsNullOrEmpty(parentKey))
                    {
                        filter = filterBuilder.Eq(k => k.ParentId, parentKey);
                    }
                    else if (!string.IsNullOrEmpty(attr.ParentColumn))
                    {
                        var model = list.FirstOrDefault();
                        var parentProp = model.GetType().GetProperty(attr.ParentColumn);
                        var parentId = parentProp.GetValue(model, null);
                        if (parentId != null)
                            filter = filterBuilder.Eq(k => k.ParentId, Convert.ToString(parentId));
                    }

                    var values = await col.Find(filter).Sort(Builders<KeyValueString>.Sort.Ascending(o => o.Value)).ToListAsync();
                    
                    foreach (var model in list)
                    {
                        var options = new DropDownStringOptions { DisplayText = !string.IsNullOrEmpty(attr.DisplayValue) ? attr.DisplayValue : "Key", DisplayValue = !string.IsNullOrEmpty(attr.DisplayText) ? attr.DisplayText : "Value", Items = values.ToArray() };
                        if (propInfo.GetValue(model, null) == null)
                        {
                            propInfo.SetValue(model, options, null);

                            var prop = model.GetType().GetProperty(attr.EntityProperty);
                            var propKV = model.GetType().GetProperty(string.Concat(propInfo.Name, "KV"));
                            if (prop != null)
                            {
                                var key = Convert.ToString(prop.GetValue(model, null));
                                var kv = values.Where(p => p.Key.Equals(key)).FirstOrDefault();
                                options.Selected = kv;
                                if (propKV != null)
                                    propKV.SetValue(model, kv);
                            }
                        }
                    }
                }
            }
        }

        public string GetCollectionNameForOptions<TMongoModel>()
             where TMongoModel : class, IMongoModel, new()
        {
            var type = typeof(TMongoModel);
            string tableOptions = string.Concat(type.Name.Replace("MongoModel", string.Empty).ToLower(), "options");
            var attr = type.GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault();
            if (attr != null && !string.IsNullOrEmpty(attr.EntityName))
                tableOptions = string.Concat(attr.EntityName.ToLower(), "options");

            return tableOptions;
        }

        public string GetCollectionName<TMongoModel>()
             where TMongoModel : class, IMongoModel, new()
        {
            var type = typeof(TMongoModel);
            string table = type.Name.Replace("MongoModel", string.Empty).ToLower();
            var attr = type.GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault();
            if (attr != null && !string.IsNullOrEmpty(attr.EntityName))
                table = attr.EntityName.ToLower();

            return table;
        }
    }
}
