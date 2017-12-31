using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.Core
{
    public interface IAcceleriderApi
    {
        [Post("http://api.usmusic.cn/login")]
        Task<string> Login(string username, string password, string security = "RSA", string clientType = "wpf");
    }
}
