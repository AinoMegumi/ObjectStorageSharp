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

        public static Task<HttpResponseMessage> Do(
            Func<string, HttpContent, Task<HttpResponseMessage>> method,
            string url, string jsonStr, string authToken = null
            , IDictionary<string, string> headers = null
            ) {
            // これやらないと追記されていく
            httpClient.DefaultRequestHeaders.Clear();
            // 最初からAccept/Content-Typeは付与しているけど上書き可
            httpClient.AddContentHeader().AddAcceptHeader().AddAuthToken(authToken);
            if (headers != null) {
                foreach (var h in headers) {
                    httpClient.AddHeader(h.Key, h.Value);
                }
            }
            var content = new StringContent(jsonStr ?? "", Encoding.UTF8, "application/json");
            return method(url, content);
        }

        public static Task<HttpResponseMessage> Post(
            string url, string jsonStr, string authToken = null
            , IDictionary<string, string> headers = null
            ) => Do(httpClient.PostAsync, url, jsonStr, authToken, headers);
        public static Task<HttpResponseMessage> Post(
                string url, object data, string authToken = null
            , IDictionary<string, string> headers = null
            ) => Do(httpClient.PostAsync, url, JsonConvert.SerializeObject(data), authToken, headers);

        public static Task<HttpResponseMessage> Get(
           string url, string authToken = null
            , IDictionary<string, string> headers = null
           ) => Do((u, c) => httpClient.GetAsync(u), url, null, authToken, headers);

        public static Task<HttpResponseMessage> Put(
            string url, string jsonStr, string authToken = null
            , IDictionary<string, string> headers = null
            ) => Do(httpClient.PutAsync, url, jsonStr, authToken, headers);
        public static Task<HttpResponseMessage> Put(
                string url, object data, string authToken = null
            , IDictionary<string, string> headers = null
            ) => Do(httpClient.PutAsync, url, JsonConvert.SerializeObject(data), authToken, headers);

        public static Task<HttpResponseMessage> Delete(string url, string authToken = null) =>
            Do((u, c) => httpClient.DeleteAsync(u), url, null, authToken);
    }
}
