using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using AInBox.Astove.Core.Service;
using Autofac;
using System.Web;
using System.Web.Http.ModelBinding;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using AInBox.Astove.Core.Security;
using AInBox.Astove.Core.Data;
using System.Text;

namespace AInBox.Astove.Core.Controllers
{
    public class BaseMongoApiController<TEntity> : ApiController, IAstoveApiController
        where TEntity : class, IEntity, new()
    {
        private readonly IComponentContext _container;
        private readonly IEntityService<TEntity> _service;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? Request.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }

        public IComponentContext Container
        {
            get { return this._container; }
        }

        public IEntityService<TEntity> Service
        {
            get { return this._service; }
        }

        public BaseMongoApiController()
        {
        }

        public BaseMongoApiController(IComponentContext container, IEntityService<TEntity> service)
        {
            this._container = container;
            this._service = service;
        }

        public BaseMongoApiController(IComponentContext container, IEntityService<TEntity> service, ApplicationUserManager userManager)
        {
            this._container = container;
            this._service = service;
            UserManager = userManager;
        }

        #region Non Actions

        [NonAction]
        public List<string> Errors(ModelStateDictionary modelState)
        {
            var errors = new List<string>();
            foreach (var state in modelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
            }

            return errors;
        }

        [NonAction]
        public List<string> Errors(Exception ex)
        {
            var errors = new List<string>();
            errors.Add(ex.Message);
            if (ex.InnerException != null)
                errors.Add(ex.InnerException.Message);

            return errors;
        }

        [NonAction]
        public string ErrorMessage(ModelStateDictionary modelState)
        {
            var errors = Errors(modelState);
            return string.Join(", ", errors.ToArray());
        }

        [NonAction]
        public string ErrorMessage(Exception ex)
        {
            var error = ex.Message;
            if (ex.InnerException != null)
                error = ex.InnerException.Message;

            return error;
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

        #endregion
    }
}
