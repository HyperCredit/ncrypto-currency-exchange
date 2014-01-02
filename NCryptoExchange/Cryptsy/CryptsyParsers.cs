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
        public static Book ParseMarketDepthBook(JObject bookJson, CryptsyMarketId marketId)
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

        public static List<MarketOrder> ParseMarketDepth(OrderType orderType,
            JArray sideJson, CryptsyMarketId marketId)
        {
            List<MarketOrder> side = new List<MarketOrder>(sideJson.Count);

            foreach (JArray depthJson in sideJson)
            {
                side.Add(new MarketOrder(orderType, depthJson[0].Value<decimal>(),
                    depthJson[1].Value<decimal>()));
            }

            return side;
        }

        public static Book ParseMarketOrders(JObject marketOrdersJson)
        {
            List<MarketOrder> buyOrders = marketOrdersJson.Value<JArray>("buyorders").Select(marketOrder => (MarketOrder)CryptsyMarketOrder.ParseBuy(marketOrder as JObject)).ToList();
            List<MarketOrder> sellOrders = marketOrdersJson.Value<JArray>("sellorders").Select(marketOrder => (MarketOrder)CryptsyMarketOrder.ParseSell(marketOrder as JObject)).ToList();

            return new Book(sellOrders, buyOrders);
        }

        public static List<MyOrder<CryptsyMarketId, CryptsyOrderId>> ParseMyOrders(JArray jsonOrders,
            CryptsyMarketId defaultMarketId, TimeZoneInfo timeZone)
        {
            List<MyOrder<CryptsyMarketId, CryptsyOrderId>> orders = new List<MyOrder<CryptsyMarketId, CryptsyOrderId>>();

            foreach (JObject jsonTrade in jsonOrders)
            {
                DateTime created = DateTime.Parse(jsonTrade.Value<string>("created"));
                JToken marketIdToken = jsonTrade["marketid"];
                CryptsyMarketId marketId = null == marketIdToken
                    ? defaultMarketId
                    : CryptsyMarketId.Parse(marketIdToken);
                CryptsyOrderId orderId = CryptsyOrderId.Parse(jsonTrade["orderid"]);
                OrderType orderType = (OrderType)Enum.Parse(typeof(OrderType), jsonTrade.Value<string>("ordertype"));

                created = TimeZoneInfo.ConvertTimeToUtc(created, timeZone);

                orders.Add(new MyOrder<CryptsyMarketId, CryptsyOrderId>(orderId,
                    orderType, created,
                    jsonTrade.Value<decimal>("price"),
                    jsonTrade.Value<decimal>("quantity"), jsonTrade.Value<decimal>("orig_quantity"),
                    marketId
                ));
            }

            return orders;
        }

        public static List<MyTrade<CryptsyMarketId, CryptsyOrderId>> ParseMyTrades(JArray jsonTrades,
            CryptsyMarketId defaultMarketId, TimeZoneInfo timeZone)
        {
            List<MyTrade<CryptsyMarketId, CryptsyOrderId>> trades = new List<MyTrade<CryptsyMarketId, CryptsyOrderId>>();

            foreach (JObject jsonTrade in jsonTrades)
            {
                DateTime tradeDateTime = DateTime.Parse(jsonTrade.Value<string>("datetime"));
                JToken marketIdToken = jsonTrade["marketid"];
                CryptsyMarketId marketId = null == marketIdToken
                    ? defaultMarketId
                    : CryptsyMarketId.Parse(marketIdToken);
                CryptsyOrderId orderId = CryptsyOrderId.Parse(jsonTrade["order_id"]);
                CryptsyTradeId tradeId = CryptsyTradeId.Parse(jsonTrade["tradeid"]);
                OrderType tradeType = (OrderType)Enum.Parse(typeof(OrderType), jsonTrade.Value<string>("tradetype"));

                tradeDateTime = TimeZoneInfo.ConvertTimeToUtc(tradeDateTime, timeZone);

                trades.Add(new MyTrade<CryptsyMarketId, CryptsyOrderId>(tradeId,
                    tradeType, tradeDateTime,
                    jsonTrade.Value<decimal>("tradeprice"),
                    jsonTrade.Value<decimal>("quantity"), jsonTrade.Value<decimal>("fee"),
                    marketId, orderId
                ));
            }

            return trades;
        }

        public static List<Transaction> ParseTransactions(JArray jsonTransactions)
        {
            List<Transaction> transactions = new List<Transaction>();

            foreach (JObject jsonTransaction in jsonTransactions)
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

            return transactions;
        }
    }
}
