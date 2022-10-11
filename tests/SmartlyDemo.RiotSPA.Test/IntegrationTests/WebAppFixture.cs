using Alba;
using System.Threading.Tasks;
using Xunit;

namespace SmartlyDemo.RiotSPA.Test.IntegrationTests
{
    public class WebAppFixture<TStartup>: IAsyncLifetime where TStartup : class
    {
        public IAlbaHost AlbaHost = null!;

        public async Task InitializeAsync()
        {
            AlbaHost = await Alba.AlbaHost.For<TStartup>(builder =>
            {
                // Configure all the things
            });
        }

        public async Task DisposeAsync()
        {
            await AlbaHost.DisposeAsync();
        }
    }
}
