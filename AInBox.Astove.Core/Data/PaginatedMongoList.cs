using System;
using System.Collections.Generic;
using System.Linq;
using AInBox.Astove.Core.Model;
using AInBox.Astove.Core.Attributes;
using AInBox.Astove.Core.Options;
using WebApiDoodle.Net.Http.Client.Model;
using AInBox.Astove.Core.Extensions;
using MongoDB.Driver;
using Autofac;

namespace AInBox.Astove.Core.Data
{
    public class PaginatedMongoList<TModel, RModel> : List<RModel>
        where TModel : IMongoModel
        where RModel : class, IMongoModel, IDto, new()
    {
        public IComponentContext Container { get; set; }

        public FilterOptions FilterOptions { get; set; }
        public FilterCondition[] FilterConditions { get; set; }

        public SortOptions SortOptions { get; set; }

        public KeyValue ParentId { get; set; }

        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPageCount { get; private set; }
        
        private IFindFluent<TModel, TModel> Source { get; set; }

        public PaginatedMongoList(IComponentContext container, FilterOptions options, FilterCondition[] conditions, SortOptions sortOptions, KeyValue parentId, int pageIndex, int pageSize, int totalCount, IEnumerable<RModel> paginatedData, IFindFluent<TModel, TModel> source)
        {
            try
            {
                AddRange(paginatedData);

                Source = source;
                Container = container;
                ParentId = parentId;
                FilterOptions = options;
                FilterConditions = conditions;
                SortOptions = sortOptions;
                PageIndex = pageIndex;
                PageSize = pageSize;
                TotalCount = totalCount;
                TotalPageCount = (int)Math.Ceiling(totalCount / (double)pageSize);
            }
            catch
            {
                this.Clear();

                Source = source;
                Container = container;
                ParentId = parentId;
                FilterOptions = options;
                FilterConditions = conditions;
                SortOptions = sortOptions;
                PageIndex = pageIndex;
                PageSize = pageSize;
                TotalCount = totalCount;
                TotalPageCount = (int)Math.Ceiling(totalCount / (double)pageSize);
            }
        }

        public bool HasPreviousPage
        {
            get { return (PageIndex > 1); }
        }

        public bool HasNextPage
        {
            get { return (PageIndex < TotalPageCount); }
        }

        public PaginatedMongoModel<T> ToPaginatedModel<T>()
            where T : class, IMongoModel, IDto, new()
        {
            var attr = typeof(TModel).GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault();

            var paginatedModel = new PaginatedMongoModel<T>
            {
                ControllerName = (attr != null && !string.IsNullOrEmpty(attr.BaseUriTemplate)) ? attr.BaseUriTemplate.Replace("api/v1/", "") : string.Empty,
                ParentId = this.ParentId,
                PagingOptions = new PagingOptions
                {
                    PageIndex = this.PageIndex,
                    PageSize = this.PageSize,
                    PageSizes = (attr != null) ? attr.PageSizes : DataEntityAttribute.DefaultPageSizes,
                    TotalCount = this.TotalCount,
                    TotalPageCount = this.TotalPageCount,
                    HasNextPage = this.HasNextPage,
                    HasPreviousPage = this.HasPreviousPage
                },
                EnablePaging = true,
                Items = this.ToList().Select(c => c.CreateInstanceOf<T>()).ToList(),
                FilterOptions = this.FilterOptions,
                FilterConditions = this.FilterConditions,
                SortOptions = this.SortOptions
            };

            return paginatedModel;
        }

        public PaginatedMongoModel<RModel> ToPaginatedModel()
        {
            var attr = typeof(TModel).GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault();

            var paginatedModel = new PaginatedMongoModel<RModel>
            {
                ControllerName = (attr != null && !string.IsNullOrEmpty(attr.BaseUriTemplate)) ? attr.BaseUriTemplate.Replace("api/v1/", "") : string.Empty,
                ParentId = this.ParentId,
                PagingOptions = new PagingOptions
                {
                    PageIndex = this.PageIndex,
                    PageSize = this.PageSize,
                    PageSizes = (attr != null) ? attr.PageSizes : DataEntityAttribute.DefaultPageSizes,
                    TotalCount = this.TotalCount,
                    TotalPageCount = this.TotalPageCount,
                    HasNextPage = this.HasNextPage,
                    HasPreviousPage = this.HasPreviousPage
                },
                EnablePaging = true,
                Items = this.ToList(),
                FilterOptions = this.FilterOptions,
                FilterConditions = this.FilterConditions,
                SortOptions = this.SortOptions
            };

            return paginatedModel;
        }
    }
}