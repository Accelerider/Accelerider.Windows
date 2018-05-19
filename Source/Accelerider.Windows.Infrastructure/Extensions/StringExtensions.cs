using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accelerider.Windows.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        #region Private members
        private static readonly MD5 Md5Algorithm = MD5.Create();
        private static readonly RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();
        private static readonly string RsaPublicKey = SuppressException(GetPublicKey);
        private static readonly byte[] DefaultKey = SystemInfo.InstallDate.ToMd5().ToBaseUTF8Bytes();
        private static readonly byte[] DefaultIv = SystemInfo.SerialNumber.ToMd5().ToBaseUTF8Bytes().Take(16).ToArray();

        private static string GetPublicKey()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "publickey.xml");
            if (!File.Exists(filePath))
                throw new FileNotFoundException("publickey.xml not found.");
            return File.ReadAllText(filePath);
        }

        private static string GetPrivateKey()
        {
            var filePath = Path.Combine(@"C:\Users\Dingp\Desktop\New Folder", "privatekey.xml");
            if (!File.Exists(filePath))
                throw new FileNotFoundException("privatekey.xml not found.");
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
        #endregion

        public static string GetJsonValue(this string text, string key) => JObject.Parse(text)?[key]?.Value<string>();

        public static T GetJsonValue<T>(this string text, string key) => JsonConvert.DeserializeObject<T>(JObject.Parse(text)?[key]?.Value<string>() ?? string.Empty);

        public static string ToMd5(this string text)
        {
            return BitConverter.ToString(Md5Algorithm.ComputeHash(Encoding.UTF8.GetBytes(text))).Replace("-", string.Empty).ToLower();
        }

        public static byte[] ToBaseUTF8Bytes(this string text) => Encoding.UTF8.GetBytes(text);

        public static string EncryptByRijndael(this string text, byte[] key = null, byte[] iv = null)
        {
            byte[] encryptedInfo;
            using (var rijndaelAlgorithm = new RijndaelManaged())
            {
                rijndaelAlgorithm.Key = key ?? DefaultKey;
                rijndaelAlgorithm.IV = iv ?? DefaultIv;
                var encryptor = rijndaelAlgorithm.CreateEncryptor(rijndaelAlgorithm.Key, rijndaelAlgorithm.IV);

                using (var msEncrypt = new MemoryStream())
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(text);
                    }
                    encryptedInfo = msEncrypt.ToArray();
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
                try
                {
                    using (var msDecrypt = new MemoryStream(cipherByte))
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        plainText = srDecrypt.ReadToEnd();
                    }
                }
                catch (CryptographicException)
                {
                    return text;
                }
            }
            return plainText;
        }

        public static string EncryptByRsa(this string text, string publicKeyXml = null)
        {
            if (RsaPublicKey == null) return text;
            Rsa.FromXmlString(publicKeyXml ?? RsaPublicKey);
            return Convert.ToBase64String(Rsa.Encrypt(Encoding.UTF8.GetBytes(text), false));
        }

        public static string DecryptByRsa(this string text, string privateKeyXml = null)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(privateKeyXml ?? GetPrivateKey());
            cipherbytes = rsa.Decrypt(Convert.FromBase64String(text), false);

            return Encoding.UTF8.GetString(cipherbytes);
        }
    }
}
