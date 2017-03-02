using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using BaiduPanDownloadWpf.Core.NetWork.Packets;
using Newtonsoft.Json;

namespace BaiduPanDownloadWpf.Core.NetWork
{
    /// <summary>
    /// 数据服务器
    /// </summary>
    public class Server
    {
        /// <summary>
        /// 默认服务器
        /// </summary>
        public static Server DefaultServer { get; } = new Server()
        {
            ServerAddress = "tool.mrs4s.top",
            Port = 10162
        };

        public static Server TestServer { get; }=new Server()
        {
            ServerAddress = "tool.mrs4s.top",
            Port=1024
        };

        /// <summary>
        /// 服务器IP地址
        /// </summary>
        public string ServerAddress { get; set; }

        /// <summary>
        /// 服务器端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <returns></returns>
        public Socket Connect()
        {
            try
            {
                var hostinfo = Dns.GetHostByName(ServerAddress);
                var ip = hostinfo.AddressList[0];
                var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(new IPEndPoint(ip, Port));
                return clientSocket;
            }
            catch (Exception ex)
            {
                throw new ServerException("连接服务器时出现错误", ex);
            }
        }

        /// <summary>
        /// 发送封包,获取回复(同步)
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public string SendPacket(PacketBase packet)
        {
            var data =
                Encoding.UTF8.GetBytes("[$Start]" +
                                       Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(packet))) +
                                       "[$End]");
            using (var client = Connect())
            {
                client.Send(data, data.Length, 0);
                var tmp = string.Empty;
                while (true)
                {
                    var result = new byte[1024];
                    var length = client.Receive(result);
                    tmp += Encoding.UTF8.GetString(result, 0, length);
                    if (tmp.Contains("[$Start]") && tmp.Contains("[$End]"))
                    {
                        return
                            Encoding.UTF8.GetString(
                                Convert.FromBase64String(tmp.Replace("[$Start]", string.Empty)
                                    .Replace("[$End]", string.Empty)));
                    }
                }
            }
        }

        /// <summary>
        /// 发送封包,获取回复(异步)
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public async Task<string> SendPacketAsync(PacketBase packet)
        {
            return await Task.Run(() => SendPacket(packet));
        }
    }
}
