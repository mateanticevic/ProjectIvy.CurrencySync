using Dapper;
using ProjectIvy.CurrencySync.Models;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System;

namespace ProjectIvy.CurrencySync
{
    public class DbHandler
    {
        public static async Task<DateTime> GetLastExchangeRateDate(string connectionString)
        {
            string sql = @"SELECT TOP 1 Timestamp FROM Common.CurrencyRate ORDER BY Timestamp DESC";

            using (var db = new SqlConnection(connectionString))
            {
                return await db.ExecuteScalarAsync<DateTime>(sql, commandType: System.Data.CommandType.Text);
            }
        }

        public static async Task<bool> InsertCurrencyExchangeRate(string connectionString, ExchangeRate rate)
        {
            var parameters = new
            {
                from = rate.FromCurrency,
                to = rate.ToCurrency,
                date = rate.Date.ToString("yyyy-MM-dd"),
                rate = rate.Rate
            };

            string sql = @"DECLARE @fromId INT = (SELECT TOP 1 Id FROM Common.Currency WHERE Code = @from)
                           DECLARE @toId INT = (SELECT TOP 1 Id FROM Common.Currency WHERE Code = @to)

                           INSERT INTO Common.CurrencyRate (FromCurrencyId, ToCurrencyId, Rate, Timestamp)
                           VALUES (@fromId, @toId, @rate, @date)";

            var command = new CommandDefinition(sql, parameters);

            using (var db = new SqlConnection(connectionString))
            {
                return await db.ExecuteScalarAsync<bool>(command);
            }
        }
    }
}
