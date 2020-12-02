using Dapper;
using System.Threading.Tasks;
using System;
using ProjectIvy.CurrencySync.Services.Fixer.Models;
using Microsoft.Data.SqlClient;

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

        public static async Task InsertOrUpdateExchangeRate(string connectionString, RatesOnDate ratesOnDate)
        {
            foreach (var rate in ratesOnDate.Rates)
            {
                var parameters = new
                {
                    from = ratesOnDate.Base,
                    to = rate.Key,
                    date = ratesOnDate.Date,
                    rate = rate.Value
                };

                string sql = @"DECLARE @fromId INT = (SELECT TOP 1 Id FROM Common.Currency WHERE Code = @from)
                               DECLARE @toId INT = (SELECT TOP 1 Id FROM Common.Currency WHERE Code = @to)

                               IF (@toId IS NULL)
                               BEGIN
                                INSERT INTO Common.Currency (ValueId, Symbol, Code, [Name]) VALUES (@to, @to, @to, @to)
                                SET @toId = SCOPE_IDENTITY()
                               END

                               IF (EXISTS(SELECT TOP 1 * FROM Common.CurrencyRate WHERE FromCurrencyId = @fromId AND ToCurrencyId = @toId AND Timestamp = @date))
                                UPDATE Common.CurrencyRate SET Rate = @rate WHERE FromCurrencyId = @fromId AND ToCurrencyId = @toId AND Timestamp = @date 
                               ELSE
                                INSERT INTO Common.CurrencyRate (FromCurrencyId, ToCurrencyId, Rate, Timestamp)
                                VALUES (@fromId, @toId, @rate, @date)";

                using (var sqlConnection = new SqlConnection(connectionString))
                {
                    await sqlConnection.ExecuteScalarAsync<bool>(sql, parameters);
                }
            }
        }
    }
}
