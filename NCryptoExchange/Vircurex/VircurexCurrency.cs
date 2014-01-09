using Lostics.NCryptoExchange.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.Vircurex
{
    public class VircurexCurrency : Currency
    {
        public VircurexCurrency(string currencyCode, string label) : base(currencyCode, label)
        {

        }

        internal static VircurexCurrency Parse(string baseCurrency, Newtonsoft.Json.Linq.JObject currencyJson)
        {
            return new VircurexCurrency(baseCurrency, currencyJson.Value<string>("name"))
            {
                Confirmations = currencyJson.Value<int>("confirmations"),
                WithdrawlFee = currencyJson.Value<decimal>("withdrawal_fee"),
                MaxDailyWithdrawl = currencyJson.Value<decimal>("max_daily_withdrawal")
            };
        }

        public int Confirmations { get; private set; }
        public decimal MaxDailyWithdrawl { get; private set; }
        public decimal WithdrawlFee { get; private set; }
    }
}
