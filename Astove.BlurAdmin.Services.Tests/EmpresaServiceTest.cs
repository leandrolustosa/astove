using Xunit;
using Astove.BlurAdmin.Data;
using Autofac;
using AInBox.Astove.Core.UnitTest;
using AInBox.Astove.Core.Service;
using Astove.BlurAdmin.Model;
using AInBox.Astove.Core.Model;
using AInBox.Astove.Core.Enums;
using System.Threading.Tasks;
using System.Linq;
using Xbehave;
using Astove.BlurAdmin.Model.Domain;
using AInBox.Astove.Core.Extensions;
using AInBox.Astove.Core.Exceptions;
using AInBox.Astove.Core.Options;

namespace Astove.BlurAdmin.Services.Tests
{
    public class EmpresaServiceTest : IAstoveUnitTest
    {
        private static IContainer container;
        private static IEntityService<Empresa> service;

        public EmpresaServiceTest()
        {
            if (container == null)
            {
                container = Bootstrap.BuildContainer();
                service = container.Resolve<IEntityService<Empresa>>();

                Reset();
            }
        }

        public void Reset(bool resetContext = false)
        {
            if (resetContext)
            {
                var context = container.Resolve<IAstoveContext>();
                context.Reset();
            }

            var estadoService = container.Resolve<IEntityService<Estado>>();
            var cidadeService = container.Resolve<IEntityService<Cidade>>();
            var pessoaService = container.Resolve<IEntityService<Pessoa>>();

            Task.Run(() => estadoService.ReloadMongoCollection()).Wait();
            Task.Run(() => cidadeService.ReloadMongoCollection()).Wait();
            Task.Run(() => pessoaService.ReloadMongoCollection()).Wait();
            Task.Run(() => service.ReloadMongoCollection()).Wait();
        }

        [Scenario]
        [Example(1)]
        public void GetAll(int quantidadeEsperada)
        {
            Reset(true);

            PaginatedMongoModel<ListaEmpresaMongoModel> result = null;

            "Quando solicitado obter todas as empresas"
                .x(async () => {
                    var model = new PaginatedRequestCommand { Page = 1, Take = 1000, Type = EnumUtility.GetEnumText(GetTypes.Paged) };
                    result = await service.GetAll<ListaEmpresaMongoModel>(container, model);
                });

            "Então a quantidade de empresas deve ser {0}"
                .x(() => {
                    Assert.Equal(result.Items.Count(), quantidadeEsperada);
                });
        }

        [Scenario]
        [Example((int)TipoEmpresa.Organizacao)]
        [Example((int)TipoEmpresa.Cliente)]
        [Example((int)TipoEmpresa.Fornecedor)]
        public void GetPost(int tipo)
        {
            PostEmpresaBindingModel result = null;

            "Quando solicitado obter get post"
                .x(async () => {
                    result = await service.GetPostEmpresaBindingModel(new GetPostEmpresaBindingModel { Tipo = tipo });
                });

            "Então o binding deve ser do tipo {0}"
                .x(() => {
                    Assert.NotNull(result);
                    Assert.Equal(result.Tipo, tipo);
                    Assert.Equal(result.InscricaoEstadual, "ISENTO");
                    Assert.Equal(result.InscricaoMunicipal, "ISENTO");
                });
        }

        [Scenario]
        [Example(1, "AI'n'Box Sistemas Web")]
        public void GetPut(int id, string nome)
        {
            PostOrPutResultModel result = null;

            "Quando solicitado obter get put com o id {0}"
                .x(async () => {
                    var model = await service.MongoService.GetMongoObjectByParentId<EmpresaClienteMongoModel>(id);
                    
                    result = await service.GetPutEmpresaBindingModel(new GetBindingModel { Id = model.Id });
                });

            "Então o binding deve ter o nome fantasia {1}"
                .x(() => {
                    Assert.NotNull(result);
                    Assert.Equal(result.IsValid, true);
                    Assert.IsType<PutEmpresaBindingModel>(result.BindingModel);
                    var obj = (PutEmpresaBindingModel)result.BindingModel;
                    Assert.Equal(nome, obj.NomeFantasia);
                    Assert.NotEqual(0, obj.EstadoId);
                    Assert.NotEqual(0, obj.CidadeId);
                    Assert.NotEqual(0, obj.CidadeOptions.Items.Length);
                });
        }

