using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using log4net;

// ReSharper disable once CheckNamespace
namespace System.IO
{
    public static class ExtensionMethods
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ExtensionMethods));

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);

        public static bool CreateHardLinkTo(this string @this, string targetPath)
        {
            if (File.Exists(targetPath) || Directory.Exists(targetPath)) throw new IOException("Cannot create an existing file or directory. ");

            if (File.Exists(@this))
            {
                return CreateHardLink(targetPath, @this, IntPtr.Zero);
            }

            if (Directory.Exists(@this))
            {
                var entries = Directory.GetFileSystemEntries(@this);
                Directory.CreateDirectory(targetPath);

                // Recursion
                return entries.All(item => item.CreateHardLinkTo(Path.Combine(targetPath, Path.GetFileName(item))));
            }

            throw new FileNotFoundException();
        }


        private static readonly Regex FileNameCountRegex = new Regex(@"[\(（]\d+?[\)）]", RegexOptions.Compiled);

        public static string GetUniqueLocalPath(this string @this, Func<string, bool> predicate = null)
        {
            if (predicate == null) predicate = File.Exists;

            var directoryName = Path.GetDirectoryName(@this) ?? throw new InvalidOperationException();
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(@this) ?? throw new InvalidOperationException();
            fileNameWithoutExtension = FileNameCountRegex.Replace(fileNameWithoutExtension, string.Empty).Trim();
            var extension = Path.GetExtension(@this);
            for (int i = 2; predicate(@this); i++)
            {
                @this = $"{Path.Combine(directoryName, fileNameWithoutExtension)} ({i}){extension}";
            }

            return @this;
        }

        public static long GetLocalDiskAvailableFreeSpace(this string path)
        {
            var targetDriveName = Directory.GetDirectoryRoot(path);

            return DriveInfo
                .GetDrives()
                .FirstOrDefault(item => item.Name.Equals(targetDriveName, StringComparison.InvariantCultureIgnoreCase))?
                .AvailableFreeSpace ?? 0L;
        }

        public static async Task<bool> TryDeleteAsync(this string path) => await TryDeleteAsync(path, 1);

        private static async Task<bool> TryDeleteAsync(string path, int retryCount)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                else if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
                else
                {
                    return true;
                }
            }
            catch (FileNotFoundException) { }
            catch (DirectoryNotFoundException) { }
            catch (IOException e)
            {
                if (retryCount > 5)
                {
                    Logger.Error($"Try to delete the path ({path}) failed. ", e);
                    return false;
                }

                Logger.Error($"Retry to delete the path ({path}) failed {retryCount} times. ", e);
                await TimeSpan.FromMilliseconds(retryCount * 500);

                await TryDeleteAsync(path, retryCount + 1);
            }
            catch (Exception e)
            {
                Logger.Error($"Try to delete the path ({path}) failed", e);
                return false;
            }

            return true;
        }

        public static string EnsureFileFolder(this string path)
        {
            var directory = Path.GetDirectoryName(path) ?? throw new InvalidOperationException();
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return path;
        }
    }
}
