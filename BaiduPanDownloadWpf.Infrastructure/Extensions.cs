using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BaiduPanDownloadWpf.Infrastructure
{
    public static class StringExtensions
    {
        private static readonly MD5 Md5Algorithm = MD5.Create();
        private static readonly RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();
        private static readonly string RsaPublicKey = GetPublicKey();//SuppressException(GetPublicKey);
        public static readonly byte[] DefaultKey = SystemInfo.InstallDate.ToMd5().ToBaseUTF8Bytes();
        public static readonly byte[] DefaultIv = SystemInfo.SerialNumber.ToMd5().ToBaseUTF8Bytes().Take(16).ToArray();


        public static byte[] ToBaseUTF8Bytes(this string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }

        #region Encrypt and Decrypt
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
            Rsa.FromXmlString(publicKeyXml ?? RsaPublicKey);
            return Convert.ToBase64String(Rsa.Encrypt(Encoding.UTF8.GetBytes(text), false));
        }
        #endregion


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

    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }
    }

    public static class SystemInfo
    {
        private static readonly string _systemInfo = GetSystemInfo();


        public static readonly string Caption = ExtractSystemInfo("Caption");
        public static readonly string CSName = ExtractSystemInfo("CSName");
        public static readonly string InstallDate = ExtractSystemInfo("InstallDate");
        public static readonly string OSArchitecture = ExtractSystemInfo("OSArchitecture");
        public static readonly string SerialNumber = ExtractSystemInfo("SerialNumber");
        public static readonly string Version = ExtractSystemInfo("Version");


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
        private static string ExtractSystemInfo(string infoKey)
        {
            return Regex.Match(_systemInfo, $"{infoKey} ?= ?\"(.+?)\";").Groups[1].Value;
        }
    }
}
