using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Accelerider.Windows.Update
{
    public static class Program
    {
        private const string UPDATE_FOLDER_NAME = "Update";
        private const string UPDATE_ROUTINE_NAME = "Accelerider.Windows.Update.exe";
        private const string ACCELERIDER_ROUTINE_NAME = "Accelerider.Windows.exe";

        public static void Main(string[] args)
        {
            var softwareFolder = Environment.CurrentDirectory;
            var updateFolder = Path.Combine(softwareFolder, UPDATE_FOLDER_NAME);

            var lastFileNames = from filePath in Directory.GetFiles(updateFolder)
                                let fileInfo = new FileInfo(filePath)
                                where fileInfo.Extension == ".exe" || fileInfo.Extension == ".dll"
                                where fileInfo.Name != UPDATE_ROUTINE_NAME
                                select fileInfo.Name;

            foreach (var lastFileName in lastFileNames)
            {
                var oldFileName = Path.Combine(softwareFolder, lastFileName);
                if (File.Exists(oldFileName))
                {
                    DeleteFile(oldFileName);
                }
                File.Move(Path.Combine(updateFolder, lastFileName), oldFileName);
            }

            Process.Start(Path.Combine(softwareFolder, ACCELERIDER_ROUTINE_NAME));
        }

        private static void DeleteFile(string filePath)
        {
            try
            {
                File.Delete(filePath);
            }
            catch (IOException)
            {
                DeleteFile(filePath);
            }
        }
    }
}
