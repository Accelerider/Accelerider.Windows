namespace BaiduPanDownloadWpf.Core.NetWork.Packets
{
    class ShareFilePacket : PacketBase
    {
        public ShareFilePacket()
        {
            PacketID = 8;
        }
        /// <summary>
        /// 文件信息
        /// </summary>
        public long[] FileIds { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = null;
    }
}
