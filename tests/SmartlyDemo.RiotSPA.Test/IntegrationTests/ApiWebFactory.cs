using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmartlyDemo.RiotSPA.Test.IntegrationTests
{
    public class ApiWebFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        // We set up our test API server with this override
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {

            // We configure our services for testing
            builder.ConfigureTestServices(services =>
            {
            });
        }
    }
}
