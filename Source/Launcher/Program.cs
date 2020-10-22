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

        private class LauncherArgs
        {
            public double Delay { get; set; }

            public bool AutoLogin { get; set; }

            //public string Username { get; set; }

            //public string Password { get; set; }

            //public string AddModuleName { get; set; }

            //public string RemoveModuleName { get; set; }

            //public Version ClearVersion { get; set; }
        }

        public static async Task Main(string[] args)
        {
            var launcherArgs = ParseLauncherArgs(args);

            if (launcherArgs.Delay > 0)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(launcherArgs.Delay));
            }

            // 1. Find the latest version bin folder.
            var bins = GetBinDirectories(CurrentDirectory).ToList();
            if (!bins.Any()) return;

            bins.Sort((x, y) => x.Version > y.Version ? -1 : 1);

            foreach (var bin in bins)
            {
                // 2. Try to start the main program.
                try
                {
                    var process = new Process
                    {
                        StartInfo =
                        {
                            FileName = Path.Combine(bin.DirectoryName, MainProgramName),
                            WindowStyle = ProcessWindowStyle.Hidden,
                            Arguments = launcherArgs.AutoLogin ? "--auto-login" : string.Empty
                        }
                    };
                    process.Start();

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

        private static LauncherArgs ParseLauncherArgs(string[] args)
        {
            var argParser = new ArgParser<LauncherArgs>();

            argParser
                //.Define("username", (o, value) => o.Username = value)
                //.Define("password", (o, value) => o.Password = value)
                //.Define("add", (o, value) => o.AddModuleName = value)
                //.Define("remove", (o, value) => o.RemoveModuleName = value)
                //.Define("clear", (o, value) => o.ClearVersion = Version.Parse(value))
                .Define<double>("delay", (o, value) => o.Delay = value)
                .Define<bool>("auto-login", (o, value) => o.AutoLogin = value);

            return argParser.Parse(args);
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
