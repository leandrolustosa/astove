using AInBox.Astove.Core.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace Astove.BlurAdmin.Data
{
    [Table("cidade")]
    public class Cidade : BaseEntityAudit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }
        [Required]
        public int EstadoId { get; set; }
        [ForeignKey("EstadoId")]
        public Estado Estado { get; set; }
        [Required]
        [MaxLength(50)]
        public string Nome { get; set; }
        [Required]
        public int CodigoIBGE { get; set; }
        [Required]
        [MaxLength(6)]
        public string DDD { get; set; }

        [NotMapped]
        public static Expression<Func<Cidade, object>>[] Includes
        {
            get { return new Expression<Func<Cidade, object>>[] { e => e.Estado }; }
        }
    }
}
