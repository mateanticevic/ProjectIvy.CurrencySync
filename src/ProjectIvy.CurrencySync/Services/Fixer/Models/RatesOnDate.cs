using System.Collections.Generic;

namespace ProjectIvy.CurrencySync.Services.Fixer.Models
{
    public class RatesOnDate
    {
        public string Base { get; set; }

        public string Date { get; set; }

        public IDictionary<string, decimal> Rates { get; set; }
    }
}
