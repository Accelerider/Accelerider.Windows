namespace BaiduPanDownloadWpf.Core.ResultData
{
    /// <summary>
    /// 验证码检查返回值
    /// </summary>
    public class CheckResult
    {
        /// <summary>
        /// 是否需要验证码
        /// </summary>
        public bool NeedCode { get; set; }

        /// <summary>
        /// BaiduID
        /// </summary>
        public string BaiduId { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 验证码图片
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// 验证码字符串
        /// </summary>
        public string CodeString { get; set; }
    }
}
