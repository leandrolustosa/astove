namespace Astove.BlurAdmin.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Seed : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "cidade",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EstadoId = c.Int(nullable: false),
                        Nome = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        CodigoIBGE = c.Int(nullable: false),
                        DDD = c.String(nullable: false, maxLength: 6, storeType: "nvarchar"),
                        UsuarioCriacaoId = c.Int(nullable: false),
                        DataCriacao = c.DateTime(nullable: false, precision: 0),
                        UsuarioAlteracaoId = c.Int(),
                        DataAlteracao = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("estado", t => t.EstadoId, cascadeDelete: true)
                .Index(t => t.EstadoId);
            
            CreateTable(
                "estado",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false, maxLength: 30, storeType: "nvarchar"),
                        Sigla = c.String(nullable: false, maxLength: 2, storeType: "nvarchar"),
                        Capital = c.String(nullable: false, maxLength: 30, storeType: "nvarchar"),
                        Regiao = c.String(nullable: false, maxLength: 30, storeType: "nvarchar"),
                        FaixaDDD = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "configuracao",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Pessoa_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("pessoa", t => t.Pessoa_Id)
                .Index(t => t.Pessoa_Id);
            
            CreateTable(
                "empresa",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Tipo = c.Int(nullable: false),
                        CNPJ = c.String(nullable: false, maxLength: 20, storeType: "nvarchar"),
                        NomeFantasia = c.String(nullable: false, maxLength: 300, storeType: "nvarchar"),
                        RazaoSocial = c.String(maxLength: 300, storeType: "nvarchar"),
                        CEP = c.String(nullable: false, maxLength: 10, storeType: "nvarchar"),
                        CidadeId = c.Int(nullable: false),
                        Modulo = c.String(maxLength: 15, storeType: "nvarchar"),
                        Logradouro = c.String(maxLength: 300, storeType: "nvarchar"),
                        Numero = c.String(maxLength: 10, storeType: "nvarchar"),
                        Complemento = c.String(maxLength: 30, storeType: "nvarchar"),
                        Bairro = c.String(maxLength: 50, storeType: "nvarchar"),
                        InscricaoEstadual = c.String(maxLength: 20, storeType: "nvarchar"),
                        InscricaoMunicipal = c.String(maxLength: 20, storeType: "nvarchar"),
                        UsuarioCriacaoId = c.Int(nullable: false),
                        DataCriacao = c.DateTime(nullable: false, precision: 0),
                        UsuarioAlteracaoId = c.Int(),
                        DataAlteracao = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("cidade", t => t.CidadeId, cascadeDelete: true)
                .Index(t => t.CidadeId);
            
            CreateTable(
                "pessoa",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmpresaId = c.Int(nullable: false),
                        CPF = c.String(nullable: false, maxLength: 15, storeType: "nvarchar"),
                        Nome = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        Email = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        UsuarioId = c.Int(nullable: false),
                        Cargo = c.String(maxLength: 30, storeType: "nvarchar"),
                        Telefone = c.String(maxLength: 14, storeType: "nvarchar"),
                        ImagemUrl = c.String(unicode: false, storeType: "text"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("empresa", t => t.EmpresaId, cascadeDelete: true)
                .Index(t => t.EmpresaId);
            
            CreateTable(
                "search",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 300, storeType: "nvarchar"),
                        Text = c.String(nullable: false, unicode: false, storeType: "text"),
                        Route = c.String(nullable: false, unicode: false, storeType: "text"),
                        GlobalParameters = c.String(unicode: false, storeType: "text"),
                        Permission = c.String(maxLength: 50, storeType: "nvarchar"),
                        DateOfCreation = c.DateTime(precision: 0),
                        PessoaId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("pessoa", t => t.PessoaId)
                .Index(t => t.PessoaId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("search", "PessoaId", "pessoa");
            DropForeignKey("pessoa", "EmpresaId", "empresa");
            DropForeignKey("configuracao", "Pessoa_Id", "pessoa");
            DropForeignKey("empresa", "CidadeId", "cidade");
            DropForeignKey("cidade", "EstadoId", "estado");
            DropIndex("search", new[] { "PessoaId" });
            DropIndex("pessoa", new[] { "EmpresaId" });
            DropIndex("empresa", new[] { "CidadeId" });
            DropIndex("configuracao", new[] { "Pessoa_Id" });
            DropIndex("cidade", new[] { "EstadoId" });
            DropTable("search");
            DropTable("pessoa");
            DropTable("empresa");
            DropTable("configuracao");
            DropTable("estado");
            DropTable("cidade");
        }
    }
}
