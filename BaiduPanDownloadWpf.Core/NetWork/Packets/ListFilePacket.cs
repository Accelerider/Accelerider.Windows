namespace BaiduPanDownloadWpf.Core.NetWork.Packets
{
    class ListFilePacket : PacketBase
    {
        public ListFilePacket()
        {
            PacketID = 5;
        }

        /// <summary>
        /// 需要列出的目录
        /// </summary>
        public string Path { get; set; }
        
        
    }
}
