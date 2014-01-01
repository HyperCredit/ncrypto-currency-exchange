using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange
{
    public abstract class AbstractExchange<M, O, MO> : IDisposable
        where M : MarketId
        where O : OrderId
        where MO: MarketOrder
    {
        public static async Task<string> GenerateSHA512Signature(FormUrlEncodedContent request, byte[] privateKey)
        {
            HMAC digester = new HMACSHA512(privateKey);
            StringBuilder hex = new StringBuilder();
            byte[] requestBytes = System.Text.Encoding.ASCII.GetBytes(await request.ReadAsStringAsync());

            return BitConverter.ToString(digester.ComputeHash(requestBytes)).Replace("-", "").ToLower();
        }

        public static async Task<JObject> GetJsonFromResponse(HttpResponseMessage response)
        {
            JObject jsonObj;

            using (Stream jsonStream = await response.Content.ReadAsStreamAsync())
            {
                using (StreamReader jsonStreamReader = new StreamReader(jsonStream))
                {
                    jsonObj = JObject.Parse(await jsonStreamReader.ReadToEndAsync());
                }
            }

            return jsonObj;
        }

        public abstract void Dispose();

        public abstract Task<AccountInfo> GetAccountInfo();

        public abstract Task<List<Market<M>>> GetMarkets();

        public abstract Task<List<Transaction>> GetMyTransactions();

        public abstract Task<MarketOrders<MO>> GetMarketOrders(M marketId);

        public abstract Task<List<Model.MarketTrade<M>>> GetMarketTrades(M marketId);

        public abstract Task<List<Model.MyTrade<M, O>>> GetMyTrades(M marketId, int? limit);

        public abstract Task<List<Model.MyTrade<M, O>>> GetAllMyTrades(int? limit);

        public abstract Task<List<Model.MyOrder<M, O>>> GetMyOrders(M marketId, int? limit);

        public abstract Task<List<Model.MyOrder<M, O>>> GetAllMyOrders(int? limit);

        public abstract Task<Model.Book> GetMarketDepth(M marketId);

        public abstract Task CancelOrder(O orderId);

        public abstract Task CancelAllOrders();

        public abstract Task CancelMarketOrders(M marketId);

        public abstract Task<O> CreateOrder(M marketId, OrderType orderType, decimal quantity, decimal price);

        public abstract string GetNextNonce();
    }
}
