using AInBox.Astove.Core.Data;
using System;
using System.ComponentModel.DataAnnotations;

namespace AInBox.Astove.Core.Data
{
    public class BaseEntityAudit : BaseEntity
    {
        public override int Id { get; set; }
        [Required]
        public int UsuarioCriacaoId { get; set; }
        [Required]
        public DateTime DataCriacao { get; set; }
        public int? UsuarioAlteracaoId { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}