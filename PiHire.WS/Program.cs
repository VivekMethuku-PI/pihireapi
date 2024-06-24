using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace PiHire.WS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Log.Logger = new LoggerConfiguration()
            //    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
            //    .Enrich.FromLogContext()
            //    .WriteTo.Console()
            //    .CreateLogger();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Logger(c =>
                    c.Filter.ByIncludingOnly(e => e.Level != Serilog.Events.LogEventLevel.Error && e.Level != Serilog.Events.LogEventLevel.Fatal)
                        .WriteTo.File($"{AppDomain.CurrentDomain.BaseDirectory}\\Logs\\mainInfo.log"))
                .WriteTo.Logger(c =>
                    c.Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Error || e.Level == Serilog.Events.LogEventLevel.Fatal)
                        .WriteTo.File($"{AppDomain.CurrentDomain.BaseDirectory}\\Logs\\mainError.log"))
                .CreateLogger();

            try
            {
                Log.Information("Starting piHire Windows service host");
                CreateHostBuilder(args).Build().Run();
                Log.Information("Started piHire Windows service host");
                return;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return;
            }
            finally
            {
                Log.Information("Closing piHire Windows service host");
                Log.CloseAndFlush();
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .UseSerilog((hostContext, services, configuration) => configuration
                    .ReadFrom.Configuration(hostContext.Configuration)
                    //.ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    /*.WriteTo.Console()*/)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<cronWorker>();
                });
    }
}
