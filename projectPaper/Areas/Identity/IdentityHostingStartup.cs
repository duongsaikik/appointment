using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using projectPaper.Data;

[assembly: HostingStartup(typeof(projectPaper.Areas.Identity.IdentityHostingStartup))]
namespace projectPaper.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<projectPaperContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("projectPaperContextConnection")));

                services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<projectPaperContext>();
            });
        }
    }
}