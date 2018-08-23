using AInBox.Astove.Core.Data;
using AInBox.Astove.Core.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace Astove.BlurAdmin.Data
{
    [Table("pessoa")]
    public class Pessoa : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }
        [Required]
        [Minimum(1)]
        public int EmpresaId { get; set; }
        [ForeignKey("EmpresaId")]
        public Empresa Empresa { get; set; }
        [Required]
        [MaxLength(15)]
        public string CPF { get; set; }
        [Required]
        [MaxLength(50)]
        public string Nome { get; set; }
        [Required]
        [MaxLength(50)]
        public string Email { get; set; }
        [Required]
        public int UsuarioId { get; set; }

        [MaxLength(30)]
        public string Cargo { get; set; }
        [MaxLength(14)]
        public string Telefone { get; set; }

        [Column(TypeName = "text")]
        public string ImagemUrl { get; set; }
        
        public ICollection<Configuracao> Configuracoes { get; set; }

        [NotMapped]
        public static Expression<Func<Pessoa, object>>[] Includes
        {
            get { return new Expression<Func<Pessoa, object>>[] { e => e.Empresa.Cidade.Estado }; }
        }
    }
}
