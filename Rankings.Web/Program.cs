using System;
using System.IO;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Rankings.Web
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var configuration = CreateConfiguration();

            var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
            telemetryConfiguration.InstrumentationKey = "b74bd4fb-1f2b-4326-9a63-97ed60202962";
            
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces)
                .WriteTo.Console()
                .Enrich.FromLogContext()
                .CreateLogger();

            try
            {
                Log.Information("Create and run host");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IConfiguration CreateConfiguration()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var builder = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddUserSecrets<Startup>();

            return builder.Build();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(
                    webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                        webBuilder.UseSerilog();
                        webBuilder.CaptureStartupErrors(false);
                    });
    }
}