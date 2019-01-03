using System;
using System.IO;
using System.Linq;
using static Accelerider.Windows.Infrastructure.SystemErrorCodes;

namespace Accelerider.Windows.Infrastructure
{
    // ReSharper disable InconsistentNaming
    public static class SystemErrorCodes
    {
        public const int ERROR_HANDLE_DISK_FULL = unchecked((int)0x80070027);
        public const int ERROR_DISK_FULL = unchecked((int)0x80070070);
    }
    // ReSharper restore InconsistentNaming

    public static class ExceptionExtensions
    {
        public static bool IsNotEnoughDiskSpace(this IOException e)
        {
            return e.IsException(ERROR_DISK_FULL, ERROR_HANDLE_DISK_FULL);
        }

        public static bool IsConnectionWasClosed(this IOException e)
        {
            throw new NotImplementedException();
        }

        public static bool IsException(this Exception e, params int[] hResults)
        {
            return hResults.Any(item => item == e.HResult);
        }
    }
}
