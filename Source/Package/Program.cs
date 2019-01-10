using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Package
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const string packageDirectory = "package-define-files";

            var packageNames = new[]
            {
                "shell.package.xml",
                "shell.bin.package.xml",
                "net-disk.package.xml",
            };

            try
            {
                packageNames
                    .Select(item => File.ReadAllText(Path.Combine(packageDirectory, item)))
                    .Select(FolderElement.Create)
                    .ForEach(item => Pack(item));
            }
            catch (FileNotFoundException e)
            {
                Out.Print($"Missing {e.FileName}!", OutType.Error);
            }

            Out.EndLine();
            Console.ReadKey();
        }

        private static void Pack(FolderElement info, bool openFolder = true)
        {
            string zipTargetPath;
            using (var tempFolder = new TempDirectory(info.Target))
            {
                Out.Print($"Start Copying: {info.Source} --> {info.Target}");
                info
                    .Flatten()
                    .Where(item => !(item is FolderElement folder) ||
                                   !folder.Files.Any() && !folder.Folders.Any())
                    .ForEach(item => item.Source.CopyTo(item.Target));
                Out.Print("Completed Copying. ");

                zipTargetPath = $"{tempFolder.Path}.zip";
                Out.Print($"Start Package: {zipTargetPath}...");
                if (File.Exists(zipTargetPath)) File.Delete(zipTargetPath);
                ZipFile.CreateFromDirectory(
                    tempFolder.Path,
                    zipTargetPath,
                    CompressionLevel.Optimal,
                    true);

                Out.Print("Completed Package. ");
            }

            if (openFolder)
            {
                Out.Print($"Opening the file: {zipTargetPath}...");
                Process.Start("explorer.exe", $"/select, {zipTargetPath}");
            }

            Out.Divider();
        }
    }
}
