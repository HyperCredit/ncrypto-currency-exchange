using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.CoinEx
{
    public class CoinExCurrency : Currency
    {
        public CoinExCurrency(string currencyCode, string label) : base(currencyCode, label)
        {

        }

        public static CoinExCurrency Parse(JObject currencyJson)
        {
            return new CoinExCurrency(currencyJson.Value<string>("name"), currencyJson.Value<string>("desc"))
            {
                Id = currencyJson.Value<int>("id"),
                TransferFee = currencyJson.Value<decimal?>("tx_fee"),
                TransferConfirmations = currencyJson.Value<int?>("tx_conf"),
                UpdatedAt = currencyJson.Value<DateTime>("updated_at")
            };
        }

        public int Id { get; set; }
        public decimal? TransferFee { get; set; }
        public int? TransferConfirmations { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
