using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure
{
    public class RoutingNode
    {
        public RoutingNode(string url)
        {
            Url = url;
        }

        public string Url { get; }

        public RoutingNode Parent { get; set; }


        public Task<T> GetAsync<T>(string url) { throw new NotImplementedException(); }

        public Task<T> PostAsync<T>(string url) { throw new NotImplementedException(); }

        public Task<bool> DeleteAsync(string url) { throw new NotImplementedException(); }

        public Task<T> PatchAsync<T>(string url) { throw new NotImplementedException(); }

        public Task<T> PutAsync<T>(string url) { throw new NotImplementedException(); }

        public override string ToString()
        {
            var url = Url;
            var parent = Parent;
            while (!string.IsNullOrEmpty(parent?.Url))
            {
                url = parent.Url + url;
                parent = parent.Parent;
            }

            return url;
        }
    }
}
