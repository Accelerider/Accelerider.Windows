using System;
using BaiduPanDownloadWpf.Infrastructure;

namespace BaiduPanDownloadWpf.Core.NetWork.Exceptions
{
    class LoginException: Exception
    {
        public ClientLoginStateEnum LoginType { get; private set; }

        public LoginException(string message, ClientLoginStateEnum loginType) : base(message)
        {
            LoginType = loginType;
        }



    }
}
