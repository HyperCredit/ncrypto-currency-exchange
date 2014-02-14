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
        public static AccountInfo ParseAccountInfo(JObject jObject)
        {
            throw new NotImplementedException();
        }

        public static DateTime ParseTime(int secondsSinceEpoch)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(secondsSinceEpoch);
        }

        public static MarketOrder ParseMarketDepth(JArray depthArray, OrderType orderType)
        {
            return new MarketOrder(orderType,
                depthArray.Value<decimal>(0), depthArray.Value<decimal>(1));
        }

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

        public static Dictionary<MarketId, Book> ParseMarketOrdersAlt(string quoteCurrencyCode,
            JObject altDepthJson)
        {
            Dictionary<MarketId, Book> marketOrders = new Dictionary<MarketId, Book>();

            // altDepthJson is structured as an object containing base currency
            // codes as keys, and book data as value.
            foreach (JProperty property in altDepthJson.Properties())
            {
                string baseCurrencyCode = property.Name;
                JObject bookJson = property.Value as JObject;

                if (null != bookJson)
                {
                    MarketId marketId = new VircurexMarketId(baseCurrencyCode, quoteCurrencyCode);
                    marketOrders[marketId] = ParseOrderBook(bookJson);
                }
            }

            return marketOrders;
        }

        public static List<MarketTrade> ParseMarketTrades(MarketId marketId, JArray tradesJson)
        {
            return tradesJson.Select(
                trade => ParseMarketTrade(marketId, (JObject)trade)
            ).ToList();
        }

        public static MarketTrade ParseMarketTrade(MarketId marketId, JObject trade)
        {
            return new MarketTrade(new VircurexTradeId(trade.Value<int>("tid")),
                ParseDateTime(trade.Value<int>("date")), trade.Value<decimal>("price"),
                trade.Value<decimal>("amount"), marketId);
        }

        public static DateTime ParseDateTime(int secondsSinceEpoch)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            return dateTime.AddSeconds(secondsSinceEpoch);
        }
    }
}
