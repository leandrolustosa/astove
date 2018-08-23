using AInBox.Astove.Core.Data;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace Astove.BlurAdmin.Data
{
    [Table("estado")]
    public class Estado : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }
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
        [Required]
        [DefaultValue("21")]
        public string FaixaDDD { get; set; }

        [NotMapped]
        public static Expression<Func<Estado, object>>[] Includes
        {
            get { return null; }
        }
    }
}
