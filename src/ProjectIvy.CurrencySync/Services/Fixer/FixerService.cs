using Newtonsoft.Json;
using ProjectIvy.CurrencySync.Services.Fixer.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace ProjectIvy.CurrencySync.Services.Fixer
{
    public class FixerService : ICurrencyService
    {
        public async Task<RatesOnDate> GetRatesOnDate(DateTime date, string apiKey)
        {
            using (var client = new HttpClient())
            {
                var jsonRaw = await client.GetStringAsync($"http://data.fixer.io/api/{date.ToString("yyyy-MM-dd")}?access_key={apiKey}");

                return JsonConvert.DeserializeObject<RatesOnDate>(jsonRaw);
            }
        }
    }
}
