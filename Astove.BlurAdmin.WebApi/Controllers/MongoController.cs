using AInBox.Astove.Core.Controllers;
using AInBox.Astove.Core.Model;
using AInBox.Astove.Core.Service;
using Autofac;
using Astove.BlurAdmin.Data;
using Astove.BlurAdmin.Model;
using Astove.BlurAdmin.Services;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Astove.BlurAdmin.WebApi.Controllers
{
    [Authorize]
    public class MongoController : WebApiCustomController<Pessoa, BaseModel>
    {
        private readonly IEntityService<Estado> estadoService;
        private readonly IEntityService<Cidade> cidadeService;
        private readonly IEntityService<Empresa> empresaService;
        private readonly IEntityService<Configuracao> configuracaoService;
        private readonly IEntityService<Search> searchService;

        public MongoController(IComponentContext container, IEntityService<Pessoa> service, IEntityService<Estado> estadoService, IEntityService<Cidade> cidadeService, IEntityService<Search> searchService, IEntityService<Empresa> empresaService, IEntityService<Configuracao> configuracaoService) : base(container, service)
        {
            this.estadoService = estadoService;
            this.cidadeService = cidadeService;
            this.searchService = searchService;
            this.empresaService = empresaService;
            this.configuracaoService = configuracaoService;
        }

        [AllowAnonymous]
        [ActionName("Atualizar")]
        public async Task<IHttpActionResult> GetAtualizar(string token)
        {
            if (!token.Equals(System.Configuration.ConfigurationManager.AppSettings["Token"], StringComparison.InvariantCultureIgnoreCase))
                return Unauthorized();

            var sb = new StringBuilder("");
            try
            {
                await this.Service.ReloadMongoCollection(sb);
                await this.estadoService.ReloadMongoCollection(sb);
                await this.cidadeService.ReloadMongoCollection(sb);
                await this.empresaService.ReloadMongoCollection(sb);
                await this.configuracaoService.ReloadMongoCollection(sb);

                await searchService.AtualizarSearchs();
                await this.Service.AtualizarPermissoes(this.UserManager, this.RoleManager);
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(sb.ToString()))
                    sb.AppendLine("Serviço: http://auditoria.grupopianna.com.br/api/ac/mongo/atualizar?token=fjdshf7854-9a87a3sdfs245kjhsda-9s324sd875sjdfh");

                SetErrorMessage(sb, ex);
            }

            return ProcessResult("Grupo Pianna - Falha ao tentar atualizar lojas", sb);
        }

        [NonAction]
        public void SetErrorMessage(StringBuilder sb, Exception ex)
        {
            sb.AppendLine(string.Concat("Source: ", (ex.InnerException == null) ? ex.Source : ex.InnerException.Source));
            sb.AppendLine(string.Concat("Message: ", (ex.InnerException == null) ? ex.Message : ex.InnerException.Message));
            sb.AppendLine(string.Concat("Em: ", DateTime.Now.ToString()));
            sb.AppendLine(string.Empty);
        }

        [NonAction]
        public void SendEmail(string assunto, string body)
        {
            var m = new System.Net.Mail.MailMessage();
            m.To.Add("leandro@ainbox.com.br");
            m.Subject = assunto;

            m.Body = body;

            var s = new System.Net.Mail.SmtpClient();
            s.Send(m);
        }

        [NonAction]
        public IHttpActionResult ProcessResult(string assunto, StringBuilder sb)
        {
            if (sb.ToString().Equals(""))
            {
                return Ok();
            }
            else
            {
                SendEmail(assunto, sb.ToString());
                return BadRequest(sb.ToString());
            }
        }
    }
}
