using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Lostics.NCryptoExchange.CoinsE
{
    public class CoinsECurrency : Currency
    {
        public CoinsECurrency(string currencyCode, string label) : base(currencyCode, label)
        {
        }

        public static CoinsECurrency Parse(JObject coinJson)
        {
            return new CoinsECurrency(coinJson.Value<string>("coin"), coinJson.Value<string>("name"))
            {
                ConfirmationsRequired = coinJson.Value<int>("confirmations"),
                Status = coinJson.Value<string>("status"),
                Tier = coinJson.Value<int>("tier"),
                TradeFeePercent = coinJson.Value<decimal>("trade_fee"),
                WithdrawalFeeAbsolute = coinJson.Value<decimal>("withdrawal_fee")
            };
        }

        public int ConfirmationsRequired { get; private set; }
        public string Status { get; private set; }
        public int Tier { get; private set; }
        public decimal TradeFeePercent { get; private set; }
        public decimal WithdrawalFeeAbsolute { get; private set; }
    }
}
