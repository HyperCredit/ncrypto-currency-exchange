using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Bter
{
    public static class BterParsers
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

        public static List<MarketTrade> ParseMarketTrades(MarketId marketId, JObject tradesJson)
        {
            return tradesJson.Value<JArray>("data").Select(
                trade => ParseMarketTrade(marketId, (JObject)trade)
            ).ToList();
        }

        public static MarketTrade ParseMarketTrade(MarketId marketId, JObject trade)
        {
            return new MarketTrade(new BterTradeId(trade.Value<int>("tid")),
                ParseDateTime(trade.Value<int>("date")), trade.Value<decimal>("price"),
                trade.Value<decimal>("amount"), marketId);
        }

        private static DateTime ParseDateTime(int secondsSinceEpoch)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            return dateTime.AddSeconds(secondsSinceEpoch);
        }
    }
}
