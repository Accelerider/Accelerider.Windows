using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BaiduPanDownloadWpf.Core
{
    public static class StringExtensions
    {
        private static readonly MD5 Md5Algorithm = MD5.Create();
        private static readonly RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();
        private static readonly string RsaPublicKey = SuppressException(GetPublicKey);
        private static readonly string SystemInfo = GetSystemInfo();
        private static readonly byte[] DefaultKey = ExtractSystemInfoAsBytes(SystemInfo, "InstallDate");
        private static readonly byte[] DefaultIv = ExtractSystemInfoAsBytes(SystemInfo, "SerialNumber").Take(16).ToArray();


        public static string ToMd5(this string text)
        {
            return BitConverter.ToString(Md5Algorithm.ComputeHash(Encoding.UTF8.GetBytes(text))).Replace("-", string.Empty).ToLower();
        }
        public static string EncryptByRijndael(this string text, byte[] key = null, byte[] iv = null)
        {
            byte[] encryptedInfo;
            using (var rijndaelAlgorithm = new RijndaelManaged())
            {
                rijndaelAlgorithm.Key = key ?? DefaultKey;
                rijndaelAlgorithm.IV = iv ?? DefaultIv;
                var encryptor = rijndaelAlgorithm.CreateEncryptor(rijndaelAlgorithm.Key, rijndaelAlgorithm.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }
                        encryptedInfo = msEncrypt.ToArray();
                    }
                }
            }
            return BitConverter.ToString(encryptedInfo).Replace("-", string.Empty);
        }
        public static string DecryptByRijndael(this string text, byte[] key = null, byte[] iv = null)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            string plainText;
            using (var rijndaelAlgorithm = new RijndaelManaged())
            {
                rijndaelAlgorithm.Key = key ?? DefaultKey;
                rijndaelAlgorithm.IV = iv ?? DefaultIv;
                var decryptor = rijndaelAlgorithm.CreateDecryptor(rijndaelAlgorithm.Key, rijndaelAlgorithm.IV);
                var cipherByte = new byte[text.Length / 2];
                for (int i = 0; i < cipherByte.Length; i++)
                {
                    cipherByte[i] = Convert.ToByte(text.Substring(i * 2, 2), 16);
                }
                using (var msDecrypt = new MemoryStream(cipherByte))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plainText;
        }
        public static string EncryptByRSA(this string text, string publicKeyXml = null)
        {
            if (RsaPublicKey == null) return text; // FOR OPEN SOURCE CODE.
            Rsa.FromXmlString(publicKeyXml ?? RsaPublicKey);
            return Convert.ToBase64String(Rsa.Encrypt(Encoding.UTF8.GetBytes(text), false));
        }

        private static string GetSystemInfo()
        {
            var result = new ManagementObjectSearcher(new ObjectQuery("SELECT * FROM Win32_OperatingSystem"));
            var strInfo = string.Empty;
            using (var collection = result.Get())
            {
                foreach (var item in collection)
                {
                    var managementObject = item as ManagementObject;
                    strInfo = managementObject?.GetText(TextFormat.Mof);
                    if (!string.IsNullOrEmpty(strInfo)) break;
                }
            }
            return strInfo;
        }
        private static byte[] ExtractSystemInfoAsBytes(string systemInfo, string infoKey)
        {
            var match = Regex.Match(systemInfo, $"{infoKey} ?= ?\"(.+?)\";");
            var md5 = match.Groups[1].Value.ToMd5();
            return Encoding.UTF8.GetBytes(md5);
        }
        private static string GetPublicKey()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "publickey.xml");
            if (!File.Exists(filePath))
                throw new FileNotFoundException("publickey.xml not found.");
            return File.ReadAllText(filePath);
        }

        private static T SuppressException<T>(Func<T> function) where T : class
        {
            try
            {
                return function?.Invoke();
            }
            catch
            {
                return null;
            }
        }
    }
}
