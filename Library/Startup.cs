using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryNet2020.ControllerHelpers;
using LibraryNet2020.Controllers;
using LibraryNet2020.Models;
using LibraryNet2020.NonPersistentModels;
using LibraryNet2020.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LibraryNet2020
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
            services.AddDbContext<LibraryContext>(options =>
                options.UseSqlite("Library"));
            services.AddControllersWithViews();
            services.AddScoped<CheckOutService, CheckOutService>();
            services.AddScoped<CheckInService, CheckInService>();
            services.AddScoped<HoldingsService, HoldingsService>();
            services.AddScoped<BranchesService, BranchesService>();
            services.AddScoped<PatronsService, PatronsService>();
            services.AddScoped<IClassificationService, MasterClassificationService>();
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
            });
        }
    }
}