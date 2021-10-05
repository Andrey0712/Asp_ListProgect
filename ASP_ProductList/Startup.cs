using AppProductList.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ASP_ProductList
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
            //  ���� ������ ��� ������ ����������
            services.AddLocalization(opts =>
            {
                //  ������� ������������ ����� ��� �������
                opts.ResourcesPath = "Resources";
            });

            //  ����������� ���������� ������������. ³� �������
            //  ��� ������������ ����������� ��� �� ���
            //  RequestLocalizationOptions - ���, ���� ���� ������������ ��� Middleware,
            //  � ���� ��� ����������� ������
            services.Configure<RequestLocalizationOptions>(options =>
            {
                //  ��������� ������� ���, �� ������������ �� ����
                var supportedCultures = new[]
                {
                    new CultureInfo("uk"),
                    new CultureInfo("en"),
                };

                //  ������������ �������� �� �����������
                options.DefaultRequestCulture = new RequestCulture("uk");
                //  ���� ����, �� ���� ����������� ����
                options.SupportedCultures = supportedCultures;
                //  ���� ����, �� ���� ����������� ����
                options.SupportedUICultures = supportedCultures;
            });

            services.AddControllersWithViews()
                //  ���� �� ���� ������ ���������� ��� View
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            ////  ���� ������������ ��� ���������� �������� � ������
            .AddDataAnnotationsLocalization();

            services.AddDbContext<EFAppContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
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
            }
            app.UseStaticFiles();

            app.UseRouting();

            //  ��������� ����������� RequestLocalizationOptions, �� ���� ������� � ConfigureServices,
            //  ����� ��������� �������.
            //  ��������� IOptions ���������������� ��� ��������� ������������ ������������,
            //  � �� ������������ � ����� IServiceCollection.Configure<TOptions>
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            app.UseAuthorization();
            var dirName = "products";
            var dirServer = Path.Combine(Directory.GetCurrentDirectory(), dirName);
            if (!Directory.Exists(dirServer))
            {
                Directory.CreateDirectory(dirServer);
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(dirServer),
                RequestPath = "/images"
            });


            app.UseEndpoints(endpoints =>
            {
                //  ������� �������� ��� ������������ ����������
                endpoints.MapControllerRoute(
                    name: "defaultlocal",
                    pattern: "{lang=uk}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
