using System;
using Accelerider.Windows.Infrastructure;

namespace Accelerider.Windows.ServerInteraction
{
    public class ResponseBase
    {
        public string Status { get; set; }

        public bool Success => string.Equals(Status, StatusCodes.Success, StringComparison.InvariantCultureIgnoreCase);
    }

    public class SendVerificationCodeResponse : ResponseBase
    {
        public string SessionId { get; set; }
    }

    public class SignUpResponse : ResponseBase
    {
        public string UserId { get; set; }
    }

    public class LoginResponse : ResponseBase
    {
        public string AccessToken { get; set; }
    }
}
