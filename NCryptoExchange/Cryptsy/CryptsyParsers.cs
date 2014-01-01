using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public class CryptsyParsers
    {
        public static CryptsyAccountInfo ParseAccountInfo(JObject returnObj)
        {
            TimeZoneInfo serverTimeZone = TimeZoneResolver.GetByShortCode(returnObj.Value<string>("servertimezone"));
            DateTime serverDateTime = DateTime.Parse(returnObj.Value<string>("serverdatetime"));

            serverDateTime = TimeZoneInfo.ConvertTimeToUtc(serverDateTime, serverTimeZone);

            return new CryptsyAccountInfo(
                ParseWallets(returnObj.Value<JObject>("balances_available"), returnObj.Value<JObject>("balances_hold")),
                serverDateTime, serverTimeZone,
                returnObj.Value<int>("openordercount")
            );
        }

        public static List<Market<CryptsyMarketId>> ParseMarkets(JArray marketsJson, TimeZoneInfo timeZone)
        {
            List<Market<CryptsyMarketId>> markets = new List<Market<CryptsyMarketId>>();

            foreach (JObject marketObj in marketsJson)
            {
                DateTime created = DateTime.Parse(marketObj.Value<string>("created"));

                TimeZoneInfo.ConvertTimeToUtc(created, timeZone);

                CryptsyMarket market = new CryptsyMarket(new CryptsyMarketId(marketObj.Value<string>("marketid")),
                    marketObj.Value<string>("primary_currency_code"), marketObj.Value<string>("primary_currency_name"),
                    marketObj.Value<string>("secondary_currency_code"), marketObj.Value<string>("secondary_currency_name"),
                    marketObj.Value<string>("label"),
                    marketObj.Value<decimal>("current_volume"), marketObj.Value<decimal>("last_trade"),
                    marketObj.Value<decimal>("high_trade"), marketObj.Value<decimal>("low_trade"),
                    created
                );

                markets.Add(market);
            }

            return markets;
        }

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

            List<MarketDepth> buy = ParseMarketDepth(buyArray, marketId);
            List<MarketDepth> sell = ParseMarketDepth(sellArray, marketId);

            return new Book(sell, buy);
        }

        public static List<MarketDepth> ParseMarketDepth(JArray sideJson, CryptsyMarketId marketId)
        {
            List<MarketDepth> side = new List<MarketDepth>(sideJson.Count);

            foreach (JArray depthJson in sideJson)
            {
                side.Add(new MarketDepth(depthJson[0].Value<decimal>(),
                    depthJson[1].Value<decimal>()));
            }

            return side;
        }

        public static MarketOrders ParseMarketOrders(JObject returnObj)
        {
            List<MarketOrder> buyOrders = CryptsyParsers.ParseMarketOrders(OrderType.Buy, returnObj.Value<JArray>("buyorders"));
            List<MarketOrder> sellOrders = CryptsyParsers.ParseMarketOrders(OrderType.Sell, returnObj.Value<JArray>("sellorders"));

            return new MarketOrders(sellOrders, buyOrders);
        }

        public static List<MarketOrder> ParseMarketOrders(OrderType orderType, JArray jArray)
        {
            List<MarketOrder> orders = new List<MarketOrder>(jArray.Count);

            try
            {
                foreach (JObject jsonOrder in jArray)
                {
                    decimal quantity = jsonOrder.Value<decimal>("quantity");
                    decimal price;

                    switch (orderType)
                    {
                        case OrderType.Buy:
                            price = jsonOrder.Value<decimal>("buyprice");
                            break;
                        case OrderType.Sell:
                            price = jsonOrder.Value<decimal>("sellprice");
                            break;
                        default:
                            throw new ArgumentException("Unknown order type \""
                                + Enum.GetName(typeof(OrderType), orderType) + "\".");
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

        public static List<MarketTrade<CryptsyMarketId>> ParseMarketTrades(JArray jsonTrades,
            CryptsyMarketId defaultMarketId, TimeZoneInfo timeZone)
        {
            List<MarketTrade<CryptsyMarketId>> trades = new List<MarketTrade<CryptsyMarketId>>();

            foreach (JObject jsonTrade in jsonTrades)
            {
                DateTime tradeDateTime = DateTime.Parse(jsonTrade.Value<string>("datetime"));
                JToken marketIdToken = jsonTrade["marketid"];
                CryptsyMarketId marketId = null == marketIdToken
                    ? defaultMarketId
                    : CryptsyMarketId.Parse(marketIdToken);
                CryptsyTradeId tradeId = CryptsyTradeId.Parse(jsonTrade["tradeid"]);
                OrderType orderType = (OrderType)Enum.Parse(typeof(OrderType), jsonTrade.Value<string>("initiate_ordertype"));

                tradeDateTime = TimeZoneInfo.ConvertTimeToUtc(tradeDateTime, timeZone);

                trades.Add(new MarketTrade<CryptsyMarketId>(tradeId,
                    orderType, tradeDateTime,
                    jsonTrade.Value<decimal>("tradeprice"),
                    jsonTrade.Value<decimal>("quantity"), jsonTrade.Value<decimal>("fee"),
                    marketId
                ));
            }

            return trades;
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

        public static List<Wallet> ParseWallets(JObject balancesAvailable, JObject balancesHold)
        {
            List<Wallet> wallets = new List<Wallet>();
            foreach (JProperty balanceAvailable in balancesAvailable.Properties())
            {
                wallets.Add(new Wallet(balanceAvailable.Name,
                    balancesAvailable.Value<decimal>(balanceAvailable.Name),
                    balancesHold.Value<decimal>(balanceAvailable.Name)));
            }
            return wallets;
        }
    }
}
