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
            for (int i = 1; predicate(@this); i++)
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

        public static bool TryDelete(this string path, int retryCount = 0)
        {
            if (!File.Exists(path)) return true;

            try
            {
                File.Delete(path);
            }
            catch (FileNotFoundException)
            {
                return true;
            }
            catch (DirectoryNotFoundException)
            {
                return true;
            }
            catch (IOException e)
            {
                if (retryCount > 5) return false;

                Logger.Error($"Try to delete the file ({path}) failed. Retry count = {retryCount}. ", e);
                TryDelete(path, retryCount + 1);
            }

            return true;
        }

        public static async Task<bool> TryDeleteAsync(this string path, int retryCount = 0)
        {
            if (!File.Exists(path)) return true;

            try
            {
                File.Delete(path);
            }
            catch (FileNotFoundException)
            {
                return true;
            }
            catch (DirectoryNotFoundException)
            {
                return true;
            }
            catch (IOException e)
            {
                if (retryCount > 5)
                {
                    Logger.Error($"Try to delete the file ({path}) failed. ", e);
                    return false;
                }

                Logger.Error($"Retry to delete the file ({path}) failed {retryCount} times. ", e);
                await TimeSpan.FromMilliseconds(retryCount * 500);

                await TryDeleteAsync(path, retryCount + 1);
            }
            catch (Exception e)
            {
                Logger.Error($"Try to delete the file ({path}) failed", e);
            }

            return true;
        }
    }
}
