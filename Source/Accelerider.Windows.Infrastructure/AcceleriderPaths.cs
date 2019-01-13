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
            Directory.CreateDirectory(Database);
            Directory.CreateDirectory(Temp);
        }

        /// <summary>
        /// It represents the path where the "Accelerider.Windows.exe" is located.
        /// </summary>
        public static readonly string Base = AppDomain.CurrentDomain.BaseDirectory;

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
        /// %AppData%\Accelerider\Database
        /// </summary>
        public static readonly string Database = Path.Combine(AppData, nameof(Database));

        /// <summary>
        /// %AppData%\Accelerider\Temp
        /// </summary>
        public static readonly string Temp = Path.Combine(AppData, nameof(Temp));
    }
}
