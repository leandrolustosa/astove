using AInBox.Astove.Core.Extensions;
using Astove.BlurAdmin.Data;
using Astove.BlurAdmin.Model;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using AInBox.Astove.Core.Service;
using System.Data.Entity;
using AInBox.Astove.Core.Model;
using System.Text;

namespace Astove.BlurAdmin.Services
{
    public static class SearchService
    {        
        public static async Task AddSearchBindingModel(this IEntityService<Search> service, PostSearchBindingModel model)
        {
            var entity = model.CreateInstanceOf<Search>();
            await service.AddAsync(entity);

            var objMongo = model.CreateInstanceOf<SearchMongoModel>();
            objMongo.ParentId = entity.Id.ToString();
            var idMongo = await service.MongoService.InsertMongoObject<SearchMongoModel>(objMongo);
        }

        public static async Task DeleteSearch(this IEntityService<Search> service, SearchMongoModel model)
        {
            await service.MongoService.DeleteMongoObject<SearchMongoModel>(model.Id);
            var searchId = int.Parse(model.ParentId);
            await service.DeleteAsync(searchId);
        }

        public async static Task AtualizarSearchs(this IEntityService<Search> service)
        {
            var col = service.MongoDatabase.GetCollection<SearchMongoModel>(service.MongoService.GetCollectionName<SearchMongoModel>());
            await col.DeleteManyAsync(Builders<SearchMongoModel>.Filter.Empty);

            var list = await service.Where(o => true).ToListAsync();
            var mongoList = new List<SearchMongoModel>();
            foreach (var obj in list)
            {
                var objMongo = obj.CreateInstanceOf<SearchMongoModel>();
                mongoList.Add(objMongo);
            }

            if (list.Count > 0)
            {
                await col.InsertManyAsync(mongoList);

                var indexTextByText = Builders<SearchMongoModel>.IndexKeys.Text(p => p.Text);
                var options = new CreateIndexOptions();
                options.DefaultLanguage = "portuguese";

                await col.Indexes.CreateOneAsync(indexTextByText, options);
            }
        }

        public async static Task<ListaSearchResultModel> GetListaSearchResultModel(this IEntityService<Search> service, IList<string> roles, string text)
        {
            var arg = text.RemoveAccent();
            var builderFilter = Builders<SearchMongoModel>.Filter;
            var filters = new List<FilterDefinition<SearchMongoModel>>();
            filters.Add(builderFilter.Regex(p => p.Text, new MongoDB.Bson.BsonRegularExpression(string.Concat(arg), "i")));
            filters.Add(builderFilter.In(p => p.Permission, roles));

            var builderSort = Builders<SearchMongoModel>.Sort;
            var sorts = new List<SortDefinition<SearchMongoModel>>();
            sorts.Add(builderSort.Ascending(p => p.Name));

            var list = await service.MongoService.GetMongoListAsync<SearchMongoModel>(filters, sorts);
            var items = new List<ListaSearchMongoModel>();
            list.ForEach(p => items.Add(p.CreateInstanceOf<ListaSearchMongoModel>()));

            var result = new ListaSearchResultModel { IsValid = true, Message = string.Empty, Items = items };
            return result;
        }

        public async static Task<BaseResultModel> ReloadMongoCollection(this IEntityService<Search> service, StringBuilder sb = null)
        {
            return await service.ReloadMongoCollection<SearchMongoModel>(true, sb, Search.Includes);
        }
    }
}
