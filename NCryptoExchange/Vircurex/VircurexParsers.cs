using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Vircurex
{
    public static class VircurexParsers
    {
        public static AccountInfo ParseAccountInfo(JObject walletsJson)
        {
            List<Wallet> wallets = new List<Wallet>();
            JObject balances = walletsJson.Value<JObject>("balances");

            foreach (JProperty currency in balances.Properties())
            {
                JObject walletJson = balances.Value<JObject>(currency.Name);
                decimal availableBalance = walletJson.Value<decimal>("availablebalance");
                decimal balance = walletJson.Value<decimal>("balance");
                decimal heldBalance = balance - availableBalance;
                string currencyCode = currency.Name;

                wallets.Add(new Wallet(currencyCode, balance, heldBalance));
            }

            return new AccountInfo(wallets);
        }

        public static DateTime ParseDateTime(int secondsSinceEpoch)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            return dateTime.AddSeconds(secondsSinceEpoch);
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

        public static List<MyOrder> ParseMyActiveOrders(JObject responseJson)
        {
            int numberOrders = responseJson.Value<int>("numberorders");
            List<MyOrder> orders = new List<MyOrder>(numberOrders);

            // Parses an order such as:
            // "orderid": 3670301,
            // "ordertype": "BUY",
            // "quantity": "19.87",
            // "openquantity": "18.79",
            // "currency1": "VTC",
            // "unitprice": "0.00363",
            // "currency2": "BTC",
            // "lastchangedat": "2014-01-13T22:54:30+00:00",
            // "releasedat": "2014-01-13T22:41:46+00:00"

            for (int orderIdx = 1; orderIdx <= numberOrders; orderIdx++)
            {
                JObject orderJson = responseJson.Value<JObject>("order-" + orderIdx);
                DateTime created = orderJson.Value<DateTime>("releasedat"); // TODO: Handle unreleased orders?
                VircurexOrderId orderId = new VircurexOrderId(orderJson.Value<int>("orderid"));
                VircurexMarketId marketId = new VircurexMarketId(orderJson.Value<string>("currency1"),
                    orderJson.Value<string>("currency2"));
                OrderType orderType
                    = orderJson.Value<string>("ordertype").Equals(Enum.GetName(typeof(OrderType), OrderType.Buy).ToUpper())
                    ? OrderType.Buy
                    : OrderType.Sell;

                orders.Add(new MyOrder(orderId, orderType, created,
                    orderJson.Value<decimal>("unitprice"), orderJson.Value<decimal>("openquantity"),
                    orderJson.Value<decimal>("quantity"), marketId));
            }
            

            return orders;
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

        public static List<MyTrade> ParseOrderExecutions(JObject responseJson)
        {
            int numberOrderExecutions = responseJson.Value<int>("numberorderexecutions");
            VircurexOrderId orderId = new VircurexOrderId(responseJson.Value<int>("orderid"));
            List<MyTrade> trades = new List<MyTrade>(numberOrderExecutions);

            // Parses a trade such as:
            // "currency1": "DOGE",
            // "currency2": "BTC",
            // "quantity": "57796.176",
            // "unitprice": "0.0000025",
            // "feepaid": "115.592352",
            // "ordertype": "BUY",
            // "executed_at": "2014-02-14T15:17:08+00:00"

            for (int orderExecutionIdx = 1; orderExecutionIdx <= numberOrderExecutions; orderExecutionIdx++)
            {
                JObject orderExecutionJson = responseJson.Value<JObject>("orderexecution-" + orderExecutionIdx);
                DateTime executed = orderExecutionJson.Value<DateTime>("executed_at");
                VircurexMarketId marketId = new VircurexMarketId(orderExecutionJson.Value<string>("currency1"),
                    orderExecutionJson.Value<string>("currency2"));
                OrderType orderType
                    = orderExecutionJson.Value<string>("ordertype").Equals(Enum.GetName(typeof(OrderType), OrderType.Buy).ToUpper())
                    ? OrderType.Buy
                    : OrderType.Sell;

                trades.Add(new MyTrade(new VircurexFakeTradeId(orderId, orderExecutionIdx), orderType,
                    executed, orderExecutionJson.Value<decimal>("unitprice"), orderExecutionJson.Value<decimal>("feepaid"),
                    orderExecutionJson.Value<decimal>("quantity"), marketId, orderId));
            }

            return trades;
        }
    }
}
