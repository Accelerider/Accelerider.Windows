using System;
using System.IO;

namespace Accelerider.Windows.Infrastructure
{
    public static class AcceleriderPaths
    {
        static AcceleriderPaths()
        {
            Directory.CreateDirectory(AppsFolder);
            Directory.CreateDirectory(LogsFolder);
            Directory.CreateDirectory(UsersFolder);
        }

        public static readonly string MainProgramFolder = AppDomain.CurrentDomain.BaseDirectory;

        public static readonly string AppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Accelerider");

        public static readonly string AppsFolder = Path.Combine(AppDataFolder, "Apps");

        public static readonly string LogsFolder = Path.Combine(AppDataFolder, "Logs");

        public static readonly string UsersFolder = Path.Combine(AppDataFolder, "Users");

        public static readonly string ConfigureFile = Path.Combine(AppDataFolder, "accelerider.config");
    }
}
