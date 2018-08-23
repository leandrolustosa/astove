using Microsoft.Owin.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;

namespace Astove.BlurAdmin.WebApi.Tests
{
    public class BaseControllerTest : IDisposable
    {
        private TestServer _server;
        private string _bearerToken;
        
        public TestServer Server { get { return _server; } }
        public string BearerToken { get { return _bearerToken; } }

        public BaseControllerTest()
        {
            _server = TestServer.Create<OwinStartup>();

            Task.WaitAll(Authenticate());
        }

        private async Task Authenticate()
        {
            var data = new List<KeyValuePair<string, string>>();
            data.Add(new KeyValuePair<string, string>("grant_type", "password"));
            data.Add(new KeyValuePair<string, string>("username", "admin@astove.com.br"));
            data.Add(new KeyValuePair<string, string>("password", "password"));
            var tokenResponse = await _server.HttpClient.PostAsync("/Token", new FormUrlEncodedContent(data));
            var bearerTokenJson = await tokenResponse.Content.ReadAsStringAsync();
            _bearerToken = string.Concat("Bearer ", JsonConvert.DeserializeObject<BearerToken>(bearerTokenJson).access_token);
        }

        public RequestBuilder CreateRequest(string path)
        {
            return _server.CreateRequest(path);
        }

        public RequestBuilder CreateRequestAuthenticated(string path)
        {
            return CreateRequest(path).AddHeader("Authorization", _bearerToken);
        }

        public RequestBuilder CreateRequestWithData<T>(string path, T model)
        {
            return CreateRequest(path).And(r => r.Content = new ObjectContent(typeof(T), model, new JsonMediaTypeFormatter()));
        }

        public RequestBuilder CreateRequestAuthenticatedWithData<T>(string path, T model)
        {
            return CreateRequestAuthenticated(path).And(r => r.Content = new ObjectContent(typeof(T), model, new JsonMediaTypeFormatter()));
        }

        public void Dispose()
        {
            if (_server != null)
                _server.Dispose();
        }
	}

	internal class BearerToken
	{
		public string access_token { get; set; }
	}
}
