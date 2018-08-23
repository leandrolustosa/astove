using AInBox.Astove.Core.Attributes;
using AInBox.Astove.Core.Model;
using AInBox.Astove.Core.Options;
using AInBox.Astove.Core.Validations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Astove.BlurAdmin.Model.Domain;

namespace Astove.BlurAdmin.Model
{
    public class PostExpositorRegisterBindingModel : IBindingModel
    {
        [Required]
        [EnumValidation(typeof(Domain.TipoEmpresa))]
        [Display(Name = "tipo de acesso")]
        public int TipoAcesso { get; set; }

        [Required]
        [StringLength(18, ErrorMessage = "O {0} deve conter no máximo {1} caracteres.")]
        [Display(Name = "CNPJ")]
        public string CNPJ { get; set; }

        [Required]
        [StringLength(300, ErrorMessage = "O {0} deve conter no máximo {1} caracteres.")]
        [Display(Name = "nome fantasia")]
        public string NomeFantasia { get; set; }

        [StringLength(300, ErrorMessage = "O {0} deve conter no máximo {1} caracteres.")]
        [Display(Name = "razão social")]
        public string RazaoSocial { get; set; }

        [StringLength(14, ErrorMessage = "O {0} deve conter no máximo {1} caracteres.")]
        [Display(Name = "CPF")]
        public string CPF { get; set; }

        [Required]
        [Display(Name = "nome")]
        [StringLength(50, ErrorMessage = "O {0} deve conter no máximo {1} caracteres.")]
        public string Nome { get; set; }

        [Required]
        [Display(Name = "CEP")]
        [StringLength(10, ErrorMessage = "O {0} deve conter no máximo {1} caracteres.")]
        public string CEP { get; set; }

        [Required]
        [Display(Name = "cidade")]
        [StringLength(50, ErrorMessage = "A {0} deve conter no máximo {1} caracteres.")]
        public string Cidade { get; set; }

        [Required]
        [Display(Name = "sigla do estado (UF)")]
        [StringLength(2, ErrorMessage = "A {0} deve conter no máximo {1} caracteres.")]
        public string SiglaUF { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "O {0} deve conter no máximo {1} caracteres.")]
        [Display(Name = "celular")]
        public string Celular { get; set; }

