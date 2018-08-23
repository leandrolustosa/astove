using AInBox.Astove.Core.Attributes;
using AInBox.Astove.Core.Model;
using AInBox.Astove.Core.Options;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Astove.BlurAdmin.Model
{
    public class ProfileMongoModel : IMongoModel
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }
        [JsonIgnore]
        public string ParentId { get; set; }
        [JsonIgnore]
        public int UsuarioId { get; set; }
        public string CPF { get; set; }
        public int EmpresaId { get; set; }
        [ColumnDefinition(EntityProperty = "Empresa.Nome")]
        public string EmpresaNome { get; set; }
        [ColumnDefinition(EntityProperty = "Empresa.Tipo")]
        public int EmpresaTipo { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Nome { get; set; }
        public string Cargo { get; set; }
        public string ImagemUrl { get; set; }
        public string[] Permissoes { get; set; }
    }

    public class BasePessoaBindingModel : IBindingModel, IMongoModel
    {
        public string ParentId { get; set; }
        [CssClass("col-md-3")]
        [Display(Name = "CPF")]
        [DataType(DataType.Text)]
        [Required]
        [Mask("999.999.999-99")]
        public string CPF { get; set; }
        public int EmpresaId { get; set; }
        public string EmpresaNome { get; set; }
        [CssClass("col-md-4")]
        [Display(Name = "e-mail")]
        [DataType(DataType.Text)]
        [Required]
        [ColumnDefinition(EntityProperty = "empresaId", TableOptions = "empresaoptions")]
        public DropDownOptions Empresa { get; set; }
        public KeyValueString EmpresaKV { get; set; }

        [CssClass("col-md-4")]
        [Display(Name = "e-mail")]
        [DataType(DataType.Text)]
        [Required]
        public string Email { get; set; }
        [CssClass("col-md-4")]
        [Display(Name = "nome")]
        [DataType(DataType.Text)]
        [StringLength(50)]
        [Required]
        public string Nome { get; set; }
        
        [CssClass("col-md-3")]
        [Display(Name = "telefone")]
        [DataType(DataType.Text)]
        [Mask("(99)99999-9999")]
        public string Telefone { get; set; }
        [CssClass("col-md-3")]
        [Display(Name = "cargo")]
        [DataType(DataType.Text)]
        [StringLength(50)]
        public string Cargo { get; set; }
                
        
        public string[] Permissoes { get; set; }
        [DataType(DataType.Text)]
        [CssClass("col-md-6")]
        [ColumnDefinition(SelectMessage = "selecione uma permissão", EntityProperty = "permissoes")]
        public DropDownStringOptions PermissaoOptions { get; set; }
    }

    public class PutPessoaBindingModel : BasePessoaBindingModel
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }
        
        public bool PermiteAlterarSenha { get; set; }

        public string NewEmail { get; set; }

        [StringLength(100, ErrorMessage = "A {0} deve conter no máximo {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha atual")]
        public string Password { get; set; }

        [StringLength(100, ErrorMessage = "A {0} deve conter no máximo {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nova senha")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirme a sua senha")]
        [Compare("NewPassword", ErrorMessage = "A nova senha e a senha de confirmação não coincidem.")]
        public string ConfirmPassword { get; set; }
    }
}
