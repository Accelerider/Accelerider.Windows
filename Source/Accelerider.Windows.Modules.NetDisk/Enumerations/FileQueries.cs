using System;

namespace Accelerider.Windows.Modules.NetDisk.Enumerations
{
    [Flags]
    public enum FileQueries
    {
        Root = 0,

        Shared = 1,
        RecycleBin = 2,

        Documents = 4,
        Music = 8,
        Videos = 16,
        Pictures = 32,

        Favorites = 64
    }
}
