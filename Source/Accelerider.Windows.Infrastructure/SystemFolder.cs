using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Accelerider.Windows.Infrastructure
{
    public static class SystemFolder
    {
        public static readonly FileLocation Desktop = GetSystemFolder("{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}");
        public static readonly FileLocation Downloads = GetSystemFolder("{374DE290-123F-4565-9164-39C4925E467B}");
        public static readonly FileLocation Documents = GetSystemFolder("{FDD39AD0-238F-46AF-ADB4-6C85480369C7}");
        public static readonly FileLocation Music = GetSystemFolder("{4BD8D571-6D19-48D3-BE97-422220080E43}");
        public static readonly FileLocation Pictures = GetSystemFolder("{33E28130-4E1E-4676-835A-98395C3BC3BB}");
        public static readonly FileLocation Videos = GetSystemFolder("{18989B1D-99B5-455B-841C-AB7C74E4DDFC}");
        public static readonly FileLocation Favorites = GetSystemFolder("{1777F761-68AD-4D8A-87BD-30B759FA33DD}");


        public static FileLocation[] GetAvailableFolders()
        {
            return (from path in new[] { Downloads, Desktop, Documents, Music, Pictures, Videos, Favorites }
                    where path != null
                    select path).ToArray();
        }


        [DllImport("Shell32.dll")]
        private static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)]Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr ppszPath);

        private static FileLocation GetSystemFolder(string guid)
        {
            int result = SHGetKnownFolderPath(new Guid(guid), 0x00004000, new IntPtr(0), out IntPtr outPath);
            return result >= 0 ? Marshal.PtrToStringUni(outPath) : null;
        }
    }
}
