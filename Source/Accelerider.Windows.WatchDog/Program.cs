using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Accelerider.Windows.WatchDog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            (string name, int age) a = ("123", 123);
            var json = JsonConvert.SerializeObject(a);
            var aa = JsonConvert.DeserializeObject(json, typeof((string, int)));


            var server = new NamedPipeServerStream("");
            using (var sr = new StreamReader(server))
            {
                sr.ReadToEndAsync();
            }
            var client = new NamedPipeClientStream("");
            using (var sw = new StreamWriter(client))
            {
                sw.WriteAsync("");
            }
                 

            var responder = new NamedPipeResponder<MockClient>();

            responder.Subscribe<string>(nameof(MockClient.SetName), (o, value) => o.SetName(value));
        }
    }

    public class MockClient
    {
        public void Print()
        {

        }

        public void SetName(string value)
        {

        }
    }

    public class NamedPipeResponder<T>
    {

        public void Subscribe<TArgs>(string methodName, Action<T, TArgs> subscriber)
        {
        }
    }

    public class NamedPipeRequester
    {

    }
}
