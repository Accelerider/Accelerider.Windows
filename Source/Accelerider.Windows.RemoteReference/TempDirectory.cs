using System;
using System.IO;

namespace Accelerider.Windows.RemoteReference
{
    public sealed class TempDirectory : IDisposable
    {
        //private static readonly ILog Logger = LogManager.GetLogger(typeof(TempDirectory));

        public string TempDirectoryPath { get; }

        public TempDirectory(string tempDirectoryPath)
        {
            TempDirectoryPath = tempDirectoryPath;

            // Clean up the path, in order to make sure the folder is empty.
            DeleteFileSystemEntry(TempDirectoryPath);

            Directory.CreateDirectory(TempDirectoryPath);
        }

        public bool CopyTo(string targetPath, string subdirectoryName = null)
        {
            try
            {
                var subdirectory = Path.Combine(TempDirectoryPath, subdirectoryName ?? string.Empty);
                if (!Directory.Exists(subdirectory))
                    throw new DirectoryNotFoundException();

                Directory.Move(subdirectory, targetPath);

                return true;
            }
            catch (Exception e)
            {
                //Logger.Error(e);
                return false;
            }
        }

        private static void DeleteFileSystemEntry(string path)
        {
            if (Directory.Exists(path)) Directory.Delete(path, true);
            if (File.Exists(path)) File.Delete(path);
        }

        public void Dispose()
        {
            DeleteFileSystemEntry(TempDirectoryPath);
        }
    }
}
