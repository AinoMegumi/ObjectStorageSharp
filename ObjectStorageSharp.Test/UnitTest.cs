using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace ObjectStorageSharp.Test {
    [TestClass]
    public class UnitTest {
        [TestMethod]
        public void Authenticate() {
            const string url = "https://ident-r1nd1001.cnode.jp/v2.0/tokens";
            Task.Run(async () => {
                try {
                    var result = await KeyStone.Authenticate(url, TestConfig.TENANT, TestConfig.USERNAME, TestConfig.PASSWORD);
                    Assert.IsNotNull(result);
                } catch (Exception ex) {
                    Assert.Fail(ex.Message);
                }
            }).GetAwaiter().GetResult();
        }
    }
}
