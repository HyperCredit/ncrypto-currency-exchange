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

        public static Book ParseMarketOrders(JObject bookJson)
        {
            JArray asksArray = bookJson.Value<JArray>("asks");
            JArray bidsArray = bookJson.Value<JArray>("bids");

            List<MarketOrder> asks = asksArray.Select(
                depth => (MarketOrder)ParseMarketDepth(depth as JArray, OrderType.Sell)
            ).ToList();
            List<MarketOrder> bids = bidsArray.Select(
                depth => (MarketOrder)ParseMarketDepth(depth as JArray, OrderType.Buy)
            ).ToList();

            return new Book(asks, bids);
        }

        private static MarketOrder ParseMarketDepth(JArray depthArray, OrderType orderType)
        {
            return new MarketOrder(orderType,
                depthArray.Value<decimal>(0), depthArray.Value<decimal>(1));
        }
    }
}
