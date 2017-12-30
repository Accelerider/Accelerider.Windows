using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.MockData
{
    public class Message
    {
        public string Author { get; set; }
        public string Text { get; set; }
        public Uri HeadUri { get; set; }
        public DateTime Date { get; set; }
    }
}
