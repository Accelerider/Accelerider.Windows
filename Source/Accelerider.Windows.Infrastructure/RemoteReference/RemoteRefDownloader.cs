using System;
using System.Collections.Generic;
using System.IO;
//using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Accelerider.Windows.RemoteReference
{
    internal class RemoteRefDownloader
    {
        //private const string TempDirectoryName = "temp";
        //private static readonly ILog Logger = LogManager.GetLogger(typeof(RemoteRefDownloader));

        public async Task StartAsync(IList<RemoteRef> remoteRefs, IProgress<(long bytesReceived, long totalBytesToReceive)> progress)
        {
            try
            {
                // 3. Create the related directory (parent).
                remoteRefs
                    .Select(item => Path.GetDirectoryName(item.LocalPath))
                    .Distinct()
                    .ToList()
                    .ForEach(item => Directory.CreateDirectory(item));

                // 4. Download Files and raise their progress event.
                await DownloadFilesAsync(remoteRefs, progress);

                // 5. The file type must be zip, so it need to unzipped and released.
                //uriLocalPathMap
                //    .Where(item => File.Exists(GetZipFileName(item.localPath)))
                //    .ToList()
                //    .ForEach(item => Unzip(GetZipFileName(item.localPath)));
            }
            catch (Exception e)
            {
                //Logger.Error("An exception occurs when loading a remote module. ", e);
                throw;
            }
        }

        private static async Task DownloadFilesAsync(
            IList<RemoteRef> remoteRefs,
            IProgress<(long bytesReceived, long totalBytesToReceive)> progress)
        {

            foreach (var remoteRef in remoteRefs)
            {
                // 1. Creates a Request, 
                var request = WebRequest.CreateHttp(remoteRef.RemotePath);
                // and configure the Headers, Cookies of it.

                // 2. Gets the Response from the Request,
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    // and check the status of the Response.
                    if (response.StatusCode != HttpStatusCode.OK) throw new Exception("StatusCode is not equals to 200. ");

                    // 3. Gets the Stream from the Response.
                    using (var inputStream = response.GetResponseStream())
                    // 4. Gets the FileStream from the LocalPath that is specified by the user.
                    using (var outputStream = new FileStream(remoteRef.LocalPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                    {
                        // 5. Copy data from RemotePath.Stream to LocalPath.Stream
                        CopyStream(inputStream, outputStream, progress);
                    }
                }
            }
        }

        private static void CopyStream(Stream inputStream, Stream outputStream, IProgress<(long bytesReceived, long totalBytesToReceive)> progress)
        {
            // Increase stream copy buffer size to boost network performance
            byte[] buffer = new byte[128 * 1024];
            int count;
            long sum = 0;
            while ((count = inputStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                outputStream.Write(buffer, 0, count);
                progress.Report((sum += count, inputStream.Length));
            }
        }

        //private static void Unzip(string filePath)
        //{
        //    // ReSharper disable once AssignNullToNotNullAttribute
        //    var tempDirectory = Path.Combine(Path.GetDirectoryName(filePath), $"{TempDirectoryName}_{Path.GetRandomFileName()}");
        //    using (var temp = new TempDirectory(tempDirectory))
        //    {
        //        ZipFile.ExtractToDirectory(filePath, temp.TempDirectoryPath);
        //        temp.CopyTo(Path.GetDirectoryName(GetSameNameFolderPath(filePath)));
        //    }
        //    DeleteFileSystemEntry(filePath);
        //}

        //private static string GetZipFileName(string directory) => $"{directory}.zip";

        //private static string GetSameNameFolderPath(string filePath)
        //{
        //    if (!File.Exists(filePath)) throw new FileNotFoundException();

        //    var parentDirectory = Path.GetDirectoryName(filePath);
        //    if (string.IsNullOrEmpty(parentDirectory)) throw new DirectoryNotFoundException("The current file has no parent folder. ");

        //    // ReSharper disable once AssignNullToNotNullAttribute
        //    return Path.Combine(parentDirectory, Path.GetFileNameWithoutExtension(filePath));
        //}
    }
}
