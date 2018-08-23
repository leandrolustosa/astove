using AInBox.Astove.Core.Http;
using AInBox.Astove.Core.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AInBox.Astove.Core.Messaging
{
    public class GoogleCloudMessagingManager
    {
        private const string GOOGLE_API_CGM_BASE_ADDRESS = "https://android.googleapis.com";
        private const string GOOGLE_API_CGM_SEND_PATH = "/gcm/send";
        private const string JSON_MEDIA_TYPE = "application/json";

        public async Task<GoogleResponseResult> SendMessage(int plataforma, GoogleMessage message, string badge = "0")
        {
            var result = new GoogleResponseResult();

            try
            {
                var httpClient = HttpClientFactory.Create();

                var apiKey = System.Configuration.ConfigurationManager.AppSettings["gcm_api_key"];
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Concat("key=", apiKey));
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", JSON_MEDIA_TYPE);

                httpClient.BaseAddress = new Uri(GOOGLE_API_CGM_BASE_ADDRESS);

                if (plataforma == (int)Plataforma.iOS)
                {
                    message.content_available = true;
                    message.priority = "high";

                    message.notification.badge = badge;
                    message.notification.sound = "default";
                }

                var json = JsonConvert.SerializeObject(message, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                var response = await httpClient.PostAsync(GOOGLE_API_CGM_SEND_PATH, new StringContent(json, Encoding.UTF8, JSON_MEDIA_TYPE));

                result = await response.Content.ReadAsAsync<GoogleResponseResult>();
            }
            catch
            {
                result = new GoogleResponseResult { success = 0, failure = 1 };
            }

            return result;
        }

    }
}
