using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.Core.NetWork
{
    internal class HttpClient
    {
        public Cookies UserCookies { get; set; } = new Cookies();

        public string Domain { get; set; } = ".baidu.com";

        /// <summary>
        /// GET请求URL
        /// </summary>
        /// <param name="url"></param>
        /// <param name="ua"></param>
        /// <param name="referer"></param>
        /// <returns></returns>
        public string Get(string url, string ua = null, string referer = null)
        {
            var result = Encoding.UTF8.GetString(GetBytes(url, ua, referer));
            return result;
        }

        /// <summary>
        /// GET请求URL（异步）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="ua"></param>
        /// <param name="referer"></param>
        /// <returns></returns>
        public async Task<string> GetAsync(string url, string ua = null, string referer = null)
        {
            return await Task.Run(() => Get(url, ua, referer));
        }

        /// <summary>
        /// POST请求URL
        /// </summary>
        /// <param name="url"></param>
        /// <param name="dic"></param>
        /// <param name="ua"></param>
        /// <param name="referer"></param>
        /// <returns></returns>
        public string Post(string url, Dictionary<string, string> dic, string ua = null, string referer = null)
        {
            string result;
            var requst = WebRequest.CreateHttp(url);
            requst.CookieContainer = new CookieContainer();
            if (UserCookies != null)
            {
                foreach (var key in UserCookies.GetKeys())
                {
                    var cookie = new Cookie(key, UserCookies.GetCookie(key)) { Domain = Domain };
                    requst.CookieContainer.Add(cookie);
                }
            }
            requst.UserAgent = ua ?? "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
            requst.Method = "POST";
            requst.ContentType = "application/x-www-form-urlencoded";
            requst.Timeout = 60000;
            requst.Referer = referer ?? string.Empty;
            try
            {
                var postBytes = Encoding.UTF8.GetBytes(string.Join("&", dic.Select(v => $"{v.Key}={v.Value}")));
                using (var stream = requst.GetRequestStream())
                    stream.Write(postBytes, 0, postBytes.Length);
                using (var response = requst.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(string.IsNullOrEmpty(response.ContentEncoding) ? "UTF-8" : response.ContentEncoding)))
                    {
                        result = reader.ReadToEnd();
                    }
                    UserCookies = Cookies.GetCookiesByCookieContainer(requst.CookieContainer);
                }
            }
            catch (WebException wex)
            {

                var stream = wex.Response?.GetResponseStream();
                if (stream == null)
                    return "{\"errno\":-10,\"message\":\"链接服务器超时\"}";
                using (var reader = new StreamReader(stream, encoding: Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
                UserCookies = Cookies.GetCookiesByCookieContainer(requst.CookieContainer);

            }
            catch (Exception ex)
            {
                return "{\"errno\":-11,\"message\":\"链接服务器出现未知错误\"}";
            }
#if DEBUG
            /*
            LogHelper.Info($"Debug - POST {url}  result " + (result.Length > 2048 ? "result to long" : result));
            LogHelper.Info("Debug - Post body: " + string.Join("&", dic.Select(v => $"{Uri.UnescapeDataString(v.Key)}={Uri.UnescapeDataString(v.Value)}")));
            */
#endif
            return result;
        }

        /// <summary>
        /// POST请求URL（异步）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="dic"></param>
        /// <param name="ua"></param>
        /// <param name="referer"></param>
        /// <returns></returns>
        public async Task<string> PostAsync(string url, Dictionary<string, string> dic, string ua = null, string referer = null)
        {
            return await Task.Run(() => Post(url, dic, ua, referer));
        }


        /// <summary>
        /// Get获取返回的Bytes
        /// </summary>
        /// <param name="url"></param>
        /// <param name="ua"></param>
        /// <param name="referer"></param>
        /// <returns></returns>
        public byte[] GetBytes(string url, string ua = null, string referer = null)
        {
            var bytes = new List<byte>();
            var requst = WebRequest.CreateHttp(url);
            requst.CookieContainer = new CookieContainer();
            if (UserCookies != null)
            {
                foreach (var key in UserCookies.GetKeys())
                {
                    var cookie = new Cookie(key, UserCookies.GetCookie(key)) { Domain = Domain };
                    requst.CookieContainer.Add(cookie);
                }
            }
            requst.UserAgent = ua ?? "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
            requst.Method = "GET";
            requst.Timeout = 60000;
            requst.Referer = referer ?? string.Empty;
            try
            {
                using (var response = requst.GetResponse() as HttpWebResponse)
                {
                    var stream = response.GetResponseStream();
                    while (true)
                    {
                        var array = new byte[1024];
                        var length = stream.Read(array, 0, array.Length);
                        if (length <= 0) break;
                        if (length < array.Length)
                        {
                            var arr = new byte[length];
                            Array.Copy(array, arr, length);
                            array = arr;
                        }
                        bytes.AddRange(array);
                    }
                    UserCookies = Cookies.GetCookiesByCookieContainer(requst.CookieContainer);
                }
            }
            catch (WebException wex)
            {
                var stream = wex.Response?.GetResponseStream();
                if (stream == null)
                    return Encoding.UTF8.GetBytes("{\"errno\":-10,\"message\":\"链接服务器超时\"}");
                while (true)
                {
                    var array = new byte[1024];
                    var length = stream.Read(array, 0, array.Length);
                    if (length <= 0) break;
                    if (length < array.Length)
                    {
                        var arr = new byte[length];
                        Array.Copy(array, arr, length);
                        array = arr;
                    }
                    bytes.AddRange(array);
                }
                UserCookies = Cookies.GetCookiesByCookieContainer(requst.CookieContainer);

            }
            catch (Exception ex)
            {
                return Encoding.UTF8.GetBytes("{\"errno\":-11,\"message\":\"链接服务器出现未知错误\"}");
            }
            return bytes.ToArray();
        }

        /// <summary>
        /// Get获取返回的Byes(异步)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="ua"></param>
        /// <param name="referer"></param>
        /// <returns></returns>
        public async Task<byte[]> GetBytesAsync(string url, string ua = null, string referer = null)
        {
            return await Task.Run(() => GetBytes(url, ua, referer));
        }


        /// <summary>
        /// 获取重定向链接
        /// </summary>
        /// <param name="url"></param>
        /// <param name="ua"></param>
        /// <param name="referer"></param>
        /// <returns></returns>
        public string RealUrl(string url, string ua = null, string referer = null)
        {
            var requst = WebRequest.CreateHttp(url);
            requst.CookieContainer = new CookieContainer();
            if (UserCookies != null)
            {
                foreach (var key in UserCookies.GetKeys())
                {
                    var cookie = new Cookie(key, UserCookies.GetCookie(key)) { Domain = Domain };
                    requst.CookieContainer.Add(cookie);
                }
            }
            requst.UserAgent = ua ?? "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
            requst.Method = "GET";
            requst.Timeout = 5000;
            //requst.Referer = referer ?? string.Empty;
            requst.AllowAutoRedirect = false;
            try
            {
                using (var resp = requst.GetResponse() as HttpWebResponse)
                {
                    return resp?.Headers["Location"];
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
