using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Lostics.NCryptoExchange.VaultOfSatoshi
{
    public static class VoSParsers
    {
        /// <summary>
        /// Parse market depth data (a paired list of bids and asks)
        /// </summary>
        /// <param name="depthJson"></param>
        /// <returns></returns>
        public static Book ParseOrderBook(JObject depthJson)
        {
            List<MarketDepth> asks = depthJson.Value<JArray>("asks").Select(depth => ParseMarketDepthEntry(depth)).ToList();
            List<MarketDepth> bids = depthJson.Value<JArray>("bids").Select(depth => ParseMarketDepthEntry(depth)).ToList();

            return new Book(asks, bids);
        }

        private static MarketDepth ParseMarketDepthEntry(JToken depth)
        {
            decimal quantity = ParseCurrencyObject(depth.Value<JObject>("quantity"));
            decimal price = ParseCurrencyObject(depth.Value<JObject>("price"));

            return new MarketDepth(price, quantity);
        }

        private static decimal ParseCurrencyObject(JObject currencyJson)
        {
            return currencyJson.Value<decimal>("value");
        }
    }
}
