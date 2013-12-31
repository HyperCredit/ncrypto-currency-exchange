﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.IO;

namespace Lostics.NCryptoExchange.CoinsE
{
    public class CoinsEExchange : AbstractExchange<CoinsEMarketId, CoinsEOrderId, CoinsETradeId, Wallet>
    {
        public const string MARKETS_LIST = "https://www.coins-e.com/api/v2/markets/list/";

        private HttpClient client = new HttpClient();

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="url">Endpoint to make a request to</param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private async Task<T> CallPublic<T>(string url, string propertyName)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            JObject jsonObj;

            try
            {
                jsonObj = await GetJsonFromResponse(response);
            }
            catch (ArgumentException e)
            {
                throw new CoinsEResponseException("Could not parse response from Coins-E.", e);
            }

            string status = jsonObj.Value<string>("status");
            if (null == status)
            {
                throw new CoinsEResponseException("Response from Coins-E did not include a \"success\" property.");
            }
            if (!status.Equals("true"))
            {
                string message = jsonObj.Value<string>("message");

                if (null != message)
                {
                    throw new CoinsEResponseException(message);
                }
                else
                {
                    throw new CoinsEResponseException("Success status from Coins-E was \""
                        + status + "\", expected \"true\".");
                }
            }

            return jsonObj.Value<T>(propertyName);
        }

        public override void Dispose()
        {
            client.Dispose();
        }

        public override Task<Model.AccountInfo<Wallet>> GetAccountInfo()
        {
            throw new NotImplementedException();
        }

        public override async Task<List<Model.Market<CoinsEMarketId>>> GetMarkets()
        {
            return CoinsEParsers.ParseMarkets(await CallPublic<JArray>(MARKETS_LIST, "markets"));
        }

        public override Task<List<Model.Transaction>> GetMyTransactions()
        {
            throw new NotImplementedException();
        }

        public override Task<MarketOrders> GetMarketOrders(CoinsEMarketId marketId)
        {
            throw new NotImplementedException();
        }

        public override Task<List<Model.MarketTrade<CoinsEMarketId, CoinsETradeId>>> GetMarketTrades(CoinsEMarketId marketId)
        {
            throw new NotImplementedException();
        }

        public override Task<List<Model.MyTrade<CoinsEMarketId, CoinsEOrderId, CoinsETradeId>>> GetMyTrades(CoinsEMarketId marketId, int? limit)
        {
            throw new NotImplementedException();
        }

        public override Task<List<Model.MyTrade<CoinsEMarketId, CoinsEOrderId, CoinsETradeId>>> GetAllMyTrades(int? limit)
        {
            throw new NotImplementedException();
        }

        public override Task<List<Model.MyOrder<CoinsEMarketId, CoinsEOrderId>>> GetMyOrders(CoinsEMarketId marketId, int? limit)
        {
            throw new NotImplementedException();
        }

        public override Task<List<Model.MyOrder<CoinsEMarketId, CoinsEOrderId>>> GetAllMyOrders(int? limit)
        {
            throw new NotImplementedException();
        }

        public override Task<Model.Book> GetMarketDepth(CoinsEMarketId marketId)
        {
            throw new NotImplementedException();
        }

        public override Task CancelOrder(CoinsEOrderId orderId)
        {
            throw new NotImplementedException();
        }

        public override Task CancelAllOrders()
        {
            throw new NotImplementedException();
        }

        public override Task CancelMarketOrders(CoinsEMarketId marketId)
        {
            throw new NotImplementedException();
        }

        public override Task<CoinsEOrderId> CreateOrder(CoinsEMarketId marketId, OrderType orderType, decimal quantity, decimal price)
        {
            throw new NotImplementedException();
        }

        public override string GetNextNonce()
        {
            throw new NotImplementedException();
        }
    }
}