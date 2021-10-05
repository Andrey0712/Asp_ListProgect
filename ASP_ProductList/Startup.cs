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
            //  Додає служби для роботи локалізації
            services.AddLocalization(opts =>
            {
                //  Задання стандартного шляху для ресурсів
                opts.ResourcesPath = "Resources";
            });

            //  Реєстрування екземпляру конфігурації. Він мінятиме
            //  дані конфігурації автоматично при їх зміні
            //  RequestLocalizationOptions - тип, який задає налашутвання для Middleware,
            //  а саме для налаштувань запиту
            services.Configure<RequestLocalizationOptions>(options =>
            {
                //  Створення колеції мов, які підтримуються на сайті
                var supportedCultures = new[]
                {
                    new CultureInfo("uk"),
                    new CultureInfo("en"),
                };

                //  Встановлення культури за замовчанням
                options.DefaultRequestCulture = new RequestCulture("uk");
                //  Задає мови, які буде підтримувати сайт
                options.SupportedCultures = supportedCultures;
                //  Задає мови, які буде підтримувати сайт
                options.SupportedUICultures = supportedCultures;
            });

            services.AddControllersWithViews()
                //  Додає на сайт служби локалізації для View
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            ////  Додає налашутвання для локалізації анотацій у проект
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

            //  Отримання налаштувань RequestLocalizationOptions, які були передані у ConfigureServices,
            //  Через залежності проекта.
            //  Інтерфейс IOptions використовуються для отримання налаштованих конфігурацій,
            //  ті що прописуються у методі IServiceCollection.Configure<TOptions>
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
                //  Задання маршруту для використання локалізації
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
