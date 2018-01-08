using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refit;

namespace Accelerider.Windows.Models
{

    public interface IAcceleriderApi
    {
        [Get("/publickey")]
        Task<string> GetPublicKeyAsync();


    }
}
