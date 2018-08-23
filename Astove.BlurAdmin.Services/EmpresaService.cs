using AInBox.Astove.Core.Extensions;
using AInBox.Astove.Core.Model;
using AInBox.Astove.Core.Service;
using System.Threading.Tasks;
using Astove.BlurAdmin.Data;
using Astove.BlurAdmin.Model;
using AInBox.Astove.Core.Enums;
using Astove.BlurAdmin.Model.Domain;
using System.Collections.Generic;
using MongoDB.Driver;
using System.Linq;
using AInBox.Astove.Core.Options;
using Autofac;
using WebApiDoodle.Net.Http.Client.Model;
using AInBox.Astove.Core.List;
using AInBox.Astove.Core.Options.EnumDomainValues;
using System.Text;

namespace Astove.BlurAdmin.Services
{
    public static class EmpresaService
    {
        public async static Task<PaginatedMongoModel<TList>> GetAll<TList>(this IEntityService<Empresa> service, IComponentContext container, PaginatedRequestCommand model)
            where TList : class, IMongoModel, IDto, new()
        {
            var result = await service.MongoService.GetMongoListAsync<EmpresaClienteMongoModel, TList>(container, model);
            var paged = result.ToPaginatedModel<TList>();

            paged.ColumnDefinitions = ColumnFactory.GenerateColumnDefinitions<TList>(ActionEnum.List).ToArray();
            paged.Condition = new Condition();

            return paged;
        }

        public async static Task<PostEmpresaBindingModel> GetPostEmpresaBindingModel(this IEntityService<Empresa> service, GetPostEmpresaBindingModel model)
        {
            var result = new PostEmpresaBindingModel { Tipo = model.Tipo, Modulo = EnumUtility.GetEnumText(Modulo.Gestor), InscricaoEstadual = "ISENTO", InscricaoMunicipal = "ISENTO" };
            await service.MongoService.SetDropDownOptions<PostEmpresaBindingModel>(result);
            return result;
        }

        public async static Task<PostResultModel> PostEmpresaBindingModel(this IEntityService<Empresa> service, PostEmpresaBindingModel model, ProfileMongoModel user)
        {
            var empresaResult = await service.GetEmpresasByDocumentoAsync(new GetEmpresasByDocumento { CNPJ = model.CNPJ });
            if (empresaResult.IsValid)
            {
                return new PostResultModel { IsValid = false, Message = string.Format("Já existe uma empresa com o CNPJ {0} informado.", model.CNPJ), StatusCode = 400 };
            }

            var entity = model.CreateInstanceOf<Empresa>(user.UsuarioId);
            entity.Modulo = EnumUtility.GetEnumText(Modulo.Gestor);
            var entityId = await service.AddAsync(entity);

            var mongoObj = entity.CreateInstanceOf<EmpresaClienteMongoModel>();
            model.CopyProperties(mongoObj);
            mongoObj.ParentId = entityId.ToString();
            var mongoId = await service.MongoService.InsertMongoObject<EmpresaClienteMongoModel>(mongoObj);

            var result = new PostResultModel { Id = mongoId, IsValid = true, ParentId = mongoObj.ParentId, StatusCode = 200 };
            return result;
        }

        public async static Task<PostOrPutResultModel> GetPutEmpresaBindingModel(this IEntityService<Empresa> service, GetBindingModel model)
        {
            var result = await service.MongoService.GetMongoObject<EmpresaClienteMongoModel>(model.Id);
            if (result == null)
            {
                result = await service.MongoService.GetMongoObjectByParentId<EmpresaClienteMongoModel>(model.Id);
                if (result == null)
                    return new PostOrPutResultModel { IsValid = false, StatusCode = 404 };
            }

            var binding = result.CreateInstanceOf<PutEmpresaBindingModel>();

            if (string.IsNullOrEmpty(binding.InscricaoEstadual))
                binding.InscricaoEstadual = "ISENTO";

            if (string.IsNullOrEmpty(binding.InscricaoMunicipal))
                binding.InscricaoMunicipal = "ISENTO";
            
            await service.MongoService.SetDropDownOptions<PutEmpresaBindingModel>(binding);
            return new PostOrPutResultModel { IsValid = true, BindingModel = binding };
        }

        public async static Task<BaseResultModel> PutEmpresaBindingModel(this IEntityService<Empresa> service, PutEmpresaBindingModel model, ProfileMongoModel user)
        {
            var mongoObj = await service.MongoService.GetMongoObjectByParentId<EmpresaClienteMongoModel>(model.ParentId);
            if (mongoObj == null)
                return new BaseResultModel { IsValid = false, StatusCode = 404, Message = "Não foi possível encontrar a empresa informada." };

            if (!model.CNPJ.Equals(mongoObj.CNPJ))
            {
                var empresaResult = await service.GetEmpresasByDocumentoAsync(new GetEmpresasByDocumento { CNPJ = model.CNPJ });
                if (empresaResult.IsValid)
                {
                    return new PostResultModel { IsValid = false, Message = string.Format("Já existe outra empresa cadastrada com o mesmo CNPJ {0} informado.", model.CNPJ), StatusCode = 400 };
                }
            }

            var entityId = int.Parse(mongoObj.ParentId);
            var entity = await service.GetSingleAsync(entityId);
            if (entity == null)
                return new BaseResultModel { IsValid = false, StatusCode = 404 };

            model.CopyProperties(mongoObj);
            await service.MongoService.UpdateMongoObject<EmpresaClienteMongoModel>(mongoObj.Id, mongoObj);

            model.CopyProperties(entity, user.UsuarioId);
            await service.EditAsync(entity.Id, entity);

            return new BaseResultModel { IsValid = true, StatusCode = 200 };
        }

