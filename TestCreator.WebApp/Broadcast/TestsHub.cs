using Microsoft.AspNetCore.SignalR;
using TestCreator.WebApp.Broadcast.Interfaces;

namespace TestCreator.WebApp.Broadcast
{
    public class TestsHub : Hub<ITestsHubClient>
    {
    }
}
