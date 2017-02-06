namespace BaiduPanDownloadWpf.Core.NetWork.Packets
{
    class BindingCookiesPacket : PacketBase
    {
        public BindingCookiesPacket()
        {
            PacketID = 4;
        }
        /// <summary>
        /// 账号Cookies
        /// </summary>
        public Cookies cookies { get; set; }
    }
}
