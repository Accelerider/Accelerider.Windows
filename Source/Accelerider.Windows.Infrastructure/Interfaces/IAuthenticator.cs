using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public interface IAuthenticator<T> where T : INetDiskUser
    {
        WebBrowser GetBrowser();

        Task<T> Authenticate();
    }
}
