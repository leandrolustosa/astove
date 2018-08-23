using AInBox.Astove.Core.Model;
using AInBox.Astove.Core.Security;
using Autofac;
using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Transactions;
using Astove.BlurAdmin.Data;
using Astove.BlurAdmin.Model;
using Astove.BlurAdmin.Model.Domain;
using AInBox.Astove.Core.Service;
using System.Linq.Expressions;
using System.Collections.Generic;
using MongoDB.Driver;
using AInBox.Astove.Core.Options;
using AInBox.Astove.Core.Extensions;
using System.Linq;
using AInBox.Astove.Core.Enums;
using System.Text;

namespace Astove.BlurAdmin.Services
{
    public static class PessoaService
    {
        #region Common

        public const string PERMISSOES = "permissoes";
        
        #endregion

        public async static Task AtualizarPermissoes(this IEntityService<Pessoa> service, ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            var col = service.MongoDatabase.GetCollection<KeyValueString>(PERMISSOES);
            await col.DeleteManyAsync(Builders<KeyValueString>.Filter.Empty);

            var permissoesEnum = EnumUtility.GetEnumTexts(typeof(Model.Domain.Permissao));
            var roles = await roleManager.Roles.ToListAsync();
            var permissoes = roles.Select(r => new KeyValueString { Key = r.Name.ToString(), Value = permissoesEnum.FirstOrDefault(p => p.Key == r.Id).Value, ParentId = r.Id.ToString() }).ToList();
            if (permissoes.Count > 0)
                await col.InsertManyAsync(permissoes);
        }

        public async static Task<PessoaAddResultModel> Register(this IEntityService<Pessoa> service, IComponentContext container, IEntityService<Estado> estadoService, IEntityService<Configuracao> configuracaoService, ApplicationUserManager userManager, PessoaRegisterBindingModel model, ApplicationUser usuario)
        {
            try
            {
                int pessoaId = 0;
                var pessoa = await service.GetByEmailAsync(model.Email);
                if (pessoa != null)
                    throw new Exception("E-mail já registrado");

                pessoa = await service.GetByDocumentAsync(model.CPF);
                if (pessoa != null)
                    throw new Exception("CPF já registrado");

                pessoa = new Pessoa();
                pessoa.CPF = model.CPF;
                pessoa.Email = model.Email;
                pessoa.Nome = model.Nome;
                pessoa.UsuarioId = usuario.Id;
                pessoa.Telefone = model.PhoneNumber;

                pessoaId = await service.AddAsync(pessoa);
                    
                var profile = new ProfileMongoModel();
                profile.ParentId = pessoa.Id.ToString();
                profile.UsuarioId = usuario.Id;
                profile.Email = pessoa.Email;
                profile.Telefone = usuario.PhoneNumber;
                profile.Nome = pessoa.Nome;
                profile.CPF = pessoa.CPF;
                profile.Cargo = pessoa.Cargo;
                profile.Permissoes = new string[0];

                await service.MongoService.InsertMongoObject<ProfileMongoModel>(profile);
                    
                return new PessoaAddResultModel { Id = pessoaId, IsValid = true, StatusCode = 200 };
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async static Task ChangeUsuario(this IEntityService<Pessoa> service, ApplicationUserManager userManager, PutPessoaChangeProfileBindingModel model, Estado estado, ApplicationUser usuario)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var pessoa = await service.Where(o => o.UsuarioId == usuario.Id).FirstOrDefaultAsync();
                    
                    pessoa.CPF = model.CPF;
                    pessoa.Email = model.NewEmail;
                    pessoa.Nome = model.Nome;
                    pessoa.Cargo = model.Cargo;
                    pessoa.Telefone = model.PhoneNumber;
                    pessoa.ImagemUrl = model.FotoUrl;

                    await service.EditAsync(pessoa.Id, pessoa);

                    var profile = await service.MongoService.GetMongoObjectByParentId<ProfileMongoModel>(pessoa.Id.ToString());
                    if (profile != null)
                    {
                        profile.ParentId = pessoa.Id.ToString();
                        profile.UsuarioId = usuario.Id;
                        profile.Email = pessoa.Email;
                        profile.Telefone = usuario.PhoneNumber;
                        profile.Nome = pessoa.Nome;
                        profile.CPF = pessoa.CPF;
                        profile.Cargo = pessoa.Cargo;
                        profile.ImagemUrl = pessoa.ImagemUrl;
                        profile.Permissoes = new string[0];

                        await service.MongoService.UpdateMongoObject<ProfileMongoModel>(profile.Id, profile);
                    }

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    scope.Dispose();
                }
            }
        }

