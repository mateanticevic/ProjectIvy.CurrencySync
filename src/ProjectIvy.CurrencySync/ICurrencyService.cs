using ProjectIvy.CurrencySync.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ProjectIvy.CurrencySync
{
    interface ICurrencyService
    {
        Task<IEnumerable<ExchangeRate>> GetRatesOnDate(DateTime date);
    }
}
