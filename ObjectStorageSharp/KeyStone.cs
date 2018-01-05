using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ObjectStorageSharp {
    public class KeyStone {
        public string AuthUrl { get; protected set; }

        protected KeyStone() { }
        public static async Task<KeyStone> Authenticate(string url, string tenant, string user, string pass) {
            var data = new JObject() as dynamic;
            data.tenantName = tenant;
            data.passwordCredentials = new JObject() as dynamic;
            data.passwordCredentials.username = user;
            data.passwordCredentials.password = pass;

            HttpResponseMessage result = await WebExtension.Post(url, data, authToken: null);
            if(result.StatusCode != System.Net.HttpStatusCode.OK) {
                Debug.WriteLine(result);
                throw new HttpRequestException(result.ToString());
            }

            Console.WriteLine(result.Content);
            return new KeyStone() {
                AuthUrl = url,
                // Token
            };
        }
    }
}
