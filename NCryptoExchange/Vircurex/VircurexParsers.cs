using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Vircurex
{
    public static class VircurexParsers
    {
        public static DateTime ParseTime(int secondsSinceEpoch)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(secondsSinceEpoch);
        }

        internal static MarketOrder ParseMarketDepth(JArray depthArray, OrderType orderType)
        {
            return new MarketOrder(orderType,
                depthArray.Value<decimal>(0), depthArray.Value<decimal>(1));
        }

        public static Book ParseMarketOrders(JObject bookJson)
        {
            JArray asksArray = bookJson.Value<JArray>("asks");
            JArray bidsArray = bookJson.Value<JArray>("bids");

            List<MarketOrder> asks = asksArray.Select(
                depth => (MarketOrder)ParseMarketDepth((JArray)depth, OrderType.Sell)
            ).ToList();
            List<MarketOrder> bids = bidsArray.Select(
                depth => (MarketOrder)ParseMarketDepth((JArray)depth, OrderType.Buy)
            ).ToList();

            return new Book(asks, bids);
        }

        public static Dictionary<MarketId, Book> ParseMarketOrdersAlt(string quoteCurrencyCode,
            JObject altDepthJson)
        {
            Dictionary<MarketId, Book> marketOrders = new Dictionary<MarketId, Book>();

            // altDepthJson is structured as an object containing base currency
            // codes as keys, and book data as value.
            foreach (JProperty property in altDepthJson.Properties())
            {
                string baseCurrencyCode = property.Name;
                MarketId marketId = new VircurexMarketId(baseCurrencyCode, quoteCurrencyCode);
                marketOrders[marketId] = ParseMarketOrders((JObject)property.Value);
            }

            return marketOrders;
        }
    }
}
