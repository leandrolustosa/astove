using AInBox.Astove.Core.Attributes;
using AInBox.Astove.Core.Controllers;
using AInBox.Astove.Core.Exceptions;
using AInBox.Astove.Core.Extensions;
using AInBox.Astove.Core.List;
using AInBox.Astove.Core.Model;
using AInBox.Astove.Core.Options;
using AInBox.Astove.Core.Options.EnumDomainValues;
using AInBox.Astove.Core.Service;
using AInBox.Astove.Core.Validations;
using Autofac;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http;
using Astove.BlurAdmin.Data;
using Astove.BlurAdmin.Model;
using Astove.BlurAdmin.Model.Domain;
using Astove.BlurAdmin.Services;

namespace Astove.BlurAdmin.WebApi.Controllers
{
    public class PessoasController : WebApiCustomController<Pessoa, PessoaModel>
    {
        private readonly IEntityService<Estado> estadoService;
        private readonly IEntityService<Configuracao> configuracaoService;
        
        public PessoasController(IComponentContext container, IEntityService<Pessoa> service, IEntityService<Estado> estadoService, IEntityService<Configuracao> configuracaoService) : base(container, service)
        {
            this.estadoService = estadoService;
            this.configuracaoService = configuracaoService;            
        }

        [AllowAnonymous]
        [ActionName("AtualizarPermissoes")]
        public async Task<IHttpActionResult> GetAtualizarUsuarios(string token)
        {
            try
            {
                if (!token.Equals(System.Configuration.ConfigurationManager.AppSettings["Token"], StringComparison.InvariantCultureIgnoreCase))
                    return Unauthorized();

                var sb = new StringBuilder("");
                try
                {
                    await this.Service.AtualizarPermissoes(this.UserManager, this.RoleManager);
                }
                catch (Exception ex)
                {
                    sb.AppendLine(string.Format("Atualizar Permissões: {0}", ex.GetExceptionMessage()));
                }

                if (sb.ToString().Equals(""))
                    return Ok();
                else
                    return BadRequest(sb.ToString());
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("DefaultAction")]
        public async Task<IHttpActionResult> Get([FromUri] PostRegisterTokenBinding modelToken)
        {
            var email = User.Identity.Name;
            try
            {
                var usuario = await UserManager.FindByNameAsync(email);
                var obj = await this.Service.Where(o => o.UsuarioId == usuario.Id, false).FirstOrDefaultAsync();
                if (obj == null)
                    return NotFound();

                var model = obj.ToModel<PessoaModel, IModel>(this.Container, false, 0);
                model.PhoneNumber = usuario.PhoneNumber;                

                return Ok<PessoaModel>(model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("Register")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> PostRegister(PessoaRegisterBindingModel model)
        {
            if (string.IsNullOrEmpty(model.RazaoSocial))
                model.RazaoSocial = model.EmpresaNome;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var obj = await this.Service.GetByDocumentAsync(model.CPF);
            if (obj != null)
            {
                ModelState.AddModelError("CPF", new Exception("CPF já existe"));
                return BadRequest(ModelState);
            }

            var usuario = await UserManager.FindByNameAsync(model.Email);
            if (usuario != null)
            {
                ModelState.AddModelError("Email", new Exception("E-mail já está sendo utilizado por um outro usuário"));
                return BadRequest(ModelState);
            }

            var cidades = await this.Service.MongoService.GetMongoListAsync<CidadeMongoModel>();
            var ddds = cidades.Select(c => c.DDD).Distinct().ToList();

            var telefoneValidacao = CustomValidator.ValidarTelefoneFixo(model.PhoneNumber, ddds);
            var celularValidacao = CustomValidator.ValidarCelular(model.PhoneNumber, ddds);
            if (!CustomValidator.ValidarCNPJ(model.CNPJ) || !CustomValidator.ValidarCPF(model.CPF) || (!telefoneValidacao.IsValid && !celularValidacao.IsValid))
            {
                if (!CustomValidator.ValidarCNPJ(model.CNPJ))
                    ModelState.AddModelError("CNPJ", "CNPJ informado não é válido");
                if (!CustomValidator.ValidarCNPJ(model.CPF))
                    ModelState.AddModelError("CPF", "CPF informado não é válido");
                if (!telefoneValidacao.IsValid && !celularValidacao.IsValid)
                    ModelState.AddModelError("PhoneNumber", string.Concat(telefoneValidacao.Message, ", ", celularValidacao.Message));

                return BadRequest(ModelState);
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber };
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    IdentityResult result = await UserManager.CreateAsync(user, model.Password);

                    if (!result.Succeeded)
                    {
                        return GetErrorResult(result);
                    }

                    usuario = await UserManager.FindByNameAsync(model.Email);
                    if (usuario == null)
                    {
                        ModelState.AddModelError("UsuarioId", new Exception("Não foi possível criar o usuário"));
                        return BadRequest(ModelState);
                    }

                    await this.Service.Register(this.Container, estadoService, configuracaoService, UserManager, model, usuario);

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
                finally
                {
                    scope.Dispose();
                }
            }          

            return Ok();
        }

        [ActionName("DefaultAction")]
        public async Task<IHttpActionResult> DeleteAccount()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var email = User.Identity.Name;
                    var usuario = await UserManager.FindByNameAsync(email);
                    var pessoa = await this.Service.Where(o => o.UsuarioId == usuario.Id).FirstOrDefaultAsync();

                    await this.UserManager.DeleteAsync(usuario);

                    await this.Service.DeleteAsync(pessoa.Id);

                    scope.Complete();

                    return Ok();
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
                finally
                {
                    scope.Dispose();
                }
            }
        }

        [ActionName("ChangeUsuario")]
        public async Task<IHttpActionResult> PutChangeUsuario(PutPessoaChangeProfileBindingModel model)
        {
            if (string.IsNullOrEmpty(model.RazaoSocial))
                model.RazaoSocial = model.EmpresaNome;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!string.IsNullOrEmpty(model.NewPassword) && model.NewPassword.Length < 4)
            {
                ModelState.AddModelError("NewPassword", new Exception("A Nova Senha deve ter no mínimo 4 caracteres."));
                return BadRequest(ModelState);
            }

            var usuario = await UserManager.FindByNameAsync(User.Identity.Name);
            if (usuario == null)
            {
                ModelState.AddModelError("Email", new Exception("Usuário não autenticado"));
                return BadRequest(ModelState);
            }

            var obj = await this.Service.GetByDocumentAsync(usuario.Id, model.CPF);
            if (obj != null)
            {
                ModelState.AddModelError("CPF", new Exception("CPF já existe"));
                return BadRequest(ModelState);
            }

            var cidades = await this.Service.MongoService.GetMongoListAsync<CidadeMongoModel>();
            var ddds = cidades.Select(c => c.DDD).Distinct().ToList();

            var telefoneValidacao = CustomValidator.ValidarTelefoneFixo(model.PhoneNumber, ddds);
            var celularValidacao = CustomValidator.ValidarCelular(model.PhoneNumber, ddds);
            if (!CustomValidator.ValidarCNPJ(model.CNPJ) || !CustomValidator.ValidarCPF(model.CPF) || (!telefoneValidacao.IsValid && !celularValidacao.IsValid))
            {
                if (!CustomValidator.ValidarCNPJ(model.CNPJ))
                    ModelState.AddModelError("CNPJ", "CNPJ informado não é válido");
                if (!CustomValidator.ValidarCNPJ(model.CPF))
                    ModelState.AddModelError("CPF", "CPF informado não é válido");
                if (!telefoneValidacao.IsValid && !celularValidacao.IsValid)
                    ModelState.AddModelError("PhoneNumber", string.Concat(telefoneValidacao.Message, ", ", celularValidacao.Message));

                return BadRequest(ModelState);
            }

            try
            {
                if (!string.IsNullOrEmpty(model.PhoneNumber))
                    model.PhoneNumber = model.PhoneNumber.Replace("_", "");

                var token = await this.UserManager.GenerateChangePhoneNumberTokenAsync(usuario.Id, model.PhoneNumber);
                IdentityResult result = await this.UserManager.ChangePhoneNumberAsync(usuario.Id, model.PhoneNumber, token);
                if (!result.Succeeded)
                    throw new Exception("Não foi possível alterar o número de telefone do usuário");

                if (!string.IsNullOrEmpty(model.Password))
                {
                    IdentityResult resultPass = await this.UserManager.ChangePasswordAsync(usuario.Id, model.Password, model.NewPassword);
                    if (!resultPass.Succeeded)
                        throw new Exception("Não foi possível alterar a senha do usuário");
                }

                if (!string.IsNullOrEmpty(model.NewEmail) && !model.NewEmail.Equals(model.Email))
                {
                    usuario.UserName = model.NewEmail;
                    usuario.Email = model.NewEmail;
                    IdentityResult resultUser = await this.UserManager.UpdateAsync(usuario);
                    if (!resultUser.Succeeded)
                        throw new Exception("Não foi possível alterar o e-mail e nome do usuário");
                }


                var estado = await estadoService.GetBySiglaAsync(model.SiglaUF);
                if (estado == null)
                {
                    var ex = new AstoveServiceException<Pessoa>("PessoasController", "PutRregister(PessoaChangeProfileBindingModel model)", string.Concat("Falha ao tentar recuperar o estado através da sigla ", model.SiglaUF));

                    return InternalServerError(ex);
                }

                await this.Service.ChangeUsuario(UserManager, model, estado, usuario);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("UsuarioId", ex);
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [DeflateCompression]
        [ActionName("GetProfiles")]
        public async Task<IHttpActionResult> GetProfiles([FromUri] PaginatedRequestCommand model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.Type) && model.Type.Equals("UpdateModel", StringComparison.InvariantCultureIgnoreCase))
                {
                    var user = await this.Service.GetLoggedInAsync(User.Identity.Name);
                    var roles = await this.UserManager.GetRolesAsync(user.UsuarioId);
                    var resultModel = await this.Service.ObterPutProfileBindingModel(this.Service.MongoService, user.Id, user.UsuarioId, roles);
                    if (resultModel.IsValid)
                        return Ok<PutPessoaChangeProfileBindingModel>((PutPessoaChangeProfileBindingModel)resultModel.BindingModel);
                    else
                        return this.GetErrorHttpActionResult(resultModel);
                }

                if (!string.IsNullOrEmpty(model.Type) && model.Type.Equals("InsertModel", StringComparison.InvariantCultureIgnoreCase))
                {
                    var bindingModel = await this.Service.ObterPostProfileBindingModel(this.Service.MongoService);
                    return Ok<PostProfileBindingModel>(bindingModel);
                }

                var result = await this.Service.MongoService.GetMongoListAsync<ProfileMongoModel, ProfileListMongoModel>(this.Container, model);
                var paged = result.ToPaginatedModel<ProfileListMongoModel>();

                paged.ColumnDefinitions = ColumnFactory.GenerateColumnDefinitions<ProfileListMongoModel>(ActionEnum.List).ToArray();
                paged.Condition = new Condition();

                return Ok<PaginatedMongoModel<ProfileListMongoModel>>(paged);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [DeflateCompression]
        [ActionName("Profile")]
        public async Task<IHttpActionResult> GetProfile()
        {
            try
            {
                var obj = await this.Service.GetLoggedInAsync(User.Identity.Name);
                if (obj == null)
                    return NotFound();

                return Ok(obj);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}
