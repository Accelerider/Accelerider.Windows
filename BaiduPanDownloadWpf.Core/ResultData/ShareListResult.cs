using Newtonsoft.Json;

namespace BaiduPanDownloadWpf.Core.ResultData
{
    /// <summary>
    /// 列出文件回复
    /// </summary>
    public class ShareListResult
    {
        #region 参数
        [JsonProperty("count")] private int _count;
        [JsonProperty("list")] private ShareItem[] _list;
        #endregion

        /// <summary>
        /// 分享数量
        /// </summary>
        public int Count => _count;
        /// <summary>
        /// 分享列表
        /// </summary>
        public ShareItem[] ItemList => _list;

        /// <summary>
        /// 分享信息
        /// </summary>
        public class ShareItem
        {
            #region 参数
            [JsonProperty("shareid")] private long _shareid;
            [JsonProperty("fsids")] private long[] _fsids;
            [JsonProperty("status")] private int _status;
            [JsonProperty("passwd")] private string _passwd;
            [JsonProperty("name")] private string _name;
            [JsonProperty("ctime")] private long _ctime;
            [JsonProperty("shortlink")] private string _shortlink;
            [JsonProperty("typicalPath")] public string _typicalPath;
            #endregion
            /// <summary>
            /// 分享ID
            /// </summary>
            public long ShareId => _shareid;
            /// <summary>
            /// 文件列表
            /// </summary>
            public long[] FileIdList => _fsids;
            /// <summary>
            /// 分享密码
            /// </summary>
            public string Password => _passwd;
            /// <summary>
            /// 名字
            /// </summary>
            public string Name => _name;
            /// <summary>
            /// 分享时间
            /// </summary>
            public long CreateTime => _ctime;
            /// <summary>
            /// 分享链接
            /// </summary>
            public string ShareLink => _shortlink;
            /// <summary>
            /// 文件目录
            /// </summary>
            public string Path => _typicalPath;
            /// <summary>
            /// 分享状态
            /// </summary>
            public ShareStatus States => (ShareStatus) _status;

        }

        /// <summary>
        /// 分享状态
        /// </summary>
        public enum ShareStatus
        {
            /// <summary>
            /// 正常
            /// </summary>
            Normal=0,
            /// <summary>
            /// 文件被删除
            /// </summary>
            Deleted=9,
            /// <summary>
            /// 分享失败
            /// </summary>
            Failure=1,
            /// <summary>
            /// 审核未通过
            /// </summary>
            Notpassed=4
        }
    }
}
