using System;
using BaiduPanDownloadWpf.Infrastructure;

namespace BaiduPanDownloadWpf.Core.NetWork.Exceptions
{
    class RegisterException: Exception
    {
        public RegisterStateEnum RegisterType { get; private set; }

        public RegisterException(string message, RegisterStateEnum registerType) : base(message)
        {
            RegisterType = registerType;
        }
    }
}
