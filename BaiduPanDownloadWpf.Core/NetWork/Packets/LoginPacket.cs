namespace BaiduPanDownloadWpf.Core.NetWork.Packets
{
    class LoginPacket : PacketBase
    {
        public LoginPacket()
        {
            PacketID = 2;
        }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Password { get; set; }
    }
}
