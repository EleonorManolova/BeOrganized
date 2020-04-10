using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(BeOrganized.Web.Areas.Identity.IdentityHostingStartup))]

namespace BeOrganized.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => { });
        }
    }
}
