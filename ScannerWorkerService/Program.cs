using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScannerWorkerService.Extensions;
using Serilog;
using Serilog.Events;

namespace ScannerWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configurations = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
               .Enrich.FromLogContext()
               .ReadFrom.Configuration(configurations)

               //Instead of supplying the logfile path here with a txt logfile, it is configured under appsettings with json formatted log; 
               //so we can easily change the file "appsettings.json" depends on the environment without rebuilding the whole app
               //.WriteTo.File(@"C:\temp\scannerworkerservice\LogFile.txt")
               .CreateLogger();

            try
            {
                Log.Information("Starting the scanner worker service");
                CreateHostBuilder(args).Build().Run();
                return;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "There was a problem starting the scanner worker service!");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseSerilog();
                })
                .UseWindowsService()        //To enable service installation under Windows Services
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var configuration = config.Build();
                    var connectionString = configuration.GetConnectionString("Default");
                    config.AddDbProvider(options => options.UseSqlServer(connectionString));
                });
    }
}
