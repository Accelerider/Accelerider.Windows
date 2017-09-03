using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.Core.Tools
{
    internal static class FileTools
    {
        public static async Task<string> GetMd5HashFromFile(string path)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var bufferSize = 1024 * 16;  
                    var buffer = new byte[bufferSize];
                    Stream inputStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                    HashAlgorithm hashAlgorithm = new MD5CryptoServiceProvider();
                    var readLength = 0; 
                    var output = new byte[bufferSize];
                    while ((readLength = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        hashAlgorithm.TransformBlock(buffer, 0, readLength, output, 0);
                    }
                    hashAlgorithm.TransformFinalBlock(buffer, 0, 0);
                    var md5 = BitConverter.ToString(hashAlgorithm.Hash);
                    hashAlgorithm.Clear();
                    inputStream.Close();
                    md5 = md5.Replace("-", "");
                    return md5.ToLower();
                }
                catch
                {
                    return null;
                }
            });
        }
    }
}
