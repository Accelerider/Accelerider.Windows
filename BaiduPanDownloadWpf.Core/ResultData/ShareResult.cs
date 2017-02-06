using Newtonsoft.Json;

namespace BaiduPanDownloadWpf.Core.ResultData
{
    /// <summary>
    /// 分享返回
    /// </summary>
    public class ShareResult
    {
        [JsonProperty("errno")] private int _errno;
        [JsonProperty("shareid")] private long _shareid;
        [JsonProperty("link")] private string _link;
        [JsonProperty("shorturl")] private string _shorturl;
        [JsonProperty("json")] private string _json;

        /// <summary>
        /// 错误代码（意义还未完全搞明白）
        /// </summary>
        public int ErrorCode => _errno;

        /// <summary>
        /// 分享ID
        /// </summary>
        public long ShareId => _shareid;

        /// <summary>
        /// 分享链接
        /// </summary>
        public string Link => _link;

        /// <summary>
        /// 分享链接(短)
        /// </summary>
        public string ShortUrl => _shorturl;

        /// <summary>
        /// 服务器返回完整Json
        /// </summary>
        public string Json => _json;

    }
}
