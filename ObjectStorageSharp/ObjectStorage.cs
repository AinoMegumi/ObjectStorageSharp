using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ObjectStorageSharp {
    /// <summary>
    /// https://www.conoha.jp/docs/swift-show_account_details_and_list_containers.html
    /// </summary>
    public class ObjectStorage {
        public KeyStone KeyStone { get; private set; }
        public string BaseUrl { get; private set; }

        protected ObjectStorage() { }
        public static ObjectStorage FromKeyStone(KeyStone ks) {
            var os = new ObjectStorage();
            if (ks.IsTokenExpired) throw new ArgumentException("キーの有効期限が切れています");

            os.KeyStone = ks;
            os.BaseUrl = ks.AuthData.access.serviceCatalog
                           .FirstOrDefault(x => x.type.Equals("object-store"))
                           ?.endpoints?.FirstOrDefault()?.publicURL ?? null;
            if (os.BaseUrl == null) throw new ArgumentException("オブジェクトストレージがサービスカタログに含まれていません");

            return os;
        }
        /// <summary>
        /// 認証アクセスが正常にできるか
        /// </summary>
        /// <returns></returns>
        public async Task Test() {
            var result = await WebExtension.Get(BaseUrl, KeyStone.Token);
            var isOK = result.StatusCode == System.Net.HttpStatusCode.OK || result.StatusCode == System.Net.HttpStatusCode.NoContent;
            if (!isOK) throw new HttpRequestException(await result.Content.ReadAsStringAsync());
        }
        /// <summary>
        /// アカウントクォータ設定
        /// </summary>
        /// <returns></returns>
        public async Task SetQuota(int gigabyte) {
            var result = await WebExtension.Post(BaseUrl, null, authToken: KeyStone.Token
                , headers: new Dictionary<string, string>(){
                    { "X-Account-Meta-Quota-Giga-Bytes", $"{gigabyte}" },
                });
            var str = await result.Content.ReadAsStringAsync();
            if (result.StatusCode != System.Net.HttpStatusCode.NoContent) throw new HttpRequestException(await result.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// コンテナ作成
        /// </summary>
        /// <param name="gigabyte"></param>
        /// <returns></returns>
        public async Task<HttpStatusCode> CreateContainer(string name, IDictionary<string, string> headers = null) {
            var result = await WebExtension.Put(BaseUrl, null, authToken: KeyStone.Token
                , headers: headers);
            var str = await result.Content.ReadAsStringAsync();
            return result.StatusCode;
        }

    }
}
