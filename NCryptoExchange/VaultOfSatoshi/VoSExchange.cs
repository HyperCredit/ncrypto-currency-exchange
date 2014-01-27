﻿using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.VaultOfSatoshi
{
    public class VoSExchange : AbstractExchange, ICoinDataSource<VoSCurrency>
    {
        public const decimal PRICE_UNIT = 0.00000001m;
        public const string DEFAULT_BASE_URL = "https://coinex.pw/api/v2/";

        private HttpClient client = new HttpClient();

        public VoSExchange()
        {
        }

        public static string BuildPublicUrl(Method method)
        {
            return DEFAULT_BASE_URL + Enum.GetName(typeof(Method), method);
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="method">The method to call on the VoS API</param>
        /// <returns>The raw JSON returned from VoS</returns>
        private async Task<T> CallPublic<T>(Method method)
            where T : JToken
        {
            return (T)JToken.Parse(await CallPublic(BuildPublicUrl(method)));
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="method">The method to call on the VoS API</param>
        /// <param name="quoteCurrencyCode">A quote currency code to append to the URL</param>
        /// <returns>The raw JSON returned from VoS</returns>
        private async Task<T> CallPublic<T>(Method method, VoSMarketId marketId)
            where T : JToken
        {
            StringBuilder url = new StringBuilder(BuildPublicUrl(method));

            url.Append("?tradePair=")
                .Append(marketId.Value.ToString());

            return (T)JToken.Parse(await CallPublic(url.ToString()));
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="url">Endpoint to make a request to</param>
        /// <returns>The raw JSON returned from VoS</returns>
        private async Task<string> CallPublic(string url)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                using (Stream jsonStream = await response.Content.ReadAsStreamAsync())
                {
                    using (StreamReader jsonStreamReader = new StreamReader(jsonStream))
                    {
                        return await jsonStreamReader.ReadToEndAsync();
                    }
                }
            }
            catch (ArgumentException e)
            {
                throw new VoSResponseException("Could not parse response from VoS.", e);
            }
        }

        /// <summary>
        /// Convert the open order list into a more conventional order book.
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public static Book ConsolidateOpenOrders(IEnumerable<VoSMarketOrder> orders)
        {
            Dictionary<decimal, decimal> bidSide = new Dictionary<decimal,decimal>();
            Dictionary<decimal, decimal> askSide = new Dictionary<decimal,decimal>();

            foreach (VoSMarketOrder order in orders) {
                decimal price = order.Price;
                decimal quantity = 0;

                switch (order.OrderType)
                {
                    case OrderType.Buy:
                        if (!bidSide.TryGetValue(price, out quantity))
                        {
                            quantity = 0;
                        }
                        quantity += order.Quantity;
                        bidSide[price] = quantity;
                        break;
                    case OrderType.Sell:
                        if (!askSide.TryGetValue(price, out quantity))
                        {
                            quantity = 0;
                        }
                        quantity += order.Quantity;
                        askSide[price] = quantity;
                        break;
                }
            }

            List<MarketDepth> bids = MarketDepth.DictionaryToList(bidSide);
            List<MarketDepth> asks = MarketDepth.DictionaryToList(askSide);
            
            // Flip the asks to put lowest first
            asks.Reverse();


            return new Book(asks,
                bids);
        }

        public override void Dispose()
        {
            client.Dispose();
        }

        /// <summary>
        /// Formats a date and time in a manner suitable for use with VoS.
        /// </summary>
        /// <param name="dateTimeUtc">A date and time, must be in the UTC timezone.</param>
        /// <returns>A formatted string</returns>
        public string FormatDateTime(DateTime dateTimeUtc)
        {
            return dateTimeUtc.ToString("s");
        }

        public override async Task<AccountInfo> GetAccountInfo()
        {
            throw new NotImplementedException();
        }

        public async Task<List<VoSCurrency>> GetCoins()
        {
            JArray coinsJson = (await CallPublic<JObject>(Method.currencies)).Value<JArray>("currencies");

            return coinsJson.Select(coin => VoSCurrency.Parse(coin as JObject)).ToList();
        }

        public override async Task<List<Market>> GetMarkets()
        {
            JObject jsonObj = await CallPublic<JObject>(Method.trade_pairs);

            JArray marketsJson = jsonObj.Value<JArray>("trade_pairs");
            return marketsJson.Select(
                market => (Market)VoSMarket.Parse((JObject)market)
            ).ToList();
        }

        public override async Task<List<MarketTrade>> GetMarketTrades(MarketId marketId)
        {
            VoSMarketId VoSMarketId = (VoSMarketId)marketId;
            JObject jsonObj = await CallPublic<JObject>(Method.trades, VoSMarketId);

            return jsonObj.Value<JArray>("trades").Select(
                marketTrade => (MarketTrade)VoSMarketTrade.Parse(marketId, (JObject)marketTrade)
            ).ToList();
        }

        public override async Task<List<MyTrade>> GetMyTrades(MarketId marketId, int? limit)
        {
            throw new NotImplementedException();
        }

        public override async Task<List<MyTrade>> GetAllMyTrades(int? limit)
        {
            throw new NotImplementedException();
        }

        public override async Task<List<MyOrder>> GetMyActiveOrders(MarketId marketId, int? limit)
        {
            throw new NotImplementedException();
        }

        public override async Task<Book> GetMarketDepth(MarketId marketId)
        {
            List<VoSMarketOrder> openOrders = await GetMarketOpenOrders(marketId);
            return ConsolidateOpenOrders(openOrders.ToArray());
        }

        public async Task<List<VoSMarketOrder>> GetMarketOpenOrders(MarketId marketId)
        {
            VoSMarketId VoSMarketId = (VoSMarketId)marketId;
            JObject jsonObj = await CallPublic<JObject>(Method.orders, VoSMarketId);

            return jsonObj.Value<JArray>("orders")
                .Select(order => VoSMarketOrder.Parse((JObject)order)).ToList();
        }

        public override async Task CancelOrder(OrderId orderId)
        {
            throw new NotImplementedException();
        }

        public override async Task CancelMarketOrders(MarketId marketId)
        {
            throw new NotImplementedException();
        }

        public override async Task<OrderId> CreateOrder(MarketId marketId, OrderType orderType, decimal quantity, decimal price)
        {
            throw new NotImplementedException();
        }

        public override string GetNextNonce()
        {
            return DateTime.Now.Ticks.ToString();
        }

        public override string Label
        {
            get { return "VoS"; }
        }

        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }

        public enum Method
        {
            balances,
            currencies,
            orders,
            trade_pairs,
            trades
        }
    }
}