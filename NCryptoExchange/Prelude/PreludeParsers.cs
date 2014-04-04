using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Prelude
{
    public static class PreludeParsers
    {
        public static DateTime ParseTime(int secondsSinceEpoch)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(secondsSinceEpoch);
        }

        internal static DateTime ParseDateTime(int secondsSinceEpoch)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            return dateTime.AddSeconds(secondsSinceEpoch);
        }

        public static Book ParseOrderBook(JObject bookJson)
        {
            JArray asksArray = bookJson.Value<JArray>("sell");
            JArray bidsArray = bookJson.Value<JArray>("buy");

            List<MarketDepth> asks = asksArray.Select(
                depth => (MarketDepth)ParseMarketDepth((JObject)depth, OrderType.Sell)
            ).ToList();
            List<MarketDepth> bids = bidsArray.Select(
                depth => (MarketDepth)ParseMarketDepth((JObject)depth, OrderType.Buy)
            ).ToList();

            return new Book(asks, bids);
        }

        internal static MarketOrder ParseMarketDepth(JObject depth, OrderType orderType)
        {
            return new MarketOrder(orderType,
                depth.Value<decimal>("rate"), depth.Value<decimal>("remaining"));
        }
    }
}
