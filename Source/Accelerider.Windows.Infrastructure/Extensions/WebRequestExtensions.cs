using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http.Headers;

namespace System.Net
{
    public static class WebRequestExtensions
    {
        public static void SetHeaderValue(this WebHeaderCollection header, string name, string value)
        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (property != null)
            {
                var collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = value;
            }
        }

        public static WebHeaderCollection ToWebHeaderCollection(this HttpHeaders headers)
        {
            var result = new WebHeaderCollection();
            foreach (var pair in headers)
                result.SetHeaderValue(pair.Key,pair.Value.First());
            return result;
        }

	    public static void SetHeaders(this WebHeaderCollection target, WebHeaderCollection headers)
	    {
		    foreach (var key in headers.AllKeys)
			    target.SetHeaderValue(key,headers.Get(key));
	    }
	    public static IEnumerable<Cookie> ToCookies(this string cookie, string domain)
	    {
		    return from item in cookie.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
			    let kv = item.Split('=')
			    let name = kv.FirstOrDefault().Trim()
			    let value = kv.Length > 1 ? kv.LastOrDefault() : string.Empty
			    select new Cookie(name, value) { Domain = domain };
	    }

	    public static string ToString(this IEnumerable<Cookie> cookies, bool flag) => string.Join("; ", cookies.Select(v => v.Name + "=" + v.Value));

	    public static CookieContainer ToCookieContainer(this string cookie, string domain)
	    {
		    var result = new CookieContainer();
		    foreach (var sub in cookie.ToCookies(domain))
			    result.Add(sub);
		    return result;
	    }

        public static void AddRangeBasedOffsetLength(this HttpWebRequest @this, long offset, long length)
        {
            @this.AddRange(offset, offset + length - 1);
        }
    }
}
