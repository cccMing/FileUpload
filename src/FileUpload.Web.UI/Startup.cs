using FileUpload.Models;
using FileUpload.Services;
using FileUpload.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace FileUpload
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
            services.AddControllersWithViews();

            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            services.AddTransient<UploadSettingsService>();
            services.AddTransient<FileService>();
            services.AddTransient<UrlBuilder>();
            services.AddTransient<Factory>();

            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();

            services.AddScoped<UploadSettings>(services =>
            {
                ActionContext actionContext = services.GetRequiredService<IActionContextAccessor>().ActionContext;
                UploadSettingsService configurationService = services.GetRequiredService<UploadSettingsService>();
                UploadSettings configuration = configurationService.Find(actionContext.RouteData, actionContext.HttpContext.User);
                return configuration;
            });
            services.AddTransient<UrlToken>(services =>
            {
                ActionContext actionContext = services.GetRequiredService<IActionContextAccessor>().ActionContext;
                UploadSettingsService configurationService = services.GetRequiredService<UploadSettingsService>();
                return configurationService.FindUrlToken(actionContext.RouteData);
            });
            services.AddTransient<IUrlHelper>(services =>
            {
                ActionContext actionContext = services.GetRequiredService<IActionContextAccessor>().ActionContext;
                IUrlHelperFactory factory = services.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });
            services.AddTransient<ProfileListViewModel>(services =>
            {
                ActionContext actionContext = services.GetRequiredService<IActionContextAccessor>().ActionContext;
                return services.GetRequiredService<Factory>().CreateProfileList(actionContext.HttpContext.User);
            });
            services.AddTransientProvider<UrlToken>();

            services.Configure<UploadOptions>(Configuration.GetSection("Upload"));
            services.Configure<AccountOptions>(Configuration.GetSection("Authentication"));
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
                app.UseExceptionHandler("/error");
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseStatusCodePages();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
