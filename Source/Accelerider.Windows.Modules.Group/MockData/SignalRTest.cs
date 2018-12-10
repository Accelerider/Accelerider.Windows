using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace Accelerider.Windows.Modules.Group.MockData
{
    public class SignalRTest
    {
        public void TestMethod()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:2333/groups",
                    options =>
                    {
                        options.AccessTokenProvider = () => Task.FromResult("753951");
                    }).Build();

        }
    }
}