        [Scenario]
        [Example("id-not-found", 404)]
        public void GetPutNotFound(string id, int statusCode)
        {
            PostOrPutResultModel result = null;

            "Quando solicitado obter get put com o id {0}"
                .x(async () => {
                    result = await service.GetPutEmpresaBindingModel(new GetBindingModel { Id = id });
                });

            "Então o binding deve ter o status code {1}"
                .x(() => {
                    Assert.NotNull(result);
                    Assert.Equal(result.IsValid, false);
                    Assert.Equal(statusCode, result.StatusCode);
                });
        }

        [Scenario]
        [Example((int)TipoEmpresa.Cliente, "Shori", "01.958.931/0001-60", "29072-251", 3173, 8, 2)]
        [Example((int)TipoEmpresa.Cliente, "Yes Feiras", "03.816.418/0001-60", "29101-522", 3172, 8, 3)]
        [Example((int)TipoEmpresa.Cliente, "Fricote", "05.793.327/0001-81", "29055-917", 3173, 8, 4)]
        public void Post(int tipo, string nomeFantasia, string cnpj, string cep, int cidadeId, int estadoId, int idOuQuantidadeEsperada)
        {
            PostResultModel result = null;
            PaginatedMongoModel<ListaEmpresaMongoModel> listResult = null;
            BaseResultModel getResult = null;
            DropDownStringOptions optionsResult = null;

            "Quando solicitado adicionar empresa do tipo cliente {1}"
                .x(async () => {
                    var user = await service.MongoService.GetMongoObjectByParentId<PessoaMongoModel>(1);
                    if (user == null)
                    {
                        result = new PostResultModel { IsValid = false, StatusCode = 404, Message = "Profile não encontrado" };
                    }
                    else
                    {
                        var model = await service.GetPostEmpresaBindingModel(new GetPostEmpresaBindingModel { Tipo = tipo });
                        model.CNPJ = cnpj;
                        model.NomeFantasia = nomeFantasia;
                        model.CEP = cep;
                        model.EstadoId = estadoId;
                        model.CidadeId = cidadeId;

                        result = await service.PostEmpresaBindingModel(model, user.CreateInstanceOf<ProfileMongoModel>());
                    }
                });

            "Então o result deve ser válido e conter as chaves do novo objeto criado com Id {6}"
                .x(() => {
                    Assert.NotNull(result);
                    Assert.Equal(result.IsValid, true);
                    Assert.Equal(idOuQuantidadeEsperada, int.Parse(result.ParentId));
                    Assert.NotNull(result.Id);
                });

            "Quando novamente solicitado obter todas as empresas"
                .x(async () => {
                    var model = new PaginatedRequestCommand { Page = 1, Take = 1000, Type = EnumUtility.GetEnumText(GetTypes.Paged) };
                    listResult = await service.GetAll<ListaEmpresaMongoModel>(container, model);
                });

            "Então a quantidade de empresas deve ser {6}"
                .x(() => {
                    Assert.Equal(listResult.Items.Count(), idOuQuantidadeEsperada);
                });

            "Quando for feita uma pesquisa por CNPJ {2}"
                .x(async () => {
                    getResult = await service.GetEmpresasByDocumentoAsync(new GetEmpresasByDocumento { CNPJ = cnpj });
                });

            "Então o nome fantasia deve ser {1}"
                .x(() => {
                    Assert.Equal(getResult.IsValid, true);
                    Assert.IsType<PostOrPutResultModel>(getResult);
                    var empresaResult = (PostOrPutResultModel)getResult;
                    Assert.IsType<PutEmpresaBindingModel>(empresaResult.BindingModel);
                    var binding = (PutEmpresaBindingModel)empresaResult.BindingModel;
                    Assert.Equal(binding.NomeFantasia, nomeFantasia);
                });

            "Quando for feita uma pesquisa por Id {2}"
                .x(async () => {
                    getResult = await service.GetEmpresaByParentIdAsync(new GetBindingModel { Id = result.ParentId });
                });

            "Então o nome fantasia deve ser {1}"
                .x(() => {
                    Assert.Equal(getResult.IsValid, true);
                    Assert.IsType<PostOrPutResultModel>(getResult);
                    var empresaResult = (PostOrPutResultModel)getResult;
                    Assert.IsType<PutEmpresaBindingModel>(empresaResult.BindingModel);
                    var binding = (PutEmpresaBindingModel)empresaResult.BindingModel;
                    Assert.Equal(binding.NomeFantasia, nomeFantasia);
                });

            "Quando for feita uma pesquisa por options por tipo de acesso {0}"
                .x(async () => {
                    optionsResult = await service.GetEmpresasByTipoAcessoOptions(tipo);
                });

            "Então a lista de opções deve ter {6}-1 itens"
                .x(() => {
                    Assert.Equal(optionsResult.Items.Count(), idOuQuantidadeEsperada - 1);                    
                });
        }

