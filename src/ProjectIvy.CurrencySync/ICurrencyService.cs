using System.Threading.Tasks;
using System;
using ProjectIvy.CurrencySync.Services.Fixer.Models;

namespace ProjectIvy.CurrencySync
{
    interface ICurrencyService
    {
        Task<RatesOnDate> GetRatesOnDate(DateTime date, string apiKey);
    }
}
