using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sample.Data;
using Sample.Models;
using Sample.Services;
using Aiplugs.Functions.Core;
using System.Data;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json.Linq;

namespace Sample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("Aiplugs:Function:Sample"));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddSingleton(SampleDb.Instance);

            // Add application services.
            services.AddTransient<IContextFactory<JObject>, SampleContextFactory>();
            services.AddTransient<ILockService, SampleLockService>();
            services.AddTransient<IUserResolver, SampleUserResolver>();
            services.AddTransient<IProcedureResolver, SampleProcedureResolver>();

            services.AddAiplugsFunctions<JObject>(options => options.UseSqlite().ForceMigration());
            
            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseAiplugsFunctions<JObject>();

            lifetime.ApplicationStopped.Register(() => SampleDb.CloseAndDispose());
        }
    }
}