        [Required]
        [Display(Name = "e-mail")]
        [StringLength(50, ErrorMessage = "O {0} deve conter no máximo {1} caracteres.")]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "A {0} deve conter no máximo {1} caracteres.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "senha")]
        public string Senha { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "confirme a sua senha")]
        [Compare("Password", ErrorMessage = "A nova senha e a senha de confirmação não coincidem.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "estande")]
        [MaxLength(20)]
        [DataType(DataType.Text)]
        public string Estande { get; set; }
        [DataType(DataType.Text)]
        [Display(Name = "metragem")]
        public int? Metragem { get; set; }
        [DataType(DataType.Text)]
        [MaxLength(40)]
        [Display(Name = "locacalização")]
        public string Localizacao { get; set; }
    }

    public class PessoaRegisterBindingModel : IBindingModel
    {
        [Required]
        [EnumValidation(typeof(Domain.TipoEmpresa))]
        [Display(Name = "tipo de acesso")]
        public int Tipo { get; set; }

        [Required]
        [StringLength(18, ErrorMessage = "O {0} deve conter no máximo {1} caracteres.")]
        [Display(Name = "CNPJ")]
        public string CNPJ { get; set; }

        [Required]
        [StringLength(300, ErrorMessage = "O {0} deve conter no máximo {1} caracteres.")]
        [Display(Name = "nome da empresa")]
        public string EmpresaNome { get; set; }

        [StringLength(300, ErrorMessage = "O {0} deve conter no máximo {1} caracteres.")]
        [Display(Name = "razão social")]
        public string RazaoSocial { get; set; }

        [Required]
        [StringLength(14, ErrorMessage = "O {0} deve conter no máximo {1} caracteres.")]
        [Display(Name = "CPF")]
        public string CPF { get; set; }

        [Required]
        [Display(Name = "nome")]
        [StringLength(50, ErrorMessage = "O {0} deve conter no máximo {1} caracteres.")]
        public string Nome { get; set; }

        [Required]
        [Display(Name = "CEP")]
        [StringLength(10, ErrorMessage = "O {0} deve conter no máximo {1} caracteres.")]
        public string CEP { get; set; }

        [Required]
        [Display(Name = "cidade")]
        [StringLength(50, ErrorMessage = "A {0} deve conter no máximo {1} caracteres.")]
        public string Cidade { get; set; }

        [Required]
        [Display(Name = "sigla do estado (UF)")]
        [StringLength(2, ErrorMessage = "A {0} deve conter no máximo {1} caracteres.")]
        public string SiglaUF { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "O {0} deve conter no máximo {1} caracteres.")]
        [Display(Name = "celular")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "e-mail")]
        [StringLength(50, ErrorMessage = "O {0} deve conter no máximo {1} caracteres.")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "A {0} deve conter no máximo {1} caracteres.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "senha")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "confirme a sua senha")]
        [Compare("Password", ErrorMessage = "A nova senha e a senha de confirmação não coincidem.")]
        public string ConfirmPassword { get; set; }
    }

    public class PessoaChangeProfileBindingModel : IBindingModel
    {
        [Required]
        [StringLength(15, ErrorMessage = "O {0} deve conter no máximo {1} caracteres.")]
        [Display(Name = "Celular")]
        [DataType(DataType.Text)]
        [Mask("(99)99999-9999")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(18, ErrorMessage = "O {0} deve conter no máximo {1} caracteres.")]
        [Display(Name = "CNPJ")]
        [DataType(DataType.Text)]
        [Mask("99.999.999/9999-99")]
        public string CNPJ { get; set; }

        [Required]
        [StringLength(300, ErrorMessage = "O {0} deve conter no máximo {1} caracteres.")]
        [Display(Name = "Nome Fantasia")]
        [DataType(DataType.Text)]
        public string EmpresaNome { get; set; }

        [StringLength(300, ErrorMessage = "O {0} deve conter no máximo {1} caracteres.")]
        [Display(Name = "Razão Social")]
        [DataType(DataType.Text)]
        public string RazaoSocial { get; set; }

        [Required]
        [StringLength(14, ErrorMessage = "O {0} deve conter no máximo {1} caracteres.")]
        [Display(Name = "CPF")]
        [DataType(DataType.Text)]
        [Mask("999.999.999-99")]
        public string CPF { get; set; }

        [Required]
        [Display(Name = "Nome")]
        [StringLength(50, ErrorMessage = "O {0} deve conter no máximo {1} caracteres.")]
        [DataType(DataType.Text)]
        public string Nome { get; set; }

        [Required]
        [Display(Name = "CEP")]
        [StringLength(10, ErrorMessage = "O {0} deve conter no máximo {1} caracteres.")]
        [DataType(DataType.Text)]
        [Mask("99999-999")]
        public string CEP { get; set; }

        [Required]
        [Display(Name = "Cidade")]
        [StringLength(50, ErrorMessage = "A {0} deve conter no máximo {1} caracteres.")]
        [DataType(DataType.Text)]
        public string Cidade { get; set; }

        [Required]
        [Display(Name = "UF")]
        [StringLength(2, ErrorMessage = "A {0} deve conter no máximo {1} caracteres.")]
        [DataType(DataType.Text)]
        public string SiglaUF { get; set; }
    }

    [DataEntity(GroupName = "Usuários")]
    public class PutPessoaChangeProfileBindingModel : PessoaChangeProfileBindingModel, IBindingModel
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }
        public int ProfileId { get; set; }
        public bool PermiteAlterarSenha { get; set; }

        [CssClass("col-md-4")]
        [Display(Name = "e-mail")]
        [DataType(DataType.Text)]
        [Required]
        public string Email { get; set; }

        public string NewEmail { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Senha atual")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nova senha")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirme a sua senha")]
        [Compare("NewPassword", ErrorMessage = "A nova senha e a senha de confirmação não coincidem.")]
        public string ConfirmPassword { get; set; }

        [CssClass("col-md-3")]
        [Display(Name = "Cargo")]
        [DataType(DataType.Text)]
        [StringLength(50)]
        public string Cargo { get; set; }

        public string FotoUrl { get; set; }
        public string FotoUrlSource { get; set; }
    }

    public class PessoaChangeConfigurationBindingModel : IBindingModel
    {
        public bool AtivarNotificacaoPush { get; set; }

        public bool AtivarNotificacaoEmail { get; set; }

        [Required]
        [Display(Name = "sigla do estado (UF)")]
        [StringLength(30, ErrorMessage = "A {0} deve conter no máximo {1} caracteres.")]
        public string Regiao { get; set; }
    }

    public class ForgotPasswordBindingModel : IBindingModel
    {
        [Required]
        [Display(Name = "e-mail")]
        public string Email { get; set; }
    }

    public class ResetPasswordBindingModel : BaseBindingModel
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [Display(Name = "nova senha")]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "confirme a sua senha")]
        [Compare(nameof(NewPassword), ErrorMessage = "Por favor, a nova senha e a senha de confirmação devem ser iguais")]
        public string ConfirmPassword { get; set; }
    }

    public class PostRegisterTokenBinding : IBindingModel
    {
        [Required]
        public DateTime DataRegistro { get; set; }

        [Display(Name = "plataforma")]
        [EnumValidation(typeof(Plataforma), ErrorMessage = "O valor informado para {0} não corresponde a nenhum valor válido")]
        public int Plataforma { get; set; }

        [Display(Name = "identificador do aparelho")]
        [Required]
        public string UniqueID { get; set; }

        [Display(Name = "nome")]
        [Required]
        public string Nome { get; set; }

        [Display(Name = "so")]
        [Required]
        public string SO { get; set; }

        [Display(Name = "token")]
        [Required]
        public string Token { get; set; }
    }

    public class BaseProfileBindingModel : BaseMongoModel, IBindingModel
    {
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
        [CssClass("col-md-4")]
        [Display(Name = "sobrenome")]
        [DataType(DataType.Text)]
        [StringLength(50)]
        [Required]
        public string Sobrenome { get; set; }
        [CssClass("col-md-3")]
        [Display(Name = "telefone")]
        [DataType(DataType.Text)]
        [Mask("(99)99999-9999")]
        public string Telefone { get; set; }
        [CssClass("col-md-3")]
        [Display(Name = "cargo")]
        [DataType(DataType.Text)]
        [StringLength(50)]
        public string Funcao { get; set; }
        [CssClass("col-md-3")]
        [Display(Name = "data de nascimento")]
        [DataType(DataType.Date)]
        public DateTime? DataNascimento { get; set; }
        [CssClass("col-md-3")]
        [Display(Name = "ativo")]
        [DataType(DataType.Text)]
        public bool Ativo { get; set; }
        public string FotoUrl { get; set; }
        public string FotoUrlSource { get; set; }
        public string[] CodigoEmpresas { get; set; }
        [DataType(DataType.Text)]
        [CssClass("col-md-6")]
        [ColumnDefinition(SelectMessage = "selecione uma empresa", EntityProperty = "empresas")]
        public DropDownStringOptions EmpresaOptions { get; set; }
        public string[] Permissoes { get; set; }
        [DataType(DataType.Text)]
        [CssClass("col-md-6")]
        [ColumnDefinition(SelectMessage = "selecione uma permissão", EntityProperty = "permissoes")]
        public DropDownStringOptions PermissaoOptions { get; set; }
    }

    [DataEntity(GroupName = "Usuários")]
    public class PostProfileBindingModel : BaseProfileBindingModel, IBindingModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "A {0} deve conter no máximo {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirme a sua senha")]
        [Compare("Password", ErrorMessage = "A nova senha e a senha de confirmação não coincidem.")]
        public string ConfirmPassword { get; set; }
    }

    [DataEntity(GroupName = "Usuários")]
    public class PutProfileBindingModel : BaseProfileBindingModel, IBindingModel
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }
        public int ProfileId { get; set; }
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
