using AInBox.Astove.Core.Attributes;
using AInBox.Astove.Core.Model;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System.ComponentModel.DataAnnotations;

namespace Astove.BlurAdmin.Model
{
    [DataEntity(EntityName = "Estado", KeyColumn = "Id", ValueColumn = "Sigla", ParentColumn = "ParentId")]
    public class EstadoMongoModel : BaseMongoModel
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Nome { get; set; }
        [Required]
        [MaxLength(2)]
        public string Sigla { get; set; }
        [Required]
        [MaxLength(30)]
        public string Capital { get; set; }
        [Required]
        [MaxLength(30)]
        public string Regiao { get; set; }
    }
}
