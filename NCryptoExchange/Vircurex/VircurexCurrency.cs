using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
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

        public static List<VircurexCurrency> Parse(JObject currenciesJson)
        {
            List<VircurexCurrency> currencies = new List<VircurexCurrency>();

            foreach (JProperty property in currenciesJson.Properties())
            {
                currencies.Add(VircurexCurrency.Parse(property.Name, property.Value as JObject));
            }

            return currencies;
        }

        public static VircurexCurrency Parse(string baseCurrency, JObject currencyJson)
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
