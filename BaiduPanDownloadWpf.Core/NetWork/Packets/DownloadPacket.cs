namespace BaiduPanDownloadWpf.Core.NetWork.Packets
{
    class DownloadPacket : PacketBase
    {
        public DownloadPacket()
        {
            PacketID = 7;
        }


        /// <summary>
        /// 文件信息
        /// </summary>
        public NetDiskFile[] Info { get; set; }

        /// <summary>
        /// 下载方法
        /// </summary>
        public int Method { get; set; } = 1;
    }
}
