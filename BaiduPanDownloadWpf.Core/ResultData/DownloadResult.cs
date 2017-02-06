using System.Collections.Generic;
using System.Linq;
using BaiduPanDownloadWpf.Core.NetWork;
using Newtonsoft.Json;

namespace BaiduPanDownloadWpf.Core.ResultData
{
    /// <summary>
    /// 下载请求回复
    /// </summary>
    public class DownloadResult
    {
        [JsonProperty("error")]
        private int _error;

        [JsonProperty("urls")]
        private Dictionary<string, UrlList> _urls;

        /// <summary>
        /// Cookies
        /// </summary>
        public Cookies Cookies { get; set; }

        /// <summary>
        /// 错误代码
        /// </summary>
        public int ErrorCode => _error;

        /// <summary>
        /// 下载链接列表
        /// </summary>
        public Dictionary<string, UrlList> DownloadUrlList => _urls;

        /// <summary>
        /// 根据文件名获取链接
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string[] GetUrlsByFileName(string fileName)
        {
            return !DownloadUrlList.ContainsKey(fileName) ? null : DownloadUrlList[fileName].UrlLists;
        }

        /// <summary>
        /// 链接列表
        /// </summary> 
        public class UrlList
        {
            [JsonProperty("urls")]
            private SubClass[] _urls;

            /// <summary>
            /// 链接List
            /// </summary>
            public string[] UrlLists => _urls.Select(v => v.Url).ToArray();

            /// <summary>
            /// 子类
            /// </summary>
            public class SubClass
            {
                [JsonProperty("url")]
                private string _url;

                /// <summary>
                /// 链接
                /// </summary>
                public string Url => _url;
            }
        }
    }
}
