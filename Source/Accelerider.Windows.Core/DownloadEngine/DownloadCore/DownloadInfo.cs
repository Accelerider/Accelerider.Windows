using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Core.NetWork;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accelerider.Windows.Core.DownloadEngine.DownloadCore
{
    /// <summary>
    /// 下载数据
    /// </summary>
    internal class DownloadInfo
    {
        /// <summary>
        /// 总长度
        /// </summary>
        public long ContentLength { get; set; }
        /// <summary>
        /// 下载完成长度
        /// </summary>
        public long CompletedLength { get; set; }
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool Completed { get; set; }

        /// <summary>
        /// 是否加密
        /// </summary>
        public bool Encryption { get; set; } = false;

        /// <summary>
        /// 线程数
        /// </summary>
        public int ThreadNum { get; set; } = 2;

        /// <summary>
        /// 下载链接
        /// </summary>
        public string[] DownloadUrl { get; set; }

        /// <summary>
        /// 链接使用计数
        /// </summary>
        public Dictionary<string, int> DownloadCount { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// 下载路径
        /// </summary>
        public string DownloadPath { get; set; }
        /// <summary>
        /// 块大小
        /// </summary>
        public long BlockLength { get; set; }
        /// <summary>
        /// Cookies
        /// </summary>
        public Cookies UserCookies { get; set; }
        /// <summary>
        /// Cookies所使用的Domain
        /// </summary>
        public string Domain { get; set; } = ".baidu.com";

        /// <summary>
        /// 下载所使用的的Referer
        /// </summary>
        public string Referer { get; set; } = "http://pan.baidu.com/disk/home";

        /// <summary>
        /// 数据
        /// </summary>
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// 下载所使用的的UA
        /// </summary>
        public string UserAgent { get; set; } = "netdisk;5.5.2.0;PC;PC-Windows;6.1.7601;WindowsBaiduNetdisk";

        /// <summary>
        /// 下载分块
        /// </summary>
        public List<DownloadBlock> DownloadBlockList { get; } = new List<DownloadBlock>();

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime CompletedTime { get; set; }

        /// <summary>
        /// 初始化分块信息
        /// </summary>
        public void Init(string path, Dictionary<string, string> data = null)
        {
            Init(data);
            Save(path);
        }

        /// <summary>
        /// 初始化分块信息
        /// </summary>
        public void Init(Dictionary<string, string> data = null)
        {
            var temp = 0L;
            DownloadBlockList.Clear();
            while (temp + BlockLength < ContentLength)
            {
                DownloadBlockList.Add(new DownloadBlock
                {
                    From = temp,
                    To = temp + BlockLength - 1,
                    Completed = false,
                });
                temp += BlockLength;
            }
            DownloadBlockList.Add(new DownloadBlock
            {
                From = temp,
                To = ContentLength,
                Completed = false,
            });
            CompletedTime = DateTime.Now;
            if (data != null)
                Data = data;
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            File.WriteAllText(path, JObject.Parse(JsonConvert.SerializeObject(this)).ToString());
        }
    }
    /// <summary>
    /// 下载分块
    /// </summary>
    internal class DownloadBlock
    {
        /// <summary>
        /// 下载开始处
        /// </summary>
        public long From { get; set; }
        /// <summary>
        /// 下载结束处
        /// </summary>
        public long To { get; set; }
        /// <summary>
        /// 已下载大小
        /// </summary>
        public long CompletedLength { get; set; } = 0L;

        /// <summary>
        /// 下载中
        /// </summary>
        [JsonIgnore]
        public bool Downloading { get; set; }

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool Completed { get; set; }
    }
}
