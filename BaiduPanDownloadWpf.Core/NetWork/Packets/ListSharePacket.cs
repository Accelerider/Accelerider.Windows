namespace BaiduPanDownloadWpf.Core.NetWork.Packets
{
    class ListSharePacket : PacketBase
    {
        public ListSharePacket()
        {
            PacketID = 11;
        }

        public int Page { get; set; }
    }
}
