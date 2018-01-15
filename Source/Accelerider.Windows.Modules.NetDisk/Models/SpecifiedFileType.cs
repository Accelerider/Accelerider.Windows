using System;

namespace Accelerider.Windows.Modules.NetDisk.Models
{
    [Flags]
    public enum SpecifiedFileType
    {
        Root = 0,
        Music = 1,
        Videos = 2,
        Pictures = 4,
    }
}