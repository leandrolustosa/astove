using AInBox.Astove.Core.Model;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApiDoodle.Net.Http.Client.Model;

namespace Astove.BlurAdmin.Model
{
    public class PostSearchBindingModel
    {
        [Required]
        [StringLength(300)]
        public string Name { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public string Route { get; set; }
        public string GlobalParameters { get; set; }
        [StringLength(50)]
        public string Permission { get; set; }
        public DateTime? DateOfCreation { get; set; }
        public int ProfileId { get; set; }
    }

    public class SearchMongoModel : IMongoModel
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string Route { get; set; }
        public string GlobalParameters { get; set; }
        public string Permission { get; set; }
        public DateTime? DateOfCreation { get; set; }
        public int? ProfileId { get; set; }
    }

    public class ListaSearchMongoModel : IMongoModel, IDto
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public string Route { get; set; }        
    }

    public class ListaSearchResultModel : BaseResultModel
    {
        public List<ListaSearchMongoModel> Items { get; set; }
    }
}