        public async static Task<BaseResultModel> DeleteEmpresaBindingModel(this IEntityService<Empresa> service, DeleteBindingModel model)
        {
            var mongoObj = await service.MongoService.GetMongoObject<EmpresaClienteMongoModel>(model.Id);
            if (mongoObj == null)
                return new BaseResultModel { IsValid = false, StatusCode = 404 };

            var entityId = int.Parse(mongoObj.ParentId);
            var entity = await service.GetSingleAsync(entityId);
            if (entity == null)
                return new BaseResultModel { IsValid = false, StatusCode = 404 };

            if (!string.IsNullOrEmpty(mongoObj.Modulo) && !mongoObj.Modulo.Equals(EnumUtility.GetEnumText(Modulo.Gestor)))
                return new BaseResultModel { IsValid = false, StatusCode = 400, Message = string.Format("Esta empresa não pode ser excluída porque foi inserida pelo módulo {0}", mongoObj.Modulo) };

            await service.MongoService.DeleteMongoObject<EmpresaClienteMongoModel>(mongoObj.Id);

            await service.DeleteAsync(entity.Id);

            return new BaseResultModel { IsValid = true, StatusCode = 200 };
        }

        public async static Task<BaseResultModel> GetEmpresasFromFilter(this IEntityService<Empresa> service, List<FilterDefinition<EmpresaClienteMongoModel>> filtersEmpresa)
        {
            var listEmpresa = await service.MongoService.GetMongoListAsync<EmpresaClienteMongoModel>(filtersEmpresa);
            if (listEmpresa.Count == 0)
                return new BaseResultModel { IsValid = false, StatusCode = 404, Message = "Nenhuma empresa foi encontrada com os filtros informados" };

            var result = listEmpresa.FirstOrDefault().CreateInstanceOf<PutEmpresaBindingModel>();
            if (string.IsNullOrEmpty(result.InscricaoEstadual))
                result.InscricaoEstadual = "ISENTO";
            if (string.IsNullOrEmpty(result.InscricaoMunicipal))
                result.InscricaoMunicipal = "ISENTO";

            return new PostOrPutResultModel { IsValid = true, StatusCode = 200, BindingModel = result };
        }

        public async static Task<BaseResultModel> GetEmpresasByDocumentoAsync(this IEntityService<Empresa> service, GetEmpresasByDocumento model)
        {
            var filtersEmpresa = new List<FilterDefinition<EmpresaClienteMongoModel>>();
            var builderEmpresa = Builders<EmpresaClienteMongoModel>.Filter;
            filtersEmpresa.Add(builderEmpresa.Eq(p => p.CNPJ, model.CNPJ));

            return await service.GetEmpresasFromFilter(filtersEmpresa);
        }

        public async static Task<BaseResultModel> GetEmpresaByParentIdAsync(this IEntityService<Empresa> service, GetBindingModel model)
        {
            var filtersEmpresa = new List<FilterDefinition<EmpresaClienteMongoModel>>();
            var builderEmpresa = Builders<EmpresaClienteMongoModel>.Filter;
            filtersEmpresa.Add(builderEmpresa.Eq(p => p.ParentId, model.Id));

            return await service.GetEmpresasFromFilter(filtersEmpresa);
        }

        public async static Task<DropDownStringOptions> GetEmpresasByTipoAcessoOptions(this IEntityService<Empresa> service, int tipo)
        {
            var filtersEmpresaCliente = new List<FilterDefinition<EmpresaClienteMongoModel>>();
            var builderEmpresaCliente = Builders<EmpresaClienteMongoModel>.Filter;
            filtersEmpresaCliente.Add(builderEmpresaCliente.Eq(p => p.Tipo, tipo));            
            var listEmpresaCliente = await service.MongoService.GetMongoListAsync<EmpresaClienteMongoModel>(filtersEmpresaCliente);

            var items = listEmpresaCliente.Select(e => new KeyValueString { Id = e.Id, Key = e.ParentId, ParentId = e.Tipo.ToString(), Value = e.NomeFantasia }).ToArray();
            var options = new DropDownStringOptions { Items = items, Selected = items.FirstOrDefault() };

            return options;
        }

        public async static Task<BaseResultModel> ReloadMongoCollection(this IEntityService<Empresa> service, StringBuilder sb = null)
        {
            return await service.ReloadMongoCollection<EmpresaClienteMongoModel>(true, sb, Empresa.Includes);
        }
    }
}