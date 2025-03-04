﻿using Newtonsoft.Json;
using ObjectStorageSharp.Response;
using System;
using System.Collections.Generic;
using System.IO;
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
        public async Task<ContainerInfo[]> GetContainerList() {
            var result = await WebExtension.Get(BaseUrl, KeyStone.Token);
            switch (result.StatusCode) {
                case HttpStatusCode.NoContent:
                    return Array.Empty<ContainerInfo>();
                case HttpStatusCode.OK:
                    break;
                default:
                    throw new HttpRequestException(await result.Content.ReadAsStringAsync());
            }
            var list = JsonConvert.DeserializeObject<ContainerInfo[]>(await result.Content.ReadAsStringAsync());
            return list;
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
            if (result.StatusCode != HttpStatusCode.NoContent) throw new HttpRequestException(await result.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// コンテナ作成
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> CreateContainer(string containerName, IDictionary<string, string> headers = null) {
            var result = await WebExtension.Put($"{BaseUrl}/{containerName}", null, authToken: KeyStone.Token
                , headers: headers);
            switch (result.StatusCode) {
                case HttpStatusCode.Accepted:
                case HttpStatusCode.Created:
                case HttpStatusCode.NoContent:
                    break;
                default:
                    throw new HttpRequestException(await result.Content.ReadAsStringAsync());
            }
            return result;
        }
        /// <summary>
        /// コンテナ削除
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeleteContainer(string containerName) {
            var result = await WebExtension.Delete($"{BaseUrl}/{containerName}", authToken: KeyStone.Token);
            if (result.StatusCode != System.Net.HttpStatusCode.NoContent) {
                throw new HttpRequestException(await result.Content.ReadAsStringAsync());
            }
            return result;
        }
        /// <summary>
        /// オブジェクト一覧
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public async Task<ObjectInfo[]> GetObjectList(string containerName, IDictionary<string, string> headers = null) {
            var result = await WebExtension.Get($"{BaseUrl}/{containerName}", authToken: KeyStone.Token, headers: headers);
            switch (result.StatusCode) {
                case HttpStatusCode.NoContent:
                    return Array.Empty<ObjectInfo>();
                case HttpStatusCode.OK:
                    break;
                default:
                    throw new HttpRequestException(await result.Content.ReadAsStringAsync());
            }
            var list = JsonConvert.DeserializeObject<ObjectInfo[]>(await result.Content.ReadAsStringAsync());
            return list;
        }
        /// <summary>
        /// オブジェクトを取得します
        /// </summary>
        /// <param name="path"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetObject(string path, IDictionary<string, string> headers = null) {
            var result = await WebExtension.Get($"{BaseUrl}/{path}", authToken: KeyStone.Token, headers: headers);
            switch (result.StatusCode) {
                case HttpStatusCode.OK:
                case HttpStatusCode.NotFound:
                    break;
                default:
                    throw new HttpRequestException(await result.Content.ReadAsStringAsync());
            }
            return result;
        }
        /// <summary>
        /// ファイルをアップロードします
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="filePath"></param>
        /// <param name="contentType"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PutObject(string containerName, string filePath, string dstName = null, IDictionary<string, string> headers = null) {
            dstName ??= Path.GetFileName(filePath);
            var result = await WebExtension.PutFile($"{BaseUrl}/{containerName}/{dstName}", filePath, authToken: KeyStone.Token, headers: headers);
            switch (result.StatusCode) {
                case HttpStatusCode.Created:
                    break;
                default:
                    throw new HttpRequestException(await result.Content.ReadAsStringAsync());
            }
            return result;
        }

        public async Task<HttpResponseMessage> DeleteObject(string containerName, string fileName, IDictionary<string, string> headers = null) {
            var result = await WebExtension.Delete($"{BaseUrl}/{containerName}/{fileName}", authToken: KeyStone.Token, headers: headers);
            switch (result.StatusCode) {
                case HttpStatusCode.NoContent:
                case HttpStatusCode.NotFound:
                    break;
                default:
                    throw new HttpRequestException(await result.Content.ReadAsStringAsync());
            }
            return result;
        }
    }


}
