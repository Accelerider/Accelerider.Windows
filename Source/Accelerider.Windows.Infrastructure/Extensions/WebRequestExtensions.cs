using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Extensions
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

    }
}
