using System;
using System.IO;

namespace Package
{
    public sealed class TempDirectory : IDisposable
    {
        public string Path { get; }

        public TempDirectory(string tempDirectoryPath, bool createNew = true)
        {
            Path = tempDirectoryPath;

            // Clean up the path, in order to make sure the folder is empty.
            DeleteFileSystemEntry(Path);

            if (createNew)
                Directory.CreateDirectory(Path);
        }

        public bool MoveTo(string targetPath) => MoveTo(string.Empty, targetPath);

        public bool MoveTo(string relativePath, string targetPath)
        {
            try
            {
                var subdirectory = System.IO.Path.Combine(Path, relativePath);
                if (!Directory.Exists(subdirectory))
                    throw new DirectoryNotFoundException();

                if (Directory.Exists(targetPath))
                    DeleteFileSystemEntry(targetPath);

                var parentDirectory = System.IO.Path.GetDirectoryName(targetPath);
                if (parentDirectory != null && !Directory.Exists(parentDirectory))
                    Directory.CreateDirectory(parentDirectory);

                Directory.Move(subdirectory, targetPath);

                return true;
            }
            catch
            {
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
            DeleteFileSystemEntry(Path);
        }

        public override string ToString() => Path;
    }
}
