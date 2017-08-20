namespace ProjectIvy.CurrencySync.Services.Fixer.Models
{
    public class GetByDate
    {
        public string Base { get; set; }
        public string Date { get; set; }
        public ExchangeRates Rates { get; set; }
    }
}
