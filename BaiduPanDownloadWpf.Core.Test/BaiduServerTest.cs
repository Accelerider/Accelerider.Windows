using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaiduPanDownloadWpf.Core;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.Core.Test
{
    [TestClass]
    public class BaiduServerTest
    {
        [TestMethod]
        public async Task LoginTest()
        {
            using (var baiduServer = new BaiduServer())
            {
                await baiduServer.SetUsernameAsync("FuckBaidu");
                if (baiduServer.HasVerificationCode)
                {
                    //await baiduServer.SetVerificationCodeAsync("grf3rgf");
                }
            }
        }
    }
}
