using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

namespace ObjectStorageSharp.Test {
    [TestClass]
    public class UnitTest {
        public const string KEYSTONE_BASE_URL = "https://identity.tyo1.conoha.io/v2.0/tokens";
        [TestMethod]
        public void Authenticate() {
            Task.Run(async () => {
                try {
                    var result = await KeyStone.Authenticate(KEYSTONE_BASE_URL, TestConfig.TENANT, TestConfig.USERNAME, TestConfig.PASSWORD);
                    Assert.IsNotNull(result);
                } catch (Exception ex) {
                    Assert.Fail(ex.Message);
                }
            }).GetAwaiter().GetResult();
        }
        [TestMethod]
        public void AuthStore() {
            Task.Run(async () => {
                try {
                    var keystone = await KeyStone.Authenticate(KEYSTONE_BASE_URL, TestConfig.TENANT, TestConfig.USERNAME, TestConfig.PASSWORD);
                    Assert.IsNotNull(keystone);

                    var jsonText = JsonConvert.SerializeObject(keystone.AuthData);
                    File.WriteAllText("auth.json", jsonText);
                } catch (Exception ex) {
                    Assert.Fail(ex.Message);
                }
            }).GetAwaiter().GetResult();
        }
        [TestMethod]
        public void GetContainerList() {
            Task.Run(async () => {
                try {
                    var keystone = await KeyStone.Authenticate(KEYSTONE_BASE_URL, TestConfig.TENANT, TestConfig.USERNAME, TestConfig.PASSWORD);
                    Assert.IsNotNull(keystone);
                    var os = ObjectStorage.FromKeyStone(keystone);
                    Assert.IsNotNull(os);

                    var result = await os.GetContainerList();
                } catch (Exception ex) {
                    Assert.Fail(ex.Message);
                }
            }).GetAwaiter().GetResult();
        }
        //[TestMethod]
        public void ObjectStorageQuota() {
            Task.Run(async () => {
                try {
                    var keystone = await KeyStone.Authenticate(KEYSTONE_BASE_URL, TestConfig.TENANT, TestConfig.USERNAME, TestConfig.PASSWORD);
                    var os = ObjectStorage.FromKeyStone(keystone);
                    await os.SetQuota(100);

                } catch (Exception ex) {
                    Assert.Fail(ex.Message);
                }
            }).GetAwaiter().GetResult();
        }
        [TestMethod]
        public void CreateContainer() {
            Task.Run(async () => {
                try {
                    var keystone = await KeyStone.Authenticate(KEYSTONE_BASE_URL, TestConfig.TENANT, TestConfig.USERNAME, TestConfig.PASSWORD);
                    var os = ObjectStorage.FromKeyStone(keystone);
                    var result = await os.CreateContainer("test-container", new Dictionary<string, string>() {
                        {"X-Web-Mode", "true" },
                        {"X-Container-Read", ".r:*" },
                    });
                    Console.WriteLine(result);
                } catch (Exception ex) {
                    Assert.Fail(ex.Message);
                }
            }).GetAwaiter().GetResult();
        }
        [TestMethod]
        public void DeleteContainer() {
            Task.Run(async () => {
                try {
                    var keystone = await KeyStone.Authenticate(KEYSTONE_BASE_URL, TestConfig.TENANT, TestConfig.USERNAME, TestConfig.PASSWORD);
                    var os = ObjectStorage.FromKeyStone(keystone);
                    var c = await os.CreateContainer("test-delete-container", new Dictionary<string, string>() {
                    });
                    var result = await os.DeleteContainer("test-delete-container");
                } catch (Exception ex) {
                    Assert.Fail(ex.Message);
                }
            }).GetAwaiter().GetResult();
        }
        [TestMethod]
        public void GetObjectList() {
            Task.Run(async () => {
                try {
                    var keystone = await KeyStone.Authenticate(KEYSTONE_BASE_URL, TestConfig.TENANT, TestConfig.USERNAME, TestConfig.PASSWORD);
                    var os = ObjectStorage.FromKeyStone(keystone);
                    var result = await os.GetObjectList("test", new Dictionary<string, string>() {
                    });
                    Console.WriteLine(result);
                } catch (Exception ex) {
                    Assert.Fail(ex.Message);
                }
            }).GetAwaiter().GetResult();
        }
        [TestMethod]
        public void GetObject() {
            Task.Run(async () => {
                try {
                    var keystone = await KeyStone.Authenticate(KEYSTONE_BASE_URL, TestConfig.TENANT, TestConfig.USERNAME, TestConfig.PASSWORD);
                    var os = ObjectStorage.FromKeyStone(keystone);
                    var result = await os.GetObject("test/test.jpg", new Dictionary<string, string>() {
                    });
                    Console.WriteLine(result);
                    var result2 = await os.GetObject("test/not_found", new Dictionary<string, string>() {
                    });
                    Console.WriteLine(result2);
                } catch (Exception ex) {
                    Assert.Fail(ex.Message);
                }
            }).GetAwaiter().GetResult();
        }
        [TestMethod]
        public void PutObject() {
            Task.Run(async () => {
                try {
                    var keystone = await KeyStone.Authenticate(KEYSTONE_BASE_URL, TestConfig.TENANT, TestConfig.USERNAME, TestConfig.PASSWORD);
                    var os = ObjectStorage.FromKeyStone(keystone);
                    var result = await os.GetObject("test/test.jpg", new Dictionary<string, string>() {
                    });
                    var tmpFileName = "test.jpg";
                    File.WriteAllBytes(tmpFileName, await result.Content.ReadAsByteArrayAsync());
                    //upload
                    var result2 = await os.PutObject("test", tmpFileName, dstName:"test2.jpg", headers: new Dictionary<string, string>() {
                    });
                    Console.WriteLine(result2);
                    File.Delete(tmpFileName);
                } catch (Exception ex) {
                    Assert.Fail(ex.Message);
                }
            }).GetAwaiter().GetResult();
        }

    }
}
