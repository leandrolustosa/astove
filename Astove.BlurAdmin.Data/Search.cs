using AInBox.Astove.Core.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace Astove.BlurAdmin.Data
{
    [Table("search")]
    public class Search : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }
        [Required]
        [StringLength(300)]
        public string Name { get; set; }
        [Required]
        [Column(TypeName = "text")]
        public string Text { get; set; }
        [Required]
        [Column(TypeName="text")]
        public string Route { get; set; }
        [Column(TypeName = "text")]
        public string GlobalParameters { get; set; }
        [StringLength(50)]
        public string Permission { get; set; }
        public DateTime? DateOfCreation { get; set; }
        public int? PessoaId { get; set; }
        [ForeignKey("PessoaId")]
        public Pessoa Pessoa { get; set; }

        [NotMapped]
        public static Expression<Func<Search, object>>[] Includes
        {
            get { return new Expression<Func<Search, object>>[] { e => e.Pessoa.Empresa.Cidade.Estado }; }
        }
    }
}
