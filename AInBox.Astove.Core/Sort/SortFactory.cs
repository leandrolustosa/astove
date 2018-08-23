using AInBox.Astove.Core.Model;
using AInBox.Astove.Core.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AInBox.Astove.Core.Sort
{
    public static class SortFactory
    {
        public static SortOptions GenerateSortOptions(PaginatedRequestCommand model)
        {
            var sortOptions = new SortOptions
            {
                Fields = (model.OrdersBy == null || model.OrdersBy.Length == 0) ? new[] { "Id" } : model.OrdersBy,
                Directions = (model.Directions == null || model.Directions.Length == 0) ? new[] { "asc" } : model.Directions
            };
            return sortOptions;
        } 
    }
}