        public async static Task<PostOrPutResultModel> ObterPutProfileBindingModel(this IEntityService<Pessoa> service, IMongoService mongoService, string id, int userId, IList<string> roles)
        {
            var obj = await service.MongoService.GetMongoObject<PessoaMongoModel>(id);
            if (obj == null)
                return new PostOrPutResultModel { IsValid = false, StatusCode = 404 };

            var profile = obj.CreateInstanceOf<PutPessoaChangeProfileBindingModel>();
            profile.FotoUrlSource = string.Empty;            
            profile.ProfileId = int.Parse(obj.ParentId);
            profile.PhoneNumber = obj.Telefone;
            profile.Cargo = obj.Cargo;
            
            profile.PermiteAlterarSenha = obj.UsuarioId == userId;

            profile.NewEmail = profile.Email;
            profile.Password = string.Empty;
            profile.NewPassword = string.Empty;
            profile.ConfirmPassword = string.Empty;

            return new PostOrPutResultModel { IsValid = true, BindingModel = profile };
        }

        public async static Task<PostProfileBindingModel> ObterPostProfileBindingModel(this IEntityService<Pessoa> service, IMongoService mongoService)
        {
            var profile = new PostProfileBindingModel
            {
                Ativo = true,
                CodigoEmpresas = new string[0],
                Email = string.Empty,
                FotoUrl = string.Empty,
                FotoUrlSource = string.Empty,
                Funcao = string.Empty,
                Nome = string.Empty,
                PermissaoOptions = await mongoService.GetMongoOptions<KeyValueString>(collection: PERMISSOES),
                Permissoes = new string[0],
                Sobrenome = string.Empty,
                Telefone = string.Empty
            };

            return profile;
        }

        public async static Task<Pessoa> GetByEmailAsync(this IEntityService<Pessoa> service, string email)
        {
            var entity = await service.Where(o => o.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefaultAsync();
            return entity;
        }

        public async static Task<Pessoa> GetByDocumentAsync(this IEntityService<Pessoa> service, string cpf)
        {
            var entity = await service.Where(o => o.CPF.Equals(cpf, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefaultAsync();
            return entity;
        }

        public async static Task<Pessoa> GetByDocumentAsync(this IEntityService<Pessoa> service, int userId, string cpf)
        {
            var entity = await service.Where(o => o.UsuarioId != userId && o.CPF.Equals(cpf, StringComparison.InvariantCultureIgnoreCase), false).FirstOrDefaultAsync();
            return entity;
        }

        public async static Task<Pessoa> GetLoggedInAsync(this IEntityService<Pessoa> service, ApplicationUserManager userManager, string email, params Expression<Func<Pessoa, object>>[] includeProperties)
        {
            var usuario = await userManager.FindByEmailAsync(email);
            var profile = await service.Where(p => p.UsuarioId == usuario.Id, false, includeProperties).FirstOrDefaultAsync();
            return profile;
        }

        public async static Task<PessoaMongoModel> GetLoggedInAsync(this IEntityService<Pessoa> service, string email)
        {
            var filtersPessoa = new List<FilterDefinition<PessoaMongoModel>>();
            var builderPessoa = Builders<PessoaMongoModel>.Filter;
            filtersPessoa.Add(builderPessoa.Eq(p => p.Email, email));
            var listPessoa = await service.MongoService.GetMongoListAsync<PessoaMongoModel>(filtersPessoa);

            return listPessoa.FirstOrDefault();
        }

        public async static Task<BaseResultModel> ReloadMongoCollection(this IEntityService<Pessoa> service, StringBuilder sb = null)
        {
            return await service.ReloadMongoCollection<PessoaMongoModel>(true, sb, Pessoa.Includes);
        }
    }
}
