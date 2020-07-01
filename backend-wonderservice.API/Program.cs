using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend_wonderservice.DATA.Abstration;
using backend_wonderservice.DATA.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mindscape.Raygun4Net;
using NLog.Extensions.Logging;
using WonderService.Data.Services;

namespace backend_wonderservice.API
{
    public class Program
    {
        static RaygunClient _client = new RaygunClient("vo0odiFrRcVsN7uIxVtaUw");
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userRole = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var role = new RoleCreator();
                    role.Create(userRole).Wait();
                }
                catch (Exception e)
                {
                    var logger = services.GetRequiredService<ILogger<DataContext>>();
                    logger.LogError(e, "An Error Occured While Migrating Database");
                }

                host.Run();

            }
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>

            Host.CreateDefaultBuilder(args).ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddEventSourceLogger();
                    logging.AddNLog();
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .ConfigureAppConfiguration((hostContext, builder) =>
                {

                    if (hostContext.HostingEnvironment.IsDevelopment())
                    {
                        builder.AddUserSecrets<Program>();
                    }
                });
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _client.Send(e.ExceptionObject as Exception);
        }
    }
}
