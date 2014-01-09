using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Cryptsy
{
    /// <summary>
    /// Methods for parsing classes where there's no Cryptsy-specific subclass. In other cases, parsers
    /// are static methods in the relevant classes.
    /// </summary>
    public class CryptsyParsers
    {
        public static Book ParseMarketDepthBook(JObject bookJson, MarketId marketId)
        {
            JToken buyJson = bookJson["buy"];
            JToken sellJson = bookJson["sell"];

            if (buyJson.Type != JTokenType.Array)
            {
                throw new CryptsyResponseException("Expected array for buy-side market depth, found \""
                    + Enum.GetName(typeof(JTokenType), buyJson.Type) + "\".");
            }

            if (sellJson.Type != JTokenType.Array)
            {
                throw new CryptsyResponseException("Expected array for sell-side market depth, found \""
                    + Enum.GetName(typeof(JTokenType), sellJson.Type) + "\".");
            }

            JArray buyArray = (JArray)buyJson;
            JArray sellArray = (JArray)sellJson;

            List<MarketOrder> buy = buyArray.Select(
                depth => (MarketOrder)CryptsyMarketOrder.ParseMarketDepth(depth as JArray, OrderType.Buy)
            ).ToList();
            List<MarketOrder> sell = sellArray.Select(
                depth => (MarketOrder)CryptsyMarketOrder.ParseMarketDepth(depth as JArray, OrderType.Sell)
            ).ToList();

            return new Book(sell, buy);
        }

        public static Book ParseMarketOrders(JObject marketOrdersJson)
        {
            List<MarketOrder> buyOrders = marketOrdersJson.Value<JArray>("buyorders").Select(marketOrder => (MarketOrder)CryptsyMarketOrder.ParseBuy(marketOrder as JObject)).ToList();
            List<MarketOrder> sellOrders = marketOrdersJson.Value<JArray>("sellorders").Select(marketOrder => (MarketOrder)CryptsyMarketOrder.ParseSell(marketOrder as JObject)).ToList();

            return new Book(sellOrders, buyOrders);
        }

        public static MyOrder ParseMyOrder(JObject myOrderJson, MarketId marketId, TimeZoneInfo timeZone)
        {
            DateTime created = DateTime.Parse(myOrderJson.Value<string>("created"));
            CryptsyOrderId orderId = CryptsyOrderId.Parse(myOrderJson["orderid"]);
            OrderType orderType = (OrderType)Enum.Parse(typeof(OrderType), myOrderJson.Value<string>("ordertype"));

            created = TimeZoneInfo.ConvertTimeToUtc(created, timeZone);

            return new MyOrder(orderId,
                orderType, created,
                myOrderJson.Value<decimal>("price"),
                myOrderJson.Value<decimal>("quantity"), myOrderJson.Value<decimal>("orig_quantity"),
                marketId
            );
        }

        public static MyOrder ParseMyOrder(JObject myOrderJson, TimeZoneInfo timeZone)
        {
            DateTime created = DateTime.Parse(myOrderJson.Value<string>("created"));
            CryptsyMarketId marketId = CryptsyMarketId.Parse(myOrderJson["marketid"]);
            CryptsyOrderId orderId = CryptsyOrderId.Parse(myOrderJson["orderid"]);
            OrderType orderType = (OrderType)Enum.Parse(typeof(OrderType), myOrderJson.Value<string>("ordertype"));

            created = TimeZoneInfo.ConvertTimeToUtc(created, timeZone);

            return new MyOrder(orderId,
                orderType, created,
                myOrderJson.Value<decimal>("price"),
                myOrderJson.Value<decimal>("quantity"), myOrderJson.Value<decimal>("orig_quantity"),
                marketId
            );
        }

        public static MyTrade ParseMyTrade(JObject jsonTrade,
            MarketId defaultMarketId, TimeZoneInfo timeZone)
        {
            DateTime tradeDateTime = DateTime.Parse(jsonTrade.Value<string>("datetime"));
            JToken marketIdToken = jsonTrade["marketid"];
            MarketId marketId = null == marketIdToken
                ? defaultMarketId
                : CryptsyMarketId.Parse(marketIdToken);
            CryptsyOrderId orderId = CryptsyOrderId.Parse(jsonTrade["order_id"]);
            CryptsyTradeId tradeId = CryptsyTradeId.Parse(jsonTrade["tradeid"]);
            OrderType tradeType = (OrderType)Enum.Parse(typeof(OrderType), jsonTrade.Value<string>("tradetype"));

            tradeDateTime = TimeZoneInfo.ConvertTimeToUtc(tradeDateTime, timeZone);

            return new MyTrade(tradeId,
                tradeType, tradeDateTime,
                jsonTrade.Value<decimal>("tradeprice"), jsonTrade.Value<decimal>("fee"),
                jsonTrade.Value<decimal>("quantity"),
                marketId, orderId
            );
        }

        public static List<Transaction> ParseTransactions(JArray jsonTransactions)
        {
            List<Transaction> transactions = new List<Transaction>();

            foreach (JObject jsonTransaction in jsonTransactions)
            {
                ParseTransaction(transactions, jsonTransaction);
            }

            return transactions;
        }

        public static void ParseTransaction(List<Transaction> transactions, JObject jsonTransaction)
        {
            TimeZoneInfo serverTimeZone = TimeZoneResolver.GetByShortCode(jsonTransaction.Value<string>("timezone"));
            DateTime transactionPosted = DateTime.Parse(jsonTransaction.Value<string>("datetime"));
            TransactionType transactionType = (TransactionType)Enum.Parse(typeof(TransactionType), jsonTransaction.Value<string>("type"));

            transactionPosted = TimeZoneInfo.ConvertTimeToUtc(transactionPosted, serverTimeZone);

            Transaction transaction = new Transaction(jsonTransaction.Value<string>("currency"),
                transactionPosted, transactionType,
                Address.Parse(jsonTransaction["address"]), jsonTransaction.Value<decimal>("amount"),
                jsonTransaction.Value<decimal>("fee"));
            transactions.Add(transaction);
        }
    }
}
