using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Web;
using System.Net;

namespace Empresa.Projeto.HttpApiClient
{
    public class WebApiClientConfigurationExpression
    {
        private readonly WebApiClientContext _apiClientContext;

        internal WebApiClientConfigurationExpression(WebApiClientContext apiClientContext)
        {
            if (apiClientContext == null)
                throw new ArgumentNullException("apiClientContext");

            _apiClientContext = apiClientContext;
        }

        public WebApiClientConfigurationExpression SetCredentialsFromAppSetting(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("username");

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            username = "username";
            password = "password";

            //MySql.Web.Security.MySqlSecurityDbContext context = new MySql.Web.Security.MySqlSecurityDbContext();
            //MySql.Data.MySqlClient.usuariosistema user = context.usuariosistema.FirstOrDefault(u => u.UserName == MySql.Web.Security.MySqlWebSecurity.CurrentUserName);

            //_apiClientContext.AuthorizationValue = user.Token;
            //_apiClientContext.AuthorizationValue = EncodeToBase64(string.Format("{0}:{1}", username, password));

            return this;
        }

        public WebApiClientConfigurationExpression ConnectTo(string baseUri)
        {
            if (string.IsNullOrEmpty(baseUri))
                throw new ArgumentNullException("baseUri");

            _apiClientContext.BaseUri = new Uri(baseUri);

            return this;
        }

        private string EncodeToBase64(string value) 
        {
            byte[] toEncodeAsBytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(toEncodeAsBytes);
        }    
    }
}
