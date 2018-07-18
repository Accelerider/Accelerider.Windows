using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.Models
{
    internal class SignUpInfoBody
    {
	    public string Email { get; set; }
	    public string Username { get; set; }
	    public string Password { get; set; }
	    public string SessionId { get; set; }
	    public string VerificationCode { get; set; }
	}

    internal class LoginInfoBody
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    internal class UserUpdateInfoBody
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EmailVisibility { get; set; }
        public IList<long> ModuleIds { get; set; }
    }
}
