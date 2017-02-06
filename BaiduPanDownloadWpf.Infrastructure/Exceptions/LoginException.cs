using System;

namespace BaiduPanDownloadWpf.Infrastructure.Exceptions
{
    public class LoginException: Exception
    {
        public ClientLoginStateEnum LoginType { get; private set; }

        public LoginException(string message, ClientLoginStateEnum loginType) : base(message)
        {
            LoginType = loginType;
        }
    }
}
