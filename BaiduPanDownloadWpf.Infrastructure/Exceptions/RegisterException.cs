using System;

namespace BaiduPanDownloadWpf.Infrastructure.Exceptions
{
    public class RegisterException: Exception
    {
        public RegisterStateEnum RegisterType { get; private set; }

        public RegisterException(string message, RegisterStateEnum registerType) : base(message)
        {
            RegisterType = registerType;
        }
    }
}
