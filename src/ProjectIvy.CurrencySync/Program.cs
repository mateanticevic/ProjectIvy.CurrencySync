using ProjectIvy.CurrencySync.Services.Fixer;
using System.Threading.Tasks;
using System;

namespace ProjectIvy.CurrencySync
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            var date = DateTime.Now.Date;
            var last = await DbHandler.GetLastExchangeRateDate(connectionString);

            while (date > last)
            {
                try
                {
                    await ResolveRatesOnDay(connectionString, date);

                    Console.WriteLine($"Day {date:yyyy-MM-dd} ok.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Something went wrong, message: {e.Message}");
                }

                date = date.AddDays(-1);
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
