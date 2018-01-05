using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ObjectStorageSharp {
    public class KeyStone {
        public KeyStoneResult AuthData { get; private set; }
        /// <summary>
        /// X-Auth-Tokenに付与するトークン
        /// </summary>
        public string Token => AuthData?.access?.token?.id ?? null;
        /// <summary>
        /// トークンの有効期限が切れている
        /// </summary>
        public bool IsTokenExpired => (AuthData?.access?.token?.expires ?? DateTime.MinValue) < DateTime.Now;

        public KeyStone(KeyStoneResult authData) {
            this.AuthData = authData;
        }
        public KeyStone(string authDataJson) {
            this.AuthData = JsonConvert.DeserializeObject<KeyStoneResult>(authDataJson);
        }
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
                throw new HttpRequestException(content);
            }

            return new KeyStone() {
                AuthData = JsonConvert.DeserializeObject<KeyStoneResult>(content),
            };
        }

    }
}
