using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.Kraken
{
    public class KrakenCurrency : Currency
    {
        public KrakenCurrency(string currencyCode, string label) : base(currencyCode, label)
        {

        }

        public static List<KrakenCurrency> Parse(JObject currenciesJson)
        {
            List<KrakenCurrency> currencies = new List<KrakenCurrency>();

            foreach (JProperty property in currenciesJson.Properties())
            {
                currencies.Add(KrakenCurrency.Parse(property.Name, (JObject)property.Value));
            }

            return currencies;
        }

        public static KrakenCurrency Parse(string baseCurrency, JObject currencyJson)
        {
            return new KrakenCurrency(baseCurrency, currencyJson.Value<string>("name"))
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
