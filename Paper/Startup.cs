using ASC.Business;
using ASC.Business.Interface;
using AutoMapper;
using DataAccess;
using DataAccess.Interfaces;
using ElCamino.AspNetCore.Identity.AzureTable.Model;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Paper.Data;
using Paper.Hubs;
using Paper.Models;
using Paper.Service;
using Paper.Web.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;
namespace Paper
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            Configuration = builder.Build();
            
        }

        public IConfiguration Configuration { get; }
       
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddMvc();
            services.AddOptions();
            services.AddDistributedMemoryCache();
            services.AddSession();

            services.Configure<ApplicationSettings>(Configuration.GetSection("Appsettings"));
            services.AddIdentity<ApplicationUser, ApplicationRole>((options) =>
            {
                options.User.RequireUniqueEmail = true;
            })
                 .AddAzureTableStores<ApplicationDbContext>(new Func<IdentityConfiguration>(() =>
                 {
                     IdentityConfiguration idconfig = new IdentityConfiguration();
                     idconfig.TablePrefix = Configuration.GetSection("IdentityAzureTable:IdentityConfiguration:TablePrefix").Value;
                     idconfig.StorageConnectionString = Configuration.GetSection("IdentityAzureTable:IdentityConfiguration:StorageConnectionString").Value;
                     idconfig.LocationMode = Configuration.GetSection("IdentityAzureTable:IdentityConfiguration:LocationMode").Value;
                     return idconfig;
                 }))
                  .AddDefaultTokenProviders()
            .CreateAzureTablesIfNotExists<ApplicationDbContext>();
            services.AddSingleton<IIdentitySeed, IdentitySeed>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUnitOfWork>(p => new UnitOfWork(Configuration.GetSection("ConnectionStrings:DefaultConnection").Value));
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;

            });
            services.AddMvc().AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = null;
                o.JsonSerializerOptions.DictionaryKeyPolicy = null;
            });
            services.AddSingleton<IEmailSender, AuthMessageSender>();
            services.AddScoped<IMasterDataOperations, MasterDataOperations>();
            services.AddAutoMapper();
           
           
            services.AddAuthentication()
             .AddGoogle(options =>
             {
                 IConfigurationSection googleAuthenNSection =
                 Configuration.GetSection("Authentication:Google");
                 options.ClientId = Configuration["Google:Identity:ClientId"];
                 options.ClientSecret = Configuration["Google:Identity:ClientSecret"];
             });
            var assembly = typeof(ASC.Utilities.Navigation.LeftNavigationViewComponent).GetTypeInfo().Assembly;
            var embeddedFileProvider = new EmbeddedFileProvider(assembly, "ASC.Utilities");
            services.AddControllersWithViews().AddRazorRuntimeCompilation(options =>
            {
                options.FileProviders.Add(embeddedFileProvider);
            });
            services.AddScoped<IMasterDataCacheOperations, MasterDataCacheOperations>();
            services.AddSingleton<INavigationCacheOperations, NavigationCacheOperations>();
            services.AddScoped<IServiceRequestOperations, ServiceRequestOperations>();
            services.AddScoped<IServiceRequestMessageOperations, ServiceRequestMessageOperations>();
          
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env, IIdentitySeed storageSeed,
              IMasterDataCacheOperations masterDataCacheOperations,
              INavigationCacheOperations navigationCacheOperations,
        
            
            IUnitOfWork unitOfWork)
            
        {
           
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseSession();
            app.UseAuthentication();
            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllerRoute(
                    name: "areaRoute",
                   pattern: "{area:exists}/{controller=Admin}/{action=Index}"
                   );
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                
                endpoints.MapRazorPages();
                endpoints.MapHub<ChatHub>("/chatHub");
            });

         
            app.UseWebSockets();
       

            using (var scope = app.ApplicationServices.CreateScope())
            {
                await storageSeed.Seed(scope.ServiceProvider.GetService<UserManager<ApplicationUser>>(),
                                        scope.ServiceProvider.GetService<RoleManager<ApplicationRole>>(),
                                         scope.ServiceProvider.GetService<IOptions<ApplicationSettings>>());
            }
            var models = Assembly.Load(new AssemblyName("ASC.Models")).GetTypes().Where(type => type.Namespace == "ASC.Models.Models");
            foreach (var model in models)
            {
                var repositoryInstance = Activator.CreateInstance(typeof(Respository<>).MakeGenericType(model), unitOfWork);
                MethodInfo method = typeof(Respository<>).MakeGenericType(model).GetMethod("CreateTableAsync");
                method.Invoke(repositoryInstance, new object[0]);

            }
             await masterDataCacheOperations.CreateMasterDataCacheAsync();
            await navigationCacheOperations.CreateNavigationCacheAsync();
        }
      
    }
}
