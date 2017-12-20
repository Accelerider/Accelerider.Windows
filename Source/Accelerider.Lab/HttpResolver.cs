using System;
using System.Net.Http;

namespace Accelerider.Lab
{
    internal class HttpResolver
    {
        private HttpClient _httpClient;
        private HttpMessageHandler _httpMessageHandler;


        public HttpResolver()
        {
            _httpMessageHandler = new HttpClientHandler();
            _httpClient = new HttpClient(_httpMessageHandler);
            
        }


        public void Get(string url)
        {

        }

        public void Post()
        {

        }

        public void Then()
        {

        }

        public void Catch()
        {

        }
    }
}