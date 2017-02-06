namespace BaiduPanDownloadWpf.Core.NetWork.Packets
{
    class RegisterPacket : PacketBase
    {
        public RegisterPacket()
        {
            PacketID = 1;
        }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}
