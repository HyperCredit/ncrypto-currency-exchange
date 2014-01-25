using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Lostics.NCryptoExchange.CoinEx
{
    public static class CoinExParsers
    {
        public static Book ParseOrderBook(JObject bookJson)
        {
            JArray asksArray = bookJson.Value<JArray>("asks");
            JArray bidsArray = bookJson.Value<JArray>("bids");

            List<MarketDepth> asks = asksArray.Select(
                depth => (MarketDepth)ParseMarketDepth((JArray)depth, OrderType.Sell)
            ).ToList();
            List<MarketDepth> bids = bidsArray.Select(
                depth => (MarketDepth)ParseMarketDepth((JArray)depth, OrderType.Buy)
            ).ToList();

            return new Book(asks, bids);
        }

        internal static MarketOrder ParseMarketDepth(JArray depthArray, OrderType orderType)
        {
            return new MarketOrder(orderType,
                depthArray.Value<decimal>(0), depthArray.Value<decimal>(1));
        }
    }
}
