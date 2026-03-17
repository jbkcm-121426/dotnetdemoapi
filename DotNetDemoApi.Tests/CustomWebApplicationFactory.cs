using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DotNetDemoApi.Tests
{
    /// <summary>
    /// Sets ContentRoot to the API project directory so WebApplicationFactory finds the app in CI (e.g. Linux).
    /// </summary>
    public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var contentRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "DotNetDemoApi"));

            if (!Directory.Exists(contentRoot))
            {
                var baseDir = Path.GetDirectoryName(typeof(CustomWebApplicationFactory).Assembly.Location);
                contentRoot = Path.GetFullPath(Path.Combine(baseDir!, "..", "..", "..", "..", "DotNetDemoApi"));
            }

            if (Directory.Exists(contentRoot))
            {
                builder.UseContentRoot(contentRoot);
            }
        }
    }
}
