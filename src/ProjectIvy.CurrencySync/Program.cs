using Microsoft.Extensions.Configuration;
using ProjectIvy.CurrencySync.Services.Fixer;
using System.IO;
using System.Threading.Tasks;
using System;

namespace ProjectIvy.CurrencySync
{
    public class Program
    {
        public static IConfigurationRoot Configuration { get; set; }

        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                    .AddJsonFile("appsettings.json");
            Configuration = builder.Build();

            string connectionString = Configuration["ConnectionStrings:MainDb"].ToString();

            var date = DbHandler.GetLastExchangeRateDate(connectionString).Result;

            while (!Equals(date.Date, DateTime.Now.Date))
            {
                try
                {
                    var work = ResolveRatesOnDay(connectionString, date);
                    work.Wait();

                    Console.WriteLine($"Day {date.ToString("yyyy-MM-dd")} ok.");
                }
                catch (Exception)
                {
                    Console.WriteLine("Duplicate day.");
                }

                date = date.AddDays(1);
            }
        }

        public static async Task ResolveRatesOnDay(string connectionString, DateTime day)
        {
            ICurrencyService x = new FixerService();

            var rates = await x.GetRatesOnDate(day);

            foreach (var rate in rates)
            {
                await DbHandler.InsertCurrencyExchangeRate(connectionString, rate);
            }
        }
    }
}
