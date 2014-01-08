using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.CoinsE
{
    public class CoinsEExchange : AbstractExchange<CoinsEMarketId, CoinsEOrderId>
    {
        public const string COINS_LIST = "https://www.coins-e.com/api/v2/coins/list/";
        public const string MARKETS_LIST = "https://www.coins-e.com/api/v2/markets/list/";
        public const string WALLETS_LIST = "https://www.coins-e.com/api/v2/wallet/all/";

        public const string HEADER_SIGN = "sign";
        public const string HEADER_KEY = "key";

        public const string PARAM_METHOD = "method";
        public const string PARAM_NONCE = "nonce";
        public const string PARAM_ORDER_ID = "order_id";
        public const string PARAM_ORDER_TYPE = "order_type";
        public const string PARAM_QUANTITY = "quantity";
        public const string PARAM_RATE = "rate";

        private readonly string publicKey;
        private readonly byte[] privateKey;
        private HttpClient client = new HttpClient();

        public CoinsEExchange(string publicKey, string privateKey)
        {
            this.publicKey = publicKey;
            this.privateKey = System.Text.Encoding.ASCII.GetBytes(privateKey);
        }

        internal FormUrlEncodedContent BuildRequest(CoinsEMethod method)
        {
            return new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(PARAM_METHOD, Enum.GetName(method.GetType(), method)),
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce())
            });
        }

        internal FormUrlEncodedContent BuildRequest(CoinsEMethod method, OrderType orderType, 
            decimal price, decimal quantity)
        {
            return new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(PARAM_METHOD, Enum.GetName(method.GetType(), method)),
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce()),
                new KeyValuePair<string, string>(PARAM_ORDER_TYPE, Enum.GetName(typeof(OrderType), orderType)),
                new KeyValuePair<string, string>(PARAM_RATE, price.ToString()),
                new KeyValuePair<string, string>(PARAM_QUANTITY, quantity.ToString())
            });
        }

        internal FormUrlEncodedContent BuildRequest(CoinsEMethod method, CoinsEOrderId orderId)
        {
            return new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(PARAM_METHOD, Enum.GetName(method.GetType(), method)),
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce()),
                new KeyValuePair<string, string>(PARAM_ORDER_ID, orderId.ToString())
            });
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="url">Endpoint to make a request to</param>
        /// <param name="propertyName">Name of a property within the response JSON, to extract and return</param>
        /// <returns>The value of the property in the JSON response from Coins-E</returns>
        /// <typeparam name="T">The type of the property to return from the JSON response (i.e. JArray, JObject)</typeparam>
        private async Task<T> CallPublic<T>(string url, string propertyName)
            where T : JToken
        {
            JObject jsonObj = await CallPublic(url);

            return jsonObj.Value<T>(propertyName);
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="url">Endpoint to make a request to</param>
        /// <returns>The raw JSON returned from Coins-E</returns>
        private async Task<JObject> CallPublic(string url)
        {
            JObject jsonObj;

            try
            {
                jsonObj = await GetJsonFromResponse(await client.GetAsync(url));
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

            return jsonObj;
        }

        private async Task<T> CallPrivate<T>(CoinsEMethod method, string url, string propertyName)
            where T : JToken
        {
            JObject jsonObj = await CallPrivate(method, url);

            return jsonObj.Value<T>(propertyName);
        }

        /// <summary>
        /// Make a call to a private (authenticated) API
        /// </summary>
        /// <param name="url">Endpoint to make a request to</param>
        /// <returns>The raw JSON returned from Coins-E</returns>
        private async Task<JObject> CallPrivate(CoinsEMethod method, string url)
        {
            FormUrlEncodedContent request = BuildRequest(method);

            request.Headers.Add(HEADER_KEY, this.publicKey);
            request.Headers.Add(HEADER_SIGN, await GenerateSHA512Signature(request, this.privateKey));

            JObject jsonObj;

            try
            {
                jsonObj = await GetJsonFromResponse(await client.PostAsync(url, request));
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

            return jsonObj;
        }

        public override void Dispose()
        {
            client.Dispose();
        }

        public override async Task<Model.AccountInfo> GetAccountInfo()
        {
            JObject responseJson = await CallPrivate(CoinsEMethod.getallwallets, WALLETS_LIST);

            return CoinsEParsers.ParseAccountInfo(responseJson);
        }

        public async Task<List<CoinsECurrency>> GetCoins()
        {
            JArray coinsJson = await CallPublic<JArray>(COINS_LIST, "coins");

            return coinsJson.Select(coin => CoinsECurrency.Parse(coin as JObject)).ToList();
        }

        public override async Task<List<Market<CoinsEMarketId>>> GetMarkets()
        {
            JArray marketsJson = await CallPublic<JArray>(MARKETS_LIST, "markets");
            
            return marketsJson.Select(
                 market => (Market<CoinsEMarketId>)CoinsEMarket.Parse(market as JObject)
             ).ToList();
        }

        public override Task<List<Model.Transaction>> GetMyTransactions()
        {
            throw new NotImplementedException();
        }

        public override Task<Book> GetMarketOrders(CoinsEMarketId marketId)
        {
            throw new NotImplementedException();
        }

        public override Task<List<Model.MarketTrade<CoinsEMarketId>>> GetMarketTrades(CoinsEMarketId marketId)
        {
            throw new NotImplementedException();
        }

        public override Task<List<Model.MyTrade<CoinsEMarketId, CoinsEOrderId>>> GetMyTrades(CoinsEMarketId marketId, int? limit)
        {
            throw new NotImplementedException();
        }

        public override Task<List<Model.MyTrade<CoinsEMarketId, CoinsEOrderId>>> GetAllMyTrades(int? limit)
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
