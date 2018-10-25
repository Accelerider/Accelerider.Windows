﻿using System;
using System.Collections.Generic;
using System.IO;
//using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.TransferService;

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
                await DownloadFilesAsync(remoteRefs);

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

        private static Task DownloadFilesAsync(
            IEnumerable<RemoteRef> remoteRefs)
        {
            var tasks = remoteRefs.Select(remoteRef => FileTransferService.GetFileDownloaderBuilder()
                    .UseDefaultConfigure()
                    .Build()
                    .From(remoteRef.RemotePath)
                    .To(remoteRef.LocalPath)
                    .ActivateAsync());

            return Task.WhenAll(tasks);
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
