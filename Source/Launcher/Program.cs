using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Launcher
{
    public class Program
    {
        private const string MainProgramName = "Accelerider.Windows.exe";
        private static readonly string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly Regex BinDirectoryRegex = new Regex(@"^bin-(\d+?\.\d+?\.\d+?)$", RegexOptions.Compiled);

        public static async Task Main(string[] args)
        {
            // 1. Find the latest version bin folder.
            var bins = GetBinDirectories(CurrentDirectory).ToList();
            if (!bins.Any()) return;

            bins.Sort((x, y) => x.Version > y.Version ? -1 : 1);

            foreach (var bin in bins)
            {
                // 2. Try to start the main program.
                try
                {
                    Process.Start(Path.Combine(bin.DirectoryName, MainProgramName));

                    // 3. Clear history versions.
                    foreach (var directory in bins.Where(item => !bin.Equals(item)))
                    {
                        await DeleteDirectoryAsync(directory.DirectoryName);
                    }
                }
                catch (Win32Exception)
                {
                    // TODO: Logging or Notify.
                    await DeleteDirectoryAsync(bin.DirectoryName);
                }
            }
        }

        private static IEnumerable<BinDirectory> GetBinDirectories(string path)
        {
            return from directory in Directory.GetDirectories(path)
                   let match = BinDirectoryRegex.Match(Path.GetFileName(directory) ?? string.Empty)
                   where match.Success
                   select new BinDirectory(Version.Parse(match.Groups[1].Value), directory);
        }

        private static async Task DeleteDirectoryAsync(string path, int count = 0)
        {
            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                if (count < 10)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(500 * count));
                    await DeleteDirectoryAsync(path, count + 1);
                }
            }
        }
    }

    public struct BinDirectory
    {
        public BinDirectory(Version version, string directoryName)
        {
            Version = version;
            DirectoryName = directoryName;
        }

        public Version Version { get; }

        public string DirectoryName { get; }
    }
}
