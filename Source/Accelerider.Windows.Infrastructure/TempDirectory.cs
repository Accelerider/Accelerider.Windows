using System;
using System.IO;

namespace Accelerider.Windows.Infrastructure
{
    public sealed class TempDirectory : IDisposable
    {
        public string DirectoryPath { get; }

        public TempDirectory(string tempDirectoryPath)
        {
            DirectoryPath = tempDirectoryPath;

            // Clean up the path, in order to make sure the folder is empty.
            DeleteFileSystemEntry(DirectoryPath);

            Directory.CreateDirectory(DirectoryPath);
        }

        public bool MoveTo(string targetPath, string subDirectoryName = null)
        {
            try
            {
                var directory = Path.Combine(DirectoryPath, subDirectoryName ?? string.Empty);
                if (!Directory.Exists(directory))
                    throw new DirectoryNotFoundException();

                Directory.Move(directory, targetPath);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void DeleteFileSystemEntry(string path)
        {
            if (Directory.Exists(path)) Directory.Delete(path, true);
            if (File.Exists(path)) File.Delete(path);
        }

        public void Dispose()
        {
            DeleteFileSystemEntry(DirectoryPath);
        }
    }
}
