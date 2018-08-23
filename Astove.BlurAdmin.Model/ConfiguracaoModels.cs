using AInBox.Astove.Core.Attributes;
using AInBox.Astove.Core.Model;
using AInBox.Astove.Core.Options;
using AInBox.Astove.Core.Options.EnumDomainValues;
using AInBox.Astove.Core.Validations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.ComponentModel.DataAnnotations;

namespace Astove.BlurAdmin.Model
{
    [DataEntity(EntityName = "Configuracao", KeyColumn = "Id", ParentColumn = "ParentId")]
    public class ConfiguracaoMongoModel : BaseMongoModel
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }


    }

    public class ListaConfiguracaoMongoModel : BaseMongoModel
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }


    }

    public class BaseConfiguracaoBindingModel : IBindingModel
    {

    }

    [DataEntity(GroupName = "Configurações")]
    public class PostConfiguracaoBindingModel : BaseConfiguracaoBindingModel
    {

    }

    [DataEntity(GroupName = "Configurações")]
    public class PutConfiguracaoBindingModel : BaseConfiguracaoBindingModel, IMongoModel
    {
        [Required]
        public string Id { get; set; }
        public string ParentId { get; set; }
    }
}