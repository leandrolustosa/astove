using AInBox.Astove.Core.Attributes;
using AInBox.Astove.Core.Model;
using AInBox.Astove.Core.Options;
using AInBox.Astove.Core.Validations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System.ComponentModel.DataAnnotations;

namespace Astove.BlurAdmin.Model
{
    [DataEntity(KeyColumn = "Id", ValueColumn = "Nome", ParentColumn = "TipoAcesso")]
    public class BaseEmpresaMongoModel : BaseMongoModel
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }
        public string NomeFantasia { get; set; }
        public string CEP { get; set; }
        public int CidadeId { get; set; }
        [ColumnDefinition(EntityProperty = "Cidade", Load = true)]
        public CidadeMongoModel Cidade { get; set; }
        [ColumnDefinition(EntityProperty = "Cidade.Estado.Regiao")]
        public string CidadeEstadoRegiao { get; set; }
        [ColumnDefinition(EntityProperty = "Cidade.Estado.Sigla")]
        public string CidadeSiglaUF { get; set; }
        [ColumnDefinition(EntityProperty = "Cidade.EstadoId")]
        public int EstadoId { get; set; }
        [MaxLength(300)]
        public string RazaoSocial { get; set; }
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
    }

    [DataEntity(KeyColumn = "Id", ValueColumn = "Nome", ParentColumn = "TipoAcesso")]
    public class EmpresaOrganizacaoMongoModel : BaseEmpresaMongoModel
    {
        [FilterDecimal(Internal = true, DefaultOperator = (int)MongoOperator.Igual, DefaultValue = (int)Domain.TipoEmpresa.Organizacao)]
        public int Tipo { get; set; }
        public string CNPJ { get; set; }
    }

    [DataEntity(KeyColumn = "Id", ValueColumn = "Nome", ParentColumn = "TipoAcesso")]
    public class EmpresaClienteMongoModel : BaseEmpresaMongoModel
    {
        public int Tipo { get; set; }
        public string CNPJ { get; set; }
    }

    public class ListaEmpresaMongoModel : BaseMongoModel
    {
        public string Id { get; set; }
        public int Tipo { get; set; }
        [FilterLike(Mask = "99.999.999/9999-99")]
        public string CNPJ { get; set; }
        [FilterLike(DisplayName = "Nome Fantasia")]
        public string NomeFantasia { get; set; }
        [FilterLike(DisplayName = "Razão Social")]
        public string RazaoSocial { get; set; }
        public string InscricaoEstadual { get; set; }
        public string InscricaoMunicipal { get; set; }
        public string CEP { get; set; }
        [FilterLike(DisplayName = "Cidade")]
        [ColumnDefinition(EntityProperty = "Cidade.Nome")]
        public string CidadeNome { get; set; }        
        [FilterLike(DisplayName = "UF")]
        [ColumnDefinition(EntityProperty = "Cidade.Estado.Sigla")]
        public string SiglaUF { get; set; }

        public string Modulo { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
    }

    public class BaseEmpresaBindingModel : IBindingModel
    {
        [Required]
        public int Tipo { get; set; }

        [DataType(DataType.Text)]
        [CssClass("col-md-3")]
        [Display(Name = "Tipo da Empresa")]
        [ColumnDefinition(EntityProperty = "Tipo", EnumType = typeof(Domain.TipoEmpresa))]
        public DropDownOptions TipoEmpresaOptions { get; set; }

        [Required]
        [MaxLength(18)]
        [DataType(DataType.Text)]
        [CssClass("col-md-3")]
        [Display(Name = "CNPJ")]
        [Mask("99.999.999/9999-99")]
        public string CNPJ { get; set; }

        [Required]
        [MaxLength(300, ErrorMessage = "O {0} deve conter no máximo {2} caracteres.")]
        [Display(Name = "nome fantasia")]
        [DataType(DataType.Text)]
        [CssClass("col-md-3")]
        public string NomeFantasia { get; set; }

        [Required]
        [MaxLength(300, ErrorMessage = "O {0} deve conter no máximo {2} caracteres.")]
        [Display(Name = "razão social")]
        [DataType(DataType.Text)]
        [CssClass("col-md-3")]
        public string RazaoSocial { get; set; }

        [MaxLength(20, ErrorMessage = "O {0} deve conter no máximo {2} caracteres.")]
        [Display(Name = "inscrição estadual")]
        [DataType(DataType.Text)]
        [CssClass("col-md-2")]
        public string InscricaoEstadual { get; set; }

        [MaxLength(20, ErrorMessage = "O {0} deve conter no máximo {2} caracteres.")]
        [Display(Name = "inscrição municipal")]
        [DataType(DataType.Text)]
        [CssClass("col-md-2")]
        public string InscricaoMunicipal { get; set; }

        [Required]
        [MaxLength(9, ErrorMessage = "O {0} deve conter no máximo {2} caracteres.")]
        [DataType(DataType.Text)]
        [CssClass("col-md-2")]
        [Display(Name = "CEP")]
        [Mask("99999-999")]
        public string CEP { get; set; }

        [Required]
        [MaxLength(300, ErrorMessage = "A {0} deve conter no máximo {2} caracteres.")]
        [DataType(DataType.Text)]
        [CssClass("col-md-6")]
        [Display(Name = "rua/av.")]
        public string Logradouro { get; set; }

        [MaxLength(10, ErrorMessage = "A {0} deve conter no máximo {2} caracteres.")]
        [DataType(DataType.Text)]
        [CssClass("col-md-2")]
        [Display(Name = "número")]
        public string Numero { get; set; }

        [MaxLength(30, ErrorMessage = "A {0} deve conter no máximo {2} caracteres.")]
        [DataType(DataType.Text)]
        [CssClass("col-md-3")]
        [Display(Name = "complemento")]
        public string Complemento { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "A {0} deve conter no máximo {2} caracteres.")]
        [DataType(DataType.Text)]
        [CssClass("col-md-3")]
        [Display(Name = "bairro")]
        public string Bairro { get; set; }

        [Minimum(1)]
        public int EstadoId { get; set; }

        [DataType(DataType.Text)]
        [CssClass("col-md-4")]
        [Display(Name = "Estado")]
        [ColumnDefinition(EntityName = "Estado", EntityProperty = "EstadoId", TableOptions = "estadooptions")]
        public DropDownStringOptions EstadoOptions { get; set; }

        [Minimum(1)]
        public int CidadeId { get; set; }

        [DataType(DataType.Text)]
        [CssClass("col-md-4")]
        [Display(Name = "Cidade")]
        [ColumnDefinition(EntityName = "Cidade", EntityProperty = "CidadeId", ParentColumn = "EstadoId", TableOptions = "cidadeoptions")]
        public DropDownStringOptions CidadeOptions { get; set; }

        [MaxLength(15)]
        public string Modulo { get; set; }
    }

    [DataEntity(GroupName = "Empresas", DisplayName = "Dados cadastrais da empresa")]
    public class PostEmpresaBindingModel : BaseEmpresaBindingModel
    {   
    }

    [DataEntity(GroupName = "Empresas", DisplayName = "Dados cadastrais da empresa")]
    public class PutEmpresaBindingModel : BaseEmpresaBindingModel
    {
        [Required]
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }

        [Required]
        public string ParentId { get; set; }
    }

    public class GetPostEmpresaBindingModel : IBindingModel
    {
        public int Tipo { get; set; }
    }

    public class EmpresaAddResultModel : BaseResultModel
    {
        public int Id { get; set; }
        public string Regiao { get; set; }
    }

    public class GetEmpresasByDocumento : IBindingModel
    {
        [Required]
        [MaxLength(18)]
        public string CNPJ { get; set; }
    }
}
