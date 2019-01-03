using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Accelerider.Windows.Modules.NetDisk
{
    public static class ApiExtensions
    {
        public static T RunApi<T>(this Task<JToken> @this)
        {
            throw new NotImplementedException();
        }
    }
}
