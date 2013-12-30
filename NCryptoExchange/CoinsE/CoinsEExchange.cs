using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.IO;

namespace Lostics.NCryptoExchange.CoinsE
{
    public class CoinsEExchange : IExchange<CoinsEMarketId, CoinsEOrderId, CoinsETradeId, Wallet>
    {
        public const string MARKETS_LIST = "https://www.coins-e.com/api/v2/markets/list/";

        private HttpClient client = new HttpClient();

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="url">Endpoint to make a request to</param>
        /// <param name="propertyName"></param>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        private async Task<JToken> CallPublic(string url, string propertyName, JTokenType propertyType)
        {
            HttpContent request = new StringContent("");
            HttpResponseMessage response = await client.PostAsync(url, request);

            JObject jsonObj;

            using (Stream jsonStream = await response.Content.ReadAsStreamAsync())
            {
                using (StreamReader jsonStreamReader = new StreamReader(jsonStream))
                {
                    try
                    {
                        jsonObj = JObject.Parse(await jsonStreamReader.ReadToEndAsync());
                    }
                    catch (ArgumentException e)
                    {
                        throw new CoinsEResponseException("Could not parse response from Cryptsy.", e);
                    }
                }
            }

            return jsonObj;
        }

        public void Dispose()
        {
            client.Dispose();
        }

        public Task<Model.AccountInfo<Wallet>> GetAccountInfo()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Model.Market<CoinsEMarketId>>> GetMarkets()
        {
            JObject marketsJson = (JObject)await CallPublic(MARKETS_LIST, "markets", JTokenType.Object);
        }

        public Task<List<Model.Transaction>> GetMyTransactions()
        {
            throw new NotImplementedException();
        }

        public Task<List<Model.MarketTrade<CoinsEMarketId, CoinsETradeId>>> GetMarketTrades(CoinsEMarketId marketId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Model.MyTrade<CoinsEMarketId, CoinsEOrderId, CoinsETradeId>>> GetMyTrades(CoinsEMarketId marketId, int? limit)
        {
            throw new NotImplementedException();
        }

        public Task<List<Model.MyTrade<CoinsEMarketId, CoinsEOrderId, CoinsETradeId>>> GetAllMyTrades(int? limit)
        {
            throw new NotImplementedException();
        }

        public Task<List<Model.MyOrder<CoinsEMarketId, CoinsEOrderId>>> GetMyOrders(CoinsEMarketId marketId, int? limit)
        {
            throw new NotImplementedException();
        }

        public Task<List<Model.MyOrder<CoinsEMarketId, CoinsEOrderId>>> GetAllMyOrders(int? limit)
        {
            throw new NotImplementedException();
        }

        public Task<Model.Book> GetMarketDepth(CoinsEMarketId marketId)
        {
            throw new NotImplementedException();
        }

        public Task CancelOrder(CoinsEOrderId orderId)
        {
            throw new NotImplementedException();
        }

        public Task CancelAllOrders()
        {
            throw new NotImplementedException();
        }

        public Task CancelMarketOrders(CoinsEMarketId marketId)
        {
            throw new NotImplementedException();
        }

        public Task<CoinsEOrderId> CreateOrder(CoinsEMarketId marketId, Model.OrderType orderType, Model.Price quantity, Model.Price price)
        {
            throw new NotImplementedException();
        }

        public string GetNextNonce()
        {
            throw new NotImplementedException();
        }
    }
}
