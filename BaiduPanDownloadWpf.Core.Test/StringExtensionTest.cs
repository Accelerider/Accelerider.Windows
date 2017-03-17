using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaiduPanDownloadWpf.Core;
using System.Security.Cryptography;
using System.IO;

namespace BaiduPanDownloadWpf.Core.Test
{
    //[TestClass]
    public class StringExtensionTest
    {
        private readonly List<string> _testTexts = new List<string>
        {
            "",
            "a",
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", // RSA Maximum allowable length: 117
            "~!@#$%^&*()_+",
            "`1234567890-=",
            "qwertyuiop[]\\asdfghjkl;'zxcvbnm,./",
            "QWERTYUIOP{}|ASDFGHJKL:\"ZXCVBNM<>?",
        };


        //[TestMethod]
        public void ToMd5Test()
        {
            const string password = "123456789";
            var md5 = password.ToMd5();
            Assert.IsTrue(md5 == "25f9e794323b453885f5181f1b624d0b".ToUpper());
        }

        //[TestMethod]
        public void EnDecryptRijndaelTest()
        {
            foreach (var item in _testTexts)
            {
                var en = item.EncryptByRijndael();
                var de = en.DecryptByRijndael();
                Assert.IsTrue(item == de);
            }
        }

        //[TestMethod]
        public void EnDecryptRSATest()
        {
            foreach (var item in _testTexts)
            {
                var en = item.EncryptByRSA();
                //var de = en.DecryptByRSA();
                //Assert.IsTrue(item == de);
            }
        }

        //[TestMethod]
        public void DecryptEmptyStringTest()
        {
            //string.Empty.DecryptByRijndael();
            //string.Empty.DecryptByRSA();
        }

        //[TestMethod]
        public void GenerateRSAKey()
        {
            var rsa = new RSACryptoServiceProvider();
            var publicKey = rsa.ToXmlString(false);
            var privateKey = rsa.ToXmlString(true);
            var publicKeyPath = Path.Combine(Directory.GetCurrentDirectory(), "publickey.xml");
            var privateKeyPath = Path.Combine(Directory.GetCurrentDirectory(), "privatekey.xml");
            File.WriteAllText(publicKeyPath, publicKey);
            File.WriteAllText(privateKeyPath, privateKey);
        }
    }
}
