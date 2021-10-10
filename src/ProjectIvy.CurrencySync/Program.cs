using ProjectIvy.CurrencySync.Services.Fixer;
using System.Threading.Tasks;
using System;
using Serilog;
using Serilog.Sinks.Graylog;
using Serilog.Events;
using Serilog.Sinks.Graylog.Core.Transport;

namespace ProjectIvy.CurrencySync
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
                                      .MinimumLevel.Override(nameof(Microsoft), LogEventLevel.Information)
                                      .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                                      .Enrich.FromLogContext()
                                      .WriteTo.Console()
                                      .WriteTo.Graylog(new GraylogSinkOptions()
                                      {
                                          Facility = "project-ivy-currency-sync",
                                          HostnameOrAddress = "10.0.1.24",
                                          Port = 12202,
                                          TransportType = TransportType.Udp
                                      })
                                      .CreateLogger();

            Log.Logger.Information("Application started");

            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            var date = await DbHandler.GetLastExchangeRateDate(connectionString);

            while (date < DateTime.Now.Date)
            {
                try
                {
                    await ResolveRatesOnDay(connectionString, date);

                    Log.Logger.Information("Currencies synced for {Date}", date.ToString("yyyy-MM-dd"));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Something went wrong, message: {e.Message}");
                    Log.Logger.Error(e, "Currency sync failed for {Date}", date.ToString("yyyy-MM-dd"));
                }

                date = date.AddDays(1);
            }
        }

        public static async Task ResolveRatesOnDay(string connectionString, DateTime day)
        {
            string apiKey = Environment.GetEnvironmentVariable("API_KEY");
            var rates = await new FixerService().GetRatesOnDate(day, apiKey);
            await DbHandler.InsertOrUpdateExchangeRate(connectionString, rates);
        }
    }
}
