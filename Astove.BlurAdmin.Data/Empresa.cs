using AInBox.Astove.Core.Data;
using AInBox.Astove.Core.Validations;
using Astove.BlurAdmin.Model.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace Astove.BlurAdmin.Data
{
    [Table("empresa")]
    public class Empresa : BaseEntityAudit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }
        [Required]
        [EnumValidation(typeof(TipoEmpresa))]
        public int Tipo { get; set; }
        [Required]
        [MaxLength(20)]
        public string CNPJ { get; set; }
        [Required]
        [MaxLength(300)]
        public string NomeFantasia { get; set; }
        [MaxLength(300)]
        public string RazaoSocial { get; set; }
        [Required]
        [MaxLength(10)]
        public string CEP { get; set; }
        [Required]
        [Minimum(1)]
        public int CidadeId { get; set; }
        [ForeignKey("CidadeId")]
        public Cidade Cidade { get; set; }

        [MaxLength(15)]
        public string Modulo { get; set; }

        [MaxLength(300)]
        public string Logradouro { get; set; }
        [MaxLength(10)]
        public string Numero { get; set; }
        [MaxLength(30)]
        public string Complemento { get; set; }
        [MaxLength(50)]
        public string Bairro { get; set; }
        [MaxLength(20)]
        public string InscricaoEstadual { get; set; }
        [MaxLength(20)]
        public string InscricaoMunicipal { get; set; }

        [InverseProperty("Empresa")]
        public ICollection<Pessoa> Pessoas { get; set; }

        [NotMapped]
        public static Expression<Func<Empresa, object>>[] Includes
        {
            get { return new Expression<Func<Empresa, object>>[] { e => e.Cidade.Estado }; }
        }
    }
}
