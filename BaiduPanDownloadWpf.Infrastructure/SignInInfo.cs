namespace BaiduPanDownloadWpf.Infrastructure
{
    public enum SignInState
    {

    }
    public class SignInInfo
    {
        public string UserName { get; }
        public string Password { get; }
        public string VerificationCode { get; }
        public SignInState State { get; set; }

        public SignInInfo(string userName, string password, string verficationCode)
        {
            UserName = userName;
            Password = password;
            VerificationCode = verficationCode;
        }
    }
}
