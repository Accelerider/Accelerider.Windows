using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.Core.NetWork.Packets
{
    class DeleteFilePacket : PacketBase
    {
        public DeleteFilePacket()
        {
            PacketID = 12;
        }
        public string Path { get; set; }
    }
}
