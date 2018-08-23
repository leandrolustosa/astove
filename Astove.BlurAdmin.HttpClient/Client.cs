using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AInBox.Astove.Core.Model;
using AInBox.Astove.Core.Attributes;
using WebApiDoodle.Net.Http.Client;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiDoodle.Net.Http.Client.Model;
using System.Net.Http.Formatting;
using AInBox.Astove.Core.Logging;
using System.Net;

namespace Empresa.Projeto.HttpApiClient
{
    public class Client<TModel, TRequestModel, TUpdateRequestModel> : HttpApiClient<TModel>, IClient<TModel, TRequestModel, TUpdateRequestModel>
        where TModel : class, IModel, IDto, new()
        where TRequestModel : class, IModel, IBindingModel, new()
        where TUpdateRequestModel : class, IModel, IBindingModel, new()
    {
        private string BaseUriTemplate { get; set; }
        private string BaseUriTemplateForSingle { get; set; }
        private readonly HttpClient _httpClient;
        private readonly ILog<TModel> logger;

        public Client(HttpClient httpClient, ILog<TModel> logger)
            : base(httpClient, MediaTypeFormatterCollection.Instance)
        {
            this._httpClient = httpClient;
            this.logger = logger;

            var attr = typeof(TModel).GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault();
            if (attr == null)
                throw new HttpApiRequestException(string.Format("O seu modelo {0} não possui definição do DataEntityAttribute", typeof(TModel).Name), System.Net.HttpStatusCode.NotImplemented);

            if (string.IsNullOrEmpty(attr.BaseUriTemplate))
                throw new HttpApiRequestException(string.Format("O seu modelo {0} não definiu a propriedade BaseUriTemplate do DataEntityAttribute", typeof(TModel).Name), System.Net.HttpStatusCode.NotImplemented);

            BaseUriTemplate = attr.BaseUriTemplate;
            BaseUriTemplateForSingle = string.Format("{0}/{1}", attr.BaseUriTemplate, "{id}");
        }

        private Task<TResult> HandleResponseAsync<TResult>(Task<HttpApiResponseMessage<TResult>> responseTask)
        {
            var model = responseTask.ContinueWith<TResult>(t => {
                var apiResponse = t.Result;                
                if (apiResponse.IsSuccess)
                {
                    return apiResponse.Model;
                }

                throw GetHttpApiRequestException(apiResponse);
            });

            return model;
        }

        private Task<PaginatedDto<TModel>> HandleResponseAsync(Task<HttpApiResponseMessage<PaginatedDto<TModel>>> responseTask)
        {
            var result = responseTask.ContinueWith<PaginatedDto<TModel>>(t =>
            {
                var apiResponse = t.Result;
                if (apiResponse.IsSuccess)
                {
                    return apiResponse.Model;
                }

                throw GetHttpApiRequestException(apiResponse);
            }); 

            return result;
        }

        private Task HandleResponseAsync(Task<HttpApiResponseMessage> responseTask)
        {
            var result = responseTask.ContinueWith(t =>
            {
                var apiResponse = t.Result;
                if (!apiResponse.IsSuccess)
                {
                    throw GetHttpApiRequestException(apiResponse);
                }
            });

            return result;
        }

        private Task HandleResponseAsync(Task<HttpResponseMessage> responseTask)
        {
            var result = responseTask.ContinueWith(t =>
            {
                var apiResponse = t;
                if (apiResponse.IsFaulted)
                {
                    throw GetHttpApiRequestException(apiResponse.Result);
                }
            });

            return result;
        }

        private HttpApiRequestException GetHttpApiRequestException(HttpApiResponseMessage apiResponse)
        {
            return new HttpApiRequestException(string.Format("{0}: {1}", (int)apiResponse.Response.StatusCode, apiResponse.Response.ReasonPhrase), apiResponse.Response.StatusCode, apiResponse.HttpError);
        }

        private HttpRequestException GetHttpApiRequestException(HttpResponseMessage apiResponse)
        {
            return new HttpRequestException(string.Format("{0}: {1}", (int)apiResponse.StatusCode, apiResponse.ReasonPhrase));
        }

        public Task<PaginatedDto<TModel>> GetEntitiesAsync(PaginatedRequestCommand request)
        {
            var parameters = new {
                page = request.Page,
                take = request.Take,
                type = request.Type,
                hasDefaultConditions = request.HasDefaultConditions,
                directions = (request.Directions == null) ? new string[0] : request.Directions,
                ordersBy = (request.OrdersBy == null) ? new string[0] : request.OrdersBy
            };

            var responseTask = base.GetAsync(BaseUriTemplate, parameters);

            return HandleResponseAsync(responseTask).ContinueWith(t => { return t.Result; });
        }

        public Task<TModel> GetEntityAsync(int entityId)
        {
            var parameters = new {
                id = entityId
            };

            var responseTask = base.GetSingleAsync(BaseUriTemplateForSingle, parameters);

            return HandleResponseAsync(responseTask).ContinueWith(t => { return t.Result; });
        }

        public Task AddEntityAsync(TRequestModel requestModel)
        {
            logger.Info("Add {0} entity {1}", BaseUriTemplate, requestModel.GetType().Name);
            var responseTask = base.PostAsync(BaseUriTemplate, requestModel);

            if (responseTask.Exception != null)
                logger.Error(responseTask.Exception);

            return HandleResponseAsync(responseTask).ContinueWith(t => { logger.Info("Add entity ContiniueWith {0} isFaulted {1} isCompleted {2} BaseUri {3}", (t.Exception != null) ? t.Exception.Message : "", t.IsFaulted, t.IsCompleted, BaseUriTemplate); });
        }

        public Task ExecuteActionPostAsync<T>(string baseUriTemplate, T requestModel)
            where T: IBindingModel
        {
            logger.Info("Execute action {0} entity {1}", baseUriTemplate, requestModel.GetType().Name);

            //CookieContainer cookieContainer = new CookieContainer();
            //CookieCollection col = cookieContainer.GetCookies(new Uri(_httpClient.BaseAddress.AbsoluteUri + baseUriTemplate));
            //string loginUrl = _httpClient.BaseAddress.AbsoluteUri + "account/login";

            //var response = _httpClient.GetAsync(loginUrl).ContinueWith(t => { return t.Result; }).Result;
            //var content = response.Content.ReadAsStringAsync().ContinueWith(t => { return t.Result; }).Result;
            //var requestVerificationToken = ParseRequestVerificationToken(content);

            //content = requestVerificationToken + "&UserName=Leandro&Password=Zwk4Yfba&RememberMe=false";
            //response = _httpClient.PostAsync(loginUrl, new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded")).ContinueWith(t => { return t.Result; }).Result;
            //content = response.Content.ReadAsStringAsync().ContinueWith(t => { return t.Result; }).Result;
            //requestVerificationToken = ParseRequestVerificationToken(content);

            //ICredentials credentials = CredentialCache.DefaultCredentials;

            var responseTask = base.PostAsync(baseUriTemplate, requestModel);

            if (responseTask.Exception != null)
                logger.Error(responseTask.Exception);

            return HandleResponseAsync(responseTask).ContinueWith(t => { logger.Info("Execute actin ContiniueWith {0} isFaulted {1} isCompleted {2} BaseUri {3}", (t.Exception != null) ? t.Exception.Message : "", t.IsFaulted, t.IsCompleted, baseUriTemplate); });
        }

        public Task UpdateEntityAsync(int entityId, TUpdateRequestModel requestModel)
        {
            logger.Info("Update {0} entity {1} id {2}", BaseUriTemplate, requestModel.GetType().Name, entityId);
            
            var parameters = new {
                id = entityId
            };

            var responseTask = base.PutAsync(BaseUriTemplateForSingle, requestModel, parameters);

            if (responseTask.Exception != null)
                logger.Error(responseTask.Exception);

            return HandleResponseAsync(responseTask).ContinueWith(t => { });
        }

        public Task ExecuteActionPutAsync<T>(string baseUriTemplate, T requestModel)
            where T : IBindingModel
        {
            logger.Info("Execute put action {0} entity {1}", baseUriTemplate, requestModel.GetType().Name);

            var responseTask = base.PutAsync(baseUriTemplate, requestModel);

            if (responseTask.Exception != null)
                logger.Error(responseTask.Exception);

            return HandleResponseAsync(responseTask).ContinueWith(t => { logger.Info("Add entity ContiniueWith {0} isFaulted {1} isCompleted {2} BaseUri {3}", (t.Exception != null) ? t.Exception.Message : "", t.IsFaulted, t.IsCompleted, BaseUriTemplate); });
        }

        public Task RemoveEntityAsync(int entityId)
        {
            var parameters = new {
                id = entityId
            };

            var responseTask = base.DeleteAsync(BaseUriTemplateForSingle, parameters);

            return HandleResponseAsync(responseTask).ContinueWith(t => { });
        }

        private string ParseRequestVerificationToken(string content)
        {
            var startIndex = content.IndexOf("__RequestVerificationToken");

            if (startIndex == -1)
            {
                return null;
            }

            content = content.Substring(startIndex, content.IndexOf("\" />", startIndex) - startIndex);
            content = content.Replace("\" type=\"hidden\" value=\"", "=");
            return content;
        }
    }
}
