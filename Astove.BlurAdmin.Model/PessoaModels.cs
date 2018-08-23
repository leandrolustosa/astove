using AInBox.Astove.Core.Attributes;
using AInBox.Astove.Core.Model;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astove.BlurAdmin.Model
{
    public class PessoaModel : BaseModel
    {
        public override int Id { get; set; }
        public string CPF { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsTokenRegistered { get; set; }
        [ColumnDefinition(EntityProperty = "Empresa", Load = true)]
        public PessoaEmpresaModel Empresa { get; set; }
    }

    public class PessoaEmpresaModel : BaseModel
    {
        public override int Id { get; set; }
        public int TipoAcesso { get; set; }
        public string CNPJ { get; set; }
        public string Nome { get; set; }
        public string CEP { get; set; }
        public string Cidade { get; set; }
    }
    
    public class PessoaConfigurationModel : BaseModel
    {
        public override int Id { get; set; }
        public bool AtivarNotificacaoPush { get; set; }
        public bool AtivarNotificacaoEmail { get; set; }
        public int NumeroProdutosCampeaoVendas { get; set; }
        public int NumeroProdutosRegionais { get; set; }        
        public string Regiao { get; set; }
    }

    [DataEntity(EntityName = "Pessoa", KeyColumn = "Id", ValueColumn = "Nome", ParentColumn = "ParentId")]
    public class PessoaMongoModel : BaseMongoModel
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }
        public int EmpresaId { get; set; }
        [ColumnDefinition(EntityProperty = "Empresa.Tipo")]
        public int EmpresaTipo { get; set; }
        [ColumnDefinition(EntityProperty = "Empresa.CNPJ")]
        public string EmpresaCNPJ { get; set; }
        [ColumnDefinition(EntityProperty = "Empresa.Nome")]
        public string EmpresaNome { get; set; }
        [ColumnDefinition(EntityProperty = "Empresa.RazaoSocial")]
        public string EmpresaRazaoSocial { get; set; }
        [ColumnDefinition(EntityProperty = "Empresa.CEP")]
        public string EmpresaCEP { get; set; }
        [ColumnDefinition(EntityProperty = "Empresa.Cidade")]
        public string EmpresaCidade { get; set; }
        [ColumnDefinition(EntityProperty = "Empresa.EstadoId")]
        public int EmpresaEstadoId { get; set; }
        [ColumnDefinition(EntityProperty = "Empresa.Estado.Regiao")]
        public string EmpresaRegiao { get; set; }
        [ColumnDefinition(EntityProperty = "Empresa.Estado.Sigla")]
        public string EmpresaSiglaUF { get; set; }
        public string CPF { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public int UsuarioId { get; set; }
        public string Cargo { get; set; }
        public string Telefone { get; set; }
        public int usuarioId { get; set; }
        public string ImagemUrl { get; set; }
        [ColumnDefinition(EntityProperty = "RegistredTokens.Count")]
        public int RegistredTokens { get; set; }
    }

    public class ProfileListMongoModel : BaseMongoModel
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }
        [FilterLike(DisplayName = "E-mail")]
        public string Email { get; set; }
        [FilterLike(DisplayName = "Telefone", Mask = "(99)9999-9999")]
        public string Telefone { get; set; }
        [FilterLike]
        public string Nome { get; set; }
        [FilterLike]
        public string Sobrenome { get; set; }
        [FilterLike]
        public string Funcao { get; set; }
        public string FotoUrl { get; set; }
        [FilterDate(DisplayName = "Data de Nascimento")]
        public DateTime? DataNascimento { get; set; }
        [FilterBoolean]
        public bool Ativo { get; set; }
    }
}
