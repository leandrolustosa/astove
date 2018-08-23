using System;
using System.Collections.Generic;
using System.Linq;
using AInBox.Astove.Core.Data;
using AInBox.Astove.Core.Options;
using Autofac;
using AInBox.Astove.Core.Model;
using System.Threading.Tasks;
using MongoDB.Driver;
using WebApiDoodle.Net.Http.Client.Model;

namespace AInBox.Astove.Core.Extensions
{
    public static class IFindFluentExtensions
    {
        public async static Task<PaginatedMongoList<TModel, RModel>> ToPaginatedMongoListAsync<TModel, RModel>(this IFindFluent<TModel, TModel> source, IComponentContext container, AInBox.Astove.Core.Options.Filter filter, SortOptions sortOptions, KeyValue parentId, int pageIndex, int pageSize)
            where TModel : class, IMongoModel, new()
            where RModel : class, IMongoModel, IDto, new()
        {
            FilterOptions options = null;
            FilterCondition[] conditions = null;
            if (filter != null && (filter.Options != null || filter.Conditions != null))
            {
                options = new FilterOptions { Filters = filter.Options.Filters.Where(f => f.Internal == false).ToList() };
                conditions = filter.Conditions.Where(c => c.Internal == false).ToArray();
            }

            int totalCount = (int)await source.CountAsync();
            int totalPageCount = (int)Math.Ceiling(totalCount / (double)pageSize);
            if ((pageIndex - 1) > totalPageCount)
                pageIndex = totalPageCount;

            var paginatedData = await source.Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();

            var returnData = new List<RModel>();
            paginatedData.ForEach(p => returnData.Add(p.CreateInstanceOf<RModel>()));
            

            return new PaginatedMongoList<TModel, RModel>(container, options, conditions, sortOptions, parentId, pageIndex, pageSize, totalCount, returnData, source);
        }
    }
}
