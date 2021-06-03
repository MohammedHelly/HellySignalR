using Customer.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Customer.Hubs;
using Microsoft.AspNetCore.SignalR;
//using Microsoft.AspNet.SignalR;

namespace Customer
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
            var connStr = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<CustomerContext>((serviceProvider, options) =>
            {
                var connStr = Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connStr);
            });
            services.AddScoped<CusHub, CusHub>();
            //services.AddScoped<IHubContext<CusHub>, IHubContext<CusHub>>();
            
            services.AddSignalR();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
          
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"); 
                endpoints.MapHub<CusHub>("Hubs/CusHub");
                endpoints.MapFallbackToController("Index", "Fallback");
            });
            //app.Use(async (context, next) =>
            //{
            //    var hubContext = context.RequestServices
            //                            .GetRequiredService<IHubContext<CusHub>>();
               
            //      if (next != null)
            //    {
            //        await next.Invoke();
            //    }
            //});
        }
    }
}
