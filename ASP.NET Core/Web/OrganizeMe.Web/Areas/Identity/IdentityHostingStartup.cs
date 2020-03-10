using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(OrganizeMe.Web.Areas.Identity.IdentityHostingStartup))]

namespace OrganizeMe.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => { });
        }
    }
}
