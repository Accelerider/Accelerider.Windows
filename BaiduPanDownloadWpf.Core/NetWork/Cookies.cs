using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Newtonsoft.Json;

namespace BaiduPanDownloadWpf.Core.NetWork
{
    /// <summary>
    /// 传输用Cookies容器
    /// </summary>
    public class Cookies
    {
        /// <summary>
        /// Cookies字符串
        /// </summary>
        public string CookiesStr
        {
            get
            {
                return JsonConvert.SerializeObject(_cookiesKv);
            }
            set
            {
                try
                {
                    _cookiesKv = JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
                }
                catch
                {
                    var list = value.Split(';');
                    _cookiesKv.Clear();
                    foreach (var cookie in list)
                    {
                        if (cookie.Contains("="))
                        {
                            _cookiesKv.Add(cookie.Split('=')[0].Replace(" ", string.Empty), cookie.Split('=')[1]);
                        }
                    }
                }
            }
        }

        Dictionary<string, string> _cookiesKv = new Dictionary<string, string>();

        public string GetCookie(string Key)
        {
            return !Contains(Key) ? string.Empty : _cookiesKv[Key].Replace(",", string.Empty);
        }

        public string[] GetKeys()
        {
            var keys = new string[_cookiesKv.Count];
            var i = 0;
            foreach (var key in _cookiesKv)
            {
                keys[i] = key.Key;
                i++;
            }
            return keys;
        }

        public bool Contains(string key)
        {
            return _cookiesKv.ContainsKey(key);
        }

        public static Cookies GetCookiesByCookieContainer(CookieContainer cookieContainer)
        {
            var cookies = new Cookies();
            foreach (Cookie cookie in GetAllCookies(cookieContainer))
            {
                cookies.SetData(cookie.Name, cookie.Value);
            }
            return cookies;
        }

        private static CookieCollection GetAllCookies(CookieContainer cookieContainer)
        {
            var cookies = new CookieCollection();
            foreach (var item in (cookieContainer.GetType().GetRuntimeFields().FirstOrDefault(v => v.Name == "m_domainTable").GetValue(cookieContainer) as IDictionary).Values)
            {
                var type = item.GetType().GetRuntimeFields().First(v => v.Name == "m_list");
                foreach (var sub in ((IDictionary)type.GetValue(item)).Values)
                {
                    cookies.Add((CookieCollection)sub);
                }
            }
            return cookies;
        }


        public Cookies Copy()
        {
            return new Cookies { CookiesStr = CookiesStr };
        }

        public void SetData(string K, string V)
        {
            if (!_cookiesKv.ContainsKey(K))
            {
                _cookiesKv.Add(K, V);
                return;
            }
            _cookiesKv[K] = V;
        }
    }
}
