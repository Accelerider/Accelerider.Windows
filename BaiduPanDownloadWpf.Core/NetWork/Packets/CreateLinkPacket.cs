using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.Core.NetWork.Packets
{
    class CreateLinkPacket : PacketBase
    {
        public CreateLinkPacket()
        {
            PacketID = 13;
        }

        public NetDiskFile Info { get; set; }
    }
}
