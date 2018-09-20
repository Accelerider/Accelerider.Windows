using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Accelerider.Windows.RemoteReference
{
    internal class RemoteRefDownloader
    {
        private const string TempDirectoryName = "temp";
        //private static readonly ILog Logger = LogManager.GetLogger(typeof(RemoteRefDownloader));

        public async Task StartAsync(IList<RemoteRef> remoteRefs)
        {
            try
            {
                var missingRemoteRefs = GetMissingRemoteRefs(remoteRefs).ToArray();
                // 3. Create the related directory (parent).
                missingRemoteRefs
                    .Select(item => Path.GetDirectoryName(item.LocalPath))
                    .Distinct()
                    .ToList()
                    .ForEach(item => Directory.CreateDirectory(item));

                // 4. Download Files and raise their progress event.
                await DownloadFilesAsync(missingRemoteRefs);

                // 5. The file type must be zip, so it need to unzipped and released.
                missingRemoteRefs
                    .Where(item => File.Exists(GetZipFileName(item.LocalPath)))
                    .ToList()
                    .ForEach(item => Unzip(GetZipFileName(item.LocalPath)));
            }
            catch (Exception e)
            {
                //Logger.Error("An exception occurs when loading a remote module. ", e);
                throw;
            }
        }

        private static Task DownloadFilesAsync(IEnumerable<(Uri RemotePath, string LocalPath)> downloadInfos) => Task.Factory.StartNew(() =>
        {
            foreach (var (remotePath, localPath) in downloadInfos)
            {
                var request = (HttpWebRequest)WebRequest.Create(remotePath);

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK) throw new Exception("StatusCode is not equals to 200. ");

                    using (var outputStream = response.GetResponseStream())
                    {
                        var filePath = GetZipFileName(localPath);
                        DeleteFileSystemEntry(filePath);

                        using (var inputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            StreamCopy(outputStream, inputStream);
                        }
                    }
                }
            }
        });

        private static void StreamCopy(Stream outputStream, Stream inputStream)
        {
            // [NS-1383] Increase stream copy buffer size to boost network performance
            byte[] buffer = new byte[128 * 1024];
            int n;
            while ((n = outputStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                inputStream.Write(buffer, 0, n);
            }
        }

        private static void Unzip(string filePath)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            var tempDirectory = Path.Combine(Path.GetDirectoryName(filePath), $"{TempDirectoryName}_{Path.GetRandomFileName()}");
            using (var temp = new TempDirectory(tempDirectory))
            {
                ZipFile.ExtractToDirectory(filePath, temp.TempDirectoryPath);
                temp.CopyTo(Path.GetDirectoryName(GetSameNameFolderPath(filePath)));
            }
            DeleteFileSystemEntry(filePath);
        }

        private static string GetZipFileName(string directory) => $"{directory}.zip";

        private static IEnumerable<(Uri RemotePath, string LocalPath)> GetMissingRemoteRefs(IEnumerable<RemoteRef> remoteRefs)
        {
            return remoteRefs
                // Protocol: The local path of all remote refs are diretory, rather than file.
                // The source file type is zip, and the local path is its decompression path.
                .Where(item => !Directory.Exists(item.LocalPath))
                .Select(item => (new Uri(item.RemotePath), item.LocalPath));
        }

        private static void DeleteFileSystemEntry(string path)
        {
            if (Directory.Exists(path)) Directory.Delete(path, true);
            if (File.Exists(path)) File.Delete(path);
        }

        private static string GetSameNameFolderPath(string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException();

            var parentDirectory = Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(parentDirectory)) throw new DirectoryNotFoundException("The current file has no parent folder. ");

            // ReSharper disable once AssignNullToNotNullAttribute
            return Path.Combine(parentDirectory, Path.GetFileNameWithoutExtension(filePath));
        }
    }
}
