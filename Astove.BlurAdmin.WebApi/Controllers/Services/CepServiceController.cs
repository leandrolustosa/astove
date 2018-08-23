using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Net.Http;
using System.Net;

namespace Astove.BlurAdmin.WebApi.Controllers.Services
{
    public class CepServiceController : ApiController
    {
        [ActionName("DefaultAction")]
        public virtual IHttpActionResult Get(string cep)
        {
            try
            {
                ByJGWebService.webservice service = new ByJGWebService.webservice();
                var username = System.Configuration.ConfigurationManager.AppSettings["byjgusername"];
                var password = System.Configuration.ConfigurationManager.AppSettings["byjgpassword"];

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    return BadRequest("Usuário e senha ByJG não estão configurados");

                var obj = service.ObterLogradouro(username, password, cep);

                return Ok<ByJGWebService.LogradouroModel>(obj);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        protected List<string> Errors(Exception ex)
        {
            var errors = new List<string>();
            errors.Add(ex.Message);
            if (ex.InnerException != null)
                errors.Add(ex.InnerException.Message);

            return errors;
        }

        protected string ErrorMessage(Exception ex)
        {
            var error = ex.Message;
            if (ex.InnerException != null)
                error = ex.InnerException.Message;

            return error;
        }
    }
}
