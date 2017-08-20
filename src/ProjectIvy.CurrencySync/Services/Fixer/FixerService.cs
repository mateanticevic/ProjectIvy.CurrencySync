using Newtonsoft.Json;
using ProjectIvy.CurrencySync.Models;
using ProjectIvy.CurrencySync.Services.Fixer.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace ProjectIvy.CurrencySync.Services.Fixer
{
    public class FixerService : ICurrencyService
    {
        public async Task<IEnumerable<ExchangeRate>> GetRatesOnDate(DateTime date)
        {
            using (var client = new HttpClient())
            {
                var jsonRaw = await client.GetStringAsync($"http://api.fixer.io/{date.ToString("yyyy-MM-dd")}");

                var data = JsonConvert.DeserializeObject<GetByDate>(jsonRaw);

                return data.ToModel();
            }
        }
    }
}
