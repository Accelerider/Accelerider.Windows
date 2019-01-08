using System;
using System.IO;
using System.Reflection;

namespace Accelerider.Windows.Infrastructure
{
    public static class AcceleriderFolders
    {
        static AcceleriderFolders()
        {
            Directory.CreateDirectory(Apps);
            Directory.CreateDirectory(Logs);
            Directory.CreateDirectory(Users);
        }

        /// <summary>
        /// It represents the path where the "Accelerider.Windows.exe" is located.
        /// </summary>
        public static readonly string MainProgram = AppDomain.CurrentDomain.BaseDirectory;

        public static string CurrentAssembly => Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);

        /// <summary>
        /// %AppData%\Accelerider
        /// </summary>
        public static readonly string AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Accelerider");

        /// <summary>
        /// %AppData%\Accelerider\Apps
        /// </summary>
        public static readonly string Apps = Path.Combine(AppData, nameof(Apps));

        /// <summary>
        /// %AppData%\Accelerider\Logs
        /// </summary>
        public static readonly string Logs = Path.Combine(AppData, nameof(Logs));

        /// <summary>
        /// %AppData%\Accelerider\Users
        /// </summary>
        public static readonly string Users = Path.Combine(AppData, nameof(Users));
    }

    public static class AcceleriderFiles
    {
        /// <summary>
        /// %AppData%\Accelerider\accelerider.config
        /// </summary>
        public static readonly string Configure = Path.Combine(AcceleriderFolders.AppData, "accelerider.config");
    }
}
