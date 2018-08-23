using AInBox.Astove.Core.Attributes;
using AInBox.Astove.Core.Model;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System.ComponentModel.DataAnnotations;

namespace Astove.BlurAdmin.Model
{
    [DataEntity(EntityName = "Cidade", KeyColumn = "Id", ValueColumn = "Nome", ParentColumn = "EstadoId")]
    public class CidadeMongoModel : BaseMongoModel
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }
        [Required]
        public int EstadoId { get; set; }
        [ColumnDefinition(EntityProperty = "Estado.Sigla")]
        public string EstadoSigla { get; set; }
        [ColumnDefinition(EntityProperty = "Estado", Load = true)]
        public EstadoMongoModel Estado { get; set; }
        [Required]
        [MaxLength(50)]
        public string Nome { get; set; }
        [Required]
        public int CodigoIBGE { get; set; }
        [Required]
        [MaxLength(6)]
        public string DDD { get; set; }
    }
}
