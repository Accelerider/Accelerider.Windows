namespace BaiduPanDownloadWpf.Core.NetWork.Packets
{
    public class PacketBase
    {
        /// <summary>
        /// 封包ID
        /// </summary>
        public int PacketID { get; set; }
        /// <summary>
        /// 用户Token
        /// </summary>
        public string Token { get; set; }
    }
}
