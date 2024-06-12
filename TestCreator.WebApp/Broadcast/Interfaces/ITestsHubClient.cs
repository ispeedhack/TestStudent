using System.Threading.Tasks;

namespace TestCreator.WebApp.Broadcast.Interfaces
{
    public interface ITestsHubClient
    {
        Task TestCreated();

        Task TestRemoved(int id);
    }
}
