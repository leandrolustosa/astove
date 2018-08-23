using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Reflection;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Net;
using System.Web;

namespace Empresa.Projeto.HttpApiClient
{
    public sealed class WebApiClientContext
    {
        private WebApiClientContext() { }
        
        private static readonly Lazy<ConcurrentDictionary<Type, object>> _clients 
            = new Lazy<ConcurrentDictionary<Type, object>>(
                () => new ConcurrentDictionary<Type, object>(), isThreadSafe: true);

        private static readonly Lazy<HttpClient> _httpClient = new Lazy<HttpClient>(() => {
            //Assembly assembly = Assembly.GetExecutingAssembly();
            
            ICredentials credentials = CredentialCache.DefaultCredentials;
            HttpClient httpClient = HttpClientFactory.Create(innerHandler: new WebRequestHandler()
            {
                UseCookies = true,
                UseDefaultCredentials = true,
                Credentials = credentials,
                CookieContainer = new CookieContainer(),
                AutomaticDecompression = System.Net.DecompressionMethods.GZip
            });

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            httpClient.DefaultRequestHeaders.Add("X-UserAgent", string.Concat("DigiJobs", "( ", "1.0.0.0", ")"));

            return httpClient;
        }, isThreadSafe: true);

        internal ConcurrentDictionary<Type, object> Clients 
        { 
            get 
            { 
                return _clients.Value; 
            } 
        }

        internal Uri BaseUri { get; set; }
        internal string AuthorizationValue { get; set; }
        internal string UserId { get; set; }
        internal HttpClient HttpClient
        {
            get
            {
                if (!_httpClient.IsValueCreated)
                {
                    InitializeHttpClient();
                }

                return _httpClient.Value;
            }
        }

        public static WebApiClientContext Create(Action<WebApiClientConfigurationExpression> action)
        {
            var apiClientContext = new WebApiClientContext();
            var configurationExpression = new WebApiClientConfigurationExpression(apiClientContext);

            action(configurationExpression);

            return apiClientContext;
        }

        private void InitializeHttpClient()
        {
            if (BaseUri == null)
                throw new ArgumentNullException("BaseUri");

            _httpClient.Value.BaseAddress = BaseUri;

            if (string.IsNullOrEmpty(this.AuthorizationValue))
            {
                //MySql.Web.Security.MySqlSecurityDbContext context = new MySql.Web.Security.MySqlSecurityDbContext();
                //MySql.Data.MySqlClient.usuariosistema user = context.usuariosistema.FirstOrDefault(u => u.UserName == MySql.Web.Security.MySqlWebSecurity.CurrentUserName);

                //this.AuthorizationValue = user.Token;
            }

            _httpClient.Value.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", AuthorizationValue);
        }
    }
}
