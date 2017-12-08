using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using CityInfo.API.Services;
using Microsoft.Extensions.Configuration;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API
{
    public class Startup
    {
        //public static IConfigurationRoot Configuration; For Core 1.0
        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            //Default way for ASP.NET Core 1.0, works also for Core 2.0, but for 2.0 we can do easier
            //var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath).AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
            //.AddJsonFile($"appSettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
            //Configuration = builder.Build();
            //AddEnvironmentVariables()
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                //.AddXmlDataContractSerializerFormatters()
                .AddMvcOptions(o => o.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter()))
                ;
            //.AddJsonOptions(o => {
            //    var castedResolver = o.SerializerSettings.ContractResolver as DefaultContractResolver;
            //    castedResolver.NamingStrategy = null;
            //});

            //services.AddTransient<LocalMailService>(); - provide concrete type
#if DEBUG
            services.AddTransient<IMailService, LocalMailService>();
#else
            services.AddTransient<IMailService, CloudMailService>();
#endif
            //services.AddDbContext<CityInfoContext>(o => o.UseSqlServer(@"Server=node1adventure.database.windows.net;Database=CityInfoDB;User id=iren@node1adventure.database.windows.net;password=CoOk999!;")); //scoped lifetime
            var connectionString = Startup.Configuration["connectionStrings:cityInfoDBConnectionString"];
            services.AddDbContext<CityInfoContext>(o => o.UseSqlServer(connectionString)); //use Windows credentials
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, CityInfoContext cityInfoContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                loggerFactory.AddDebug();
            }
            else
            {
                app.UseExceptionHandler();
            }

            //loggerFactory.AddProvider(new NLog.Extensions.Logging.NLogLoggerProvider());
            loggerFactory.AddNLog();
            loggerFactory.ConfigureNLog("nlog.config");

            cityInfoContext.EnsureSeedDataForContext();

            app.UseStatusCodePages();
            Debug.WriteLine(env.ContentRootPath);

            app.UseMvc();

            //app.Run(async (context) =>
            //{
            //    throw new Exception("My exception :)");
            //});
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
