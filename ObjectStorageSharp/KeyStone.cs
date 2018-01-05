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
            data.auth = new JObject() as dynamic;
            data.auth.tenantName = tenant;
            data.auth.passwordCredentials = new JObject() as dynamic;
            data.auth.passwordCredentials.username = user;
            data.auth.passwordCredentials.password = pass;

            HttpResponseMessage result = await WebExtension.Post(url, data, authToken: null);
            var content = await result.Content.ReadAsStringAsync();
            if (result.StatusCode != System.Net.HttpStatusCode.OK) {
                throw new HttpRequestException(result.ToString());
            }

            return new KeyStone() {
                AuthUrl = url,
                // Token
            };
        }
    }
}