        [Scenario]
        [Example((int)TipoEmpresa.Cliente, 1)]
        public void PostInvalid(int tipo, int quantidadeEsperada)
        {
            PostResultModel result = null;
            PaginatedMongoModel<ListaEmpresaMongoModel> listResult = null;

            "Quando solicitado adicionar empresa com dados inválidos então deve retornar exceção {1}"
                .x(() => {
                    Assert.ThrowsAsync<AstoveModelInvalidException<Empresa>>(async () => {
                        var user = await service.MongoService.GetMongoObjectByParentId<PessoaMongoModel>(1);
                        if (user == null)
                        {
                            result = new PostResultModel { IsValid = false, StatusCode = 404, Message = "Profile não encontrado" };
                        }
                        else
                        {
                            var model = await service.GetPostEmpresaBindingModel(new GetPostEmpresaBindingModel { Tipo = tipo });
                            result = await service.PostEmpresaBindingModel(model, user.CreateInstanceOf<ProfileMongoModel>());
                        }
                    });                
                });

            "Quando novamente solicitado obter todas as empresas"
                .x(async () => {
                    var model = new PaginatedRequestCommand { Page = 1, Take = 1000, Type = EnumUtility.GetEnumText(GetTypes.Paged) };
                    listResult = await service.GetAll<ListaEmpresaMongoModel>(container, model);
                });

            "Então a quantidade de empresas deve ser {6}"
                .x(() => {
                    Assert.Equal(listResult.Items.Count(), quantidadeEsperada);
                });
        }

        [Scenario]
        [Example(1, "AI'n'Box")]
        public void Put(int id, string nomeFantasia)
        {
            BaseResultModel result = null;
            
            "Quando solicitado editar empresa do tipo organização {1}"
                .x(async () => {
                    var user = await service.MongoService.GetMongoObjectByParentId<PessoaMongoModel>(1);
                    if (user == null)
                    {
                        result = new BaseResultModel { IsValid = false, StatusCode = 404, Message = "Profile não encontrado" };
                    }
                    else
                    {
                        var model = await service.MongoService.GetMongoObjectByParentId<EmpresaClienteMongoModel>(id);
                        var putResult = await service.GetPutEmpresaBindingModel(new GetBindingModel { Id = model.Id });

                        Assert.IsType<PutEmpresaBindingModel>(putResult.BindingModel);
                        var obj = (PutEmpresaBindingModel)putResult.BindingModel;

                        obj.NomeFantasia = nomeFantasia;
                        
                        result = await service.PutEmpresaBindingModel(obj, user.CreateInstanceOf<ProfileMongoModel>());
                    }
                });

            "Então o result deve ser válido e conter o novo nome fantasia {1}"
                .x(async () => {
                    Assert.NotNull(result);
                    Assert.Equal(result.IsValid, true);

                    var entity = await service.GetSingleAsync(id);

                    Assert.NotNull(entity.NomeFantasia.Equals(nomeFantasia));

                    Reset(true);
                });
        }

        [Scenario]
        [Example(1, 0)]
        public void Delete(int id, int quantidadeEsperada)
        {
            BaseResultModel result = null;
            PaginatedMongoModel<ListaEmpresaMongoModel> listResult = null;

            "Quando solicitado editar empresa com id {0}"
                .x(async () => {
                    var user = await service.MongoService.GetMongoObjectByParentId<PessoaMongoModel>(1);
                    if (user == null)
                    {
                        result = new BaseResultModel { IsValid = false, StatusCode = 404, Message = "Profile não encontrado" };
                    }
                    else
                    {
                        var model = await service.MongoService.GetMongoObjectByParentId<EmpresaClienteMongoModel>(id);                        
                        result = await service.DeleteEmpresaBindingModel(new DeleteBindingModel { Id = model.Id });
                    }
                });

            "Então o result deve ser válido e o objeto deve ter sido excluído"
                .x(() => {
                    Assert.NotNull(result);
                    Assert.Equal(result.IsValid, true);
                });

            "Quando novamente solicitado obter todas as empresas"
                .x(async () => {
                    var model = new PaginatedRequestCommand { Page = 1, Take = 1000, Type = EnumUtility.GetEnumText(GetTypes.Paged) };
                    listResult = await service.GetAll<ListaEmpresaMongoModel>(container, model);
                });

            "Então a quantidade de empresas deve ser {1}"
                .x(() => {
                    Assert.Equal(listResult.Items.Count(), quantidadeEsperada);

                    Reset(true);
                });
        }
    }
}
