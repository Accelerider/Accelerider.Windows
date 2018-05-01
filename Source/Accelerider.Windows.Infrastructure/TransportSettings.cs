using System.Net.Http.Headers;

namespace Accelerider.Windows.Infrastructure
{
    public class TransportSettings
    {
        public HttpHeaders Headers { get; set; }

        internal TransportSettings() { }
    }
}
