using AInBox.Astove.Core.Controllers;
using AInBox.Astove.Core.Data;
using AInBox.Astove.Core.Enums;
using AInBox.Astove.Core.Model;
using Autofac;
using Astove.BlurAdmin.Data;
using Astove.BlurAdmin.Model;
using Astove.BlurAdmin.Model.Domain;
using Astove.BlurAdmin.Services;
using Microsoft.AspNet.Identity;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http;
using System.Text;
using AInBox.Astove.Core.Extensions;
using MongoDB.Driver;
using AInBox.Astove.Core.List;
using AInBox.Astove.Core.Options;
using AInBox.Astove.Core.Options.EnumDomainValues;
using AInBox.Astove.Core.Attributes;
using AInBox.Astove.Core.Service;

namespace Astove.BlurAdmin.WebApi.Controllers
{
    [Authorize]
    public class SearchController : WebApiCustomController<Search, BaseModel>
    {
        private readonly IEntityService<Pessoa> pessoaService;

        public SearchController(IComponentContext container, IEntityService<Search> service, IEntityService<Pessoa> pessoaService) : base(container, service)
        {
            this.pessoaService = pessoaService;
        }

        [AllowAnonymous]
        [ActionName("AtualizarSearchs")]
        public async Task<IHttpActionResult> GetAtualizarSearchs(string token)
        {
            try
            {
                if (!token.Equals(System.Configuration.ConfigurationManager.AppSettings["Token"], StringComparison.InvariantCultureIgnoreCase))
                    return Unauthorized();

                var sb = new StringBuilder("");
                try
                {
                    await this.Service.AtualizarSearchs();
                }
                catch (Exception ex)
                {
                    sb.AppendLine(string.Format("Atualizar Searchs: {0}", ex.GetExceptionMessage()));
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

        [DeflateCompression]
        [ActionName("GetSearchs")]
        public async Task<IHttpActionResult> GetSearchs(string text)
        {
            try
            {
                var email = User.Identity.Name;
                var user = this.UserManager.FindByEmail(email);
                var roles = await this.UserManager.GetRolesAsync(user.Id);
                var result = await this.Service.GetListaSearchResultModel(roles, text);
                return Ok<ListaSearchResultModel>(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("DefaultAction")]
        public async Task<IHttpActionResult> PostSearch(PostSearchBindingModel model)
        {
            try
            {
                var email = User.Identity.Name;
                var usuario = await pessoaService.GetLoggedInAsync(email);
                model.ProfileId = int.Parse(usuario.ParentId);

                var result = this.Service.AddSearchBindingModel(model);

                return Ok<BaseResultModel>(new BaseResultModel { IsValid = true, Message = string.Empty });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
