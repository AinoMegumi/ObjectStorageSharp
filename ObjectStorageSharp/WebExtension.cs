using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ObjectStorageSharp {
    public static class WebExtension {
        private static HttpClient httpClient = new HttpClient();

        public static HttpClient AddHeader(this HttpClient client, string name, string value) {
            client.DefaultRequestHeaders.TryAddWithoutValidation(name, value);
            return client;
        }
        public static HttpClient AddAcceptHeader(this HttpClient client, string header = "application/json") => client.AddHeader("Accept", header);
        public static HttpClient AddContentHeader(this HttpClient client, string header = "application/json") => client.AddHeader("Content-Type", header);
        public static HttpClient AddAuthToken(this HttpClient client, string token) => (token != null) ? client.AddHeader("X-Auth-Token", token) : client;


        public static Task<HttpResponseMessage> Post(string url, string jsonStr, string authToken = null) {
            httpClient.AddContentHeader().AddAuthToken(authToken);

            var content = new StringContent(jsonStr, Encoding.UTF8, "application/json");
            return httpClient.PostAsync(url, content);
        }
        public static Task<HttpResponseMessage> Post(string url, object data, string authToken = null) => Post(url, JsonConvert.SerializeObject(data), authToken);
    }
}
