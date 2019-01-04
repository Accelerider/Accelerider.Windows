using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Package
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int count = 0;

            Console.WriteLine("[Action] Start Copying");

            var package = PackageInfo.Parse(File.ReadAllText("package.xml"));

            using (var tempFolder = new TempDirectory(package.Target))
            {
                package
                    .Flatten()
                    .Where(item => !(item is FolderElement folder) ||
                                   !folder.Files.Any() && !folder.Folders.Any())
                    .ForEach(item =>
                    {
                        item.Source.CopyTo(item.Target);

                        Console.WriteLine($"[Copy] {count++,-4}: " +
                                          $"{item.Source} " +
                                          $"--> " +
                                          $"{item.Target}");
                    });

                Console.WriteLine("[Action] Completed Copying");
                Console.WriteLine("[Action] Start Compressing");

                var zipTargetPath = $"{tempFolder.Path}.zip";
                if (File.Exists(zipTargetPath)) File.Delete(zipTargetPath);
                ZipFile.CreateFromDirectory(
                    tempFolder.Path,
                    zipTargetPath,
                    CompressionLevel.Optimal,
                    true);

                Console.WriteLine("[Action] Completed Compressing");
            }

            Console.ReadKey();
        }
    }

    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> @this, Action<T> callback)
        {
            foreach (var item in @this)
            {
                callback?.Invoke(item);
            }
        }

        // ReSharper disable AssignNullToNotNullAttribute
        public static void CopyTo(this string source, string target)
        {
            if (File.Exists(source))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(target));

                File.Copy(source, target, true);
            }
            else if (Directory.Exists(source))
            {
                Directory.CreateDirectory(target);

                Directory.GetFileSystemEntries(source)
                    .ForEach(item => item.CopyTo(Path.Combine(target, Path.GetFileName(item))));
            }
        }
        // ReSharper restore AssignNullToNotNullAttribute
    }
}
