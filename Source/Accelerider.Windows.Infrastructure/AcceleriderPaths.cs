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
            Directory.CreateDirectory(Settings);
        }

        /// <summary>
        /// It represents the path where the "Accelerider.Windows.exe" is located.
        /// </summary>
        public static readonly string MainProgram = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// It represents the path where the current assembly (*.dll file) is located.
        /// </summary>
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
        /// %AppData%\Accelerider\Settings
        /// </summary>
        public static readonly string Settings = Path.Combine(AppData, nameof(Settings));
    }

    public static class AcceleriderFiles
    {
        /// <summary>
        /// %AppData%\Accelerider\settings.json
        /// </summary>
        public static readonly string Configure = Path.Combine(AcceleriderFolders.AppData, "settings.json");
    }
}
