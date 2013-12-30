using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Cryptsy
{
    class Parsers
    {
        internal static List<Market<CryptsyMarketId>> ParseMarkets(JArray marketsJson, TimeZoneInfo timeZone)
        {
            List<Market<CryptsyMarketId>> markets = new List<Market<CryptsyMarketId>>();

            foreach (JObject marketObj in marketsJson)
            {
                DateTime created = DateTime.Parse(marketObj["created"].ToString());

                TimeZoneInfo.ConvertTimeToUtc(created, timeZone);

                CryptsyMarket market = new CryptsyMarket(new CryptsyMarketId(marketObj["marketid"].ToString()),
                    marketObj["primary_currency_code"].ToString(), marketObj["primary_currency_name"].ToString(),
                    marketObj["secondary_currency_code"].ToString(), marketObj["secondary_currency_name"].ToString(),
                    marketObj["label"].ToString(),
                    Price.Parse(marketObj["current_volume"]), Price.Parse(marketObj["last_trade"]),
                    Price.Parse(marketObj["high_trade"]), Price.Parse(marketObj["low_trade"]),
                    created
                );

                markets.Add(market);
            }

            return markets;
        }

        internal static Book ParseMarketDepthBook(JObject bookJson, CryptsyMarketId marketId)
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

            List<MarketDepth> buy = ParseMarketDepth(buyArray, marketId);
            List<MarketDepth> sell = ParseMarketDepth(sellArray, marketId);

            return new Book(sell, buy);
        }

        internal static List<MarketDepth> ParseMarketDepth(JArray sideJson, CryptsyMarketId marketId)
        {
            List<MarketDepth> side = new List<MarketDepth>(sideJson.Count);

            foreach (JObject depthJson in sideJson)
            {
                side.Add(new MarketDepth(Price.Parse(depthJson["price"]),
                    Price.Parse(depthJson["quantity"])));
            }

            return side;
        }

        internal static List<MarketOrder> ParseMarketOrders(OrderType orderType, JArray jArray)
        {
            List<MarketOrder> orders = new List<MarketOrder>(jArray.Count);

            try
            {
                foreach (JObject jsonOrder in jArray)
                {
                    Price quantity = Price.Parse(jsonOrder["quantity"]);
                    Price price;

                    switch (orderType)
                    {
                        case OrderType.Buy:
                            price = Price.Parse(jsonOrder["buyprice"]);
                            break;
                        case OrderType.Sell:
                            price = Price.Parse(jsonOrder["sellprice"]);
                            break;
                        default:
                            throw new ArgumentException("Unknown order type \"" + orderType.ToString() + "\".");
                    }

                    orders.Add(new MarketOrder(orderType, price, quantity));
                }
            }
            catch (System.FormatException e)
            {
                throw new CryptsyResponseException("Encountered invalid quantity/price while parsing market orders.", e);
            }

            return orders;
        }

        internal static List<MarketTrade<CryptsyMarketId, CryptsyTradeId>> ParseMarketTrades(JArray jsonTrades, CryptsyMarketId defaultMarketId)
        {
            List<MarketTrade<CryptsyMarketId, CryptsyTradeId>> trades = new List<MarketTrade<CryptsyMarketId, CryptsyTradeId>>();

            foreach (JObject jsonTrade in jsonTrades)
            {
                // FIXME: Need to correct timezone on this
                DateTime tradeDateTime = DateTime.Parse(jsonTrade["datetime"].ToString());
                JToken marketIdToken = jsonTrade["marketid"];
                CryptsyMarketId marketId = null == marketIdToken
                    ? defaultMarketId
                    : CryptsyMarketId.Parse(marketIdToken);
                CryptsyTradeId tradeId = CryptsyTradeId.Parse(jsonTrade["tradeid"]);
                OrderType tradeType = (OrderType)Enum.Parse(typeof(OrderType), jsonTrade["tradetype"].ToString());
                trades.Add(new MarketTrade<CryptsyMarketId, CryptsyTradeId>(tradeId,
                    tradeType, tradeDateTime,
                    Price.Parse(jsonTrade["tradeprice"]),
                    Price.Parse(jsonTrade["quantity"]), Price.Parse(jsonTrade["fee"]),
                    marketId
                ));
            }

            return trades;
        }

        internal static List<MyOrder<CryptsyMarketId, CryptsyOrderId>> ParseMyOrders(JArray jsonOrders, CryptsyMarketId defaultMarketId)
        {
            List<MyOrder<CryptsyMarketId, CryptsyOrderId>> orders = new List<MyOrder<CryptsyMarketId, CryptsyOrderId>>();

            foreach (JObject jsonTrade in jsonOrders)
            {
                // FIXME: Need to correct timezone on this
                DateTime created = DateTime.Parse(jsonTrade["created"].ToString());
                JToken marketIdToken = jsonTrade["marketid"];
                CryptsyMarketId marketId = null == marketIdToken
                    ? defaultMarketId
                    : CryptsyMarketId.Parse(marketIdToken);
                CryptsyOrderId orderId = CryptsyOrderId.Parse(jsonTrade["orderid"]);
                OrderType orderType = (OrderType)Enum.Parse(typeof(OrderType), jsonTrade["ordertype"].ToString());
                orders.Add(new MyOrder<CryptsyMarketId, CryptsyOrderId>(orderId,
                    orderType, created,
                    Price.Parse(jsonTrade["price"]),
                    Price.Parse(jsonTrade["quantity"]), Price.Parse(jsonTrade["orig_quantity"]),
                    marketId
                ));
            }

            return orders;
        }

        internal static List<MyTrade<CryptsyMarketId, CryptsyOrderId, CryptsyTradeId>> ParseMyTrades(JArray jsonTrades, CryptsyMarketId defaultMarketId)
        {
            List<MyTrade<CryptsyMarketId, CryptsyOrderId, CryptsyTradeId>> trades = new List<MyTrade<CryptsyMarketId, CryptsyOrderId, CryptsyTradeId>>();

            foreach (JObject jsonTrade in jsonTrades)
            {
                // FIXME: Need to correct timezone on this
                DateTime tradeDateTime = DateTime.Parse(jsonTrade["datetime"].ToString());
                JToken marketIdToken = jsonTrade["marketid"];
                CryptsyMarketId marketId = null == marketIdToken
                    ? defaultMarketId
                    : CryptsyMarketId.Parse(marketIdToken);
                CryptsyOrderId orderId = CryptsyOrderId.Parse(jsonTrade["order_id"]);
                CryptsyTradeId tradeId = CryptsyTradeId.Parse(jsonTrade["tradeid"]);
                OrderType tradeType = (OrderType)Enum.Parse(typeof(OrderType), jsonTrade["tradetype"].ToString());
                trades.Add(new MyTrade<CryptsyMarketId, CryptsyOrderId, CryptsyTradeId>(tradeId,
                    tradeType, tradeDateTime,
                    Price.Parse(jsonTrade["tradeprice"]),
                    Price.Parse(jsonTrade["quantity"]), Price.Parse(jsonTrade["fee"]),
                    marketId, orderId
                ));
            }

            return trades;
        }

        internal static List<Transaction> ParseTransactions(JArray jsonTransactions)
        {
            List<Transaction> transactions = new List<Transaction>();

            foreach (JObject jsonTransaction in jsonTransactions)
            {
                TimeZoneInfo serverTimeZone = TimeZoneResolver.GetByShortCode(jsonTransaction["timezone"].ToString());
                DateTime transactionPosted = DateTime.Parse(jsonTransaction["datetime"].ToString());
                TransactionType transactionType = (TransactionType)Enum.Parse(typeof(TransactionType), jsonTransaction["type"].ToString());

                TimeZoneInfo.ConvertTimeToUtc(transactionPosted, serverTimeZone);

                Transaction transaction = new Transaction(jsonTransaction["currency"].ToString(),
                    transactionPosted, transactionType,
                    Address.Parse(jsonTransaction["address"]), Price.Parse(jsonTransaction["amount"]),
                    Price.Parse(jsonTransaction["fee"]));
                transactions.Add(transaction);
            }

            return transactions;
        }
    }
}
