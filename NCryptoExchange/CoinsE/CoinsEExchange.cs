using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.CoinsE
{
    /// <summary>
    /// Wrapper around the Cryptsy (https://www.coins-e.com/) API.
    /// 
    /// To use, requires a public and private key.
    /// </summary>
    public class CoinsEExchange : AbstractExchange, ICoinDataSource<CoinsECurrency>
    {
        public const string DEFAULT_BASE_URL = "https://www.coins-e.com/api/v2/";
        public const string COINS_LIST = DEFAULT_BASE_URL + "coins/list/";
        public const string MARKETS_DATA = DEFAULT_BASE_URL + "markets/data/";
        public const string MARKET_DEPTH_PATH = "depth/";
        public const string MARKETS_LIST = DEFAULT_BASE_URL + "markets/list/";
        public const string WALLETS_LIST = DEFAULT_BASE_URL + "wallet/all/";

        public const string HEADER_SIGN = "sign";
        public const string HEADER_KEY = "key";

        public const string PARAM_CURSOR = "cursor";
        public const string PARAM_FILTER = "filter";
        public const string PARAM_LIMIT = "limit";
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
            this.BaseUrl = DEFAULT_BASE_URL;

            this.publicKey = publicKey;
            this.privateKey = System.Text.Encoding.ASCII.GetBytes(privateKey);
        }

        private static void AssertSuccessStatus(JObject jsonObj)
        {
            bool? status = jsonObj.Value<bool>("status");

            if (null == status)
            {
                throw new CoinsEResponseException("Response from Coins-E did not include a \"success\" property.");
            }

            if (!(bool)status)
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
        }

        public FormUrlEncodedContent BuildPrivateRequest(CoinsEMethod method)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(PARAM_METHOD, Enum.GetName(method.GetType(), method)),
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce())
            });

            request.Headers.Add(HEADER_KEY, this.publicKey);
            request.Headers.Add(HEADER_SIGN, GenerateSHA512Signature(request, this.privateKey));

            return request;
        }

        public FormUrlEncodedContent BuildPrivateRequest(CoinsEMethod method, CoinsEOrderFilter filter,
            string cursor, int? limit)
        {
            List<KeyValuePair<string, string>> kvPairs = new List<KeyValuePair<string, string>>(
                new[] {
                    new KeyValuePair<string, string>(PARAM_METHOD, Enum.GetName(method.GetType(), method)),
                    new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce()),
                    new KeyValuePair<string, string>(PARAM_FILTER, Enum.GetName(typeof(CoinsEOrderFilter), filter).ToLower())
                }
            );

            if (null != cursor)
            {
                kvPairs.Add(new KeyValuePair<string, string>(PARAM_CURSOR, cursor));
            }
            if (null != limit)
            {
                kvPairs.Add(new KeyValuePair<string, string>(PARAM_LIMIT, limit.ToString()));
            }

            FormUrlEncodedContent request = new FormUrlEncodedContent(kvPairs.ToArray());

            request.Headers.Add(HEADER_KEY, this.publicKey);
            request.Headers.Add(HEADER_SIGN, GenerateSHA512Signature(request, this.privateKey));

            return request;
        }

        public FormUrlEncodedContent BuildPrivateRequest(CoinsEMethod method, OrderType orderType, 
            decimal quantity, decimal price)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(PARAM_METHOD, Enum.GetName(method.GetType(), method)),
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce()),
                new KeyValuePair<string, string>(PARAM_ORDER_TYPE, Enum.GetName(typeof(OrderType), orderType)),
                new KeyValuePair<string, string>(PARAM_RATE, price.ToString()),
                new KeyValuePair<string, string>(PARAM_QUANTITY, quantity.ToString())
            });

            request.Headers.Add(HEADER_KEY, this.publicKey);
            request.Headers.Add(HEADER_SIGN, GenerateSHA512Signature(request, this.privateKey));

            return request;
        }

        public FormUrlEncodedContent BuildPrivateRequest(CoinsEMethod method, OrderId orderId)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(PARAM_METHOD, Enum.GetName(method.GetType(), method)),
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce()),
                new KeyValuePair<string, string>(PARAM_ORDER_ID, orderId.ToString())
            });

            request.Headers.Add(HEADER_KEY, this.publicKey);
            request.Headers.Add(HEADER_SIGN, GenerateSHA512Signature(request, this.privateKey));

            return request;
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
                HttpResponseMessage response = await client.GetAsync(url);
                using (Stream jsonStream = await response.Content.ReadAsStreamAsync())
                {
                    using (StreamReader jsonStreamReader = new StreamReader(jsonStream))
                    {
                        jsonObj = JObject.Parse(await jsonStreamReader.ReadToEndAsync());
                    }
                }
            }
            catch (ArgumentException e)
            {
                throw new CoinsEResponseException("Could not parse response from Coins-E.", e);
            }

            AssertSuccessStatus(jsonObj);

            return jsonObj;
        }

        /// <summary>
        /// Make a call to a private (authenticated) API
        /// </summary>
        /// <param name="url">Endpoint to make a request to</param>
        /// <returns>The raw JSON returned from Coins-E</returns>
        private async Task<JObject> CallPrivate(CoinsEMethod method, string url)
        {
            return await CallPrivate(BuildPrivateRequest(method), url);
        }

        /// <summary>
        /// Make a call to a private (authenticated) API
        /// </summary>
        /// <param name="url">Endpoint to make a request to</param>
        /// <returns>The raw JSON returned from Coins-E</returns>
        private async Task<JObject> CallPrivate(CoinsEMethod method, OrderId orderId, string url)
        {
            return await CallPrivate(BuildPrivateRequest(method, orderId), url);
        }

        /// <summary>
        /// Make a call to a private (authenticated) API
        /// </summary>
        /// <param name="url">Endpoint to make a request to</param>
        /// <returns>The raw JSON returned from Coins-E</returns>
        private async Task<JObject> CallPrivate(CoinsEMethod method, CoinsEOrderFilter filter,
            string cursor, int? limit, string url)
        {
            return await CallPrivate(BuildPrivateRequest(method, filter, cursor, limit), url);
        }

        /// <summary>
        /// Make a call to a private (authenticated) API
        /// </summary>
        /// <param name="url">Endpoint to make a request to</param>
        /// <returns>The raw JSON returned from Coins-E</returns>
        private async Task<JObject> CallPrivate(CoinsEMethod method, OrderType orderType, decimal quantity, decimal price,
            string url)
        {
            return await CallPrivate(BuildPrivateRequest(method, orderType, quantity, price), url);
        }


        /// <summary>
        /// Make a call to a private (authenticated) API
        /// </summary>
        /// <param name="url">Endpoint to make a request to</param>
        /// <returns>The raw JSON returned from Coins-E</returns>
        private async Task<JObject> CallPrivate(FormUrlEncodedContent request, string url)
        {
            JObject jsonObj;

            try
            {
                HttpResponseMessage response = await client.PostAsync(url, request);
                using (Stream jsonStream = await response.Content.ReadAsStreamAsync())
                {
                    using (StreamReader jsonStreamReader = new StreamReader(jsonStream))
                    {
                        jsonObj = JObject.Parse(await jsonStreamReader.ReadToEndAsync());
                    }
                }
            }
            catch (ArgumentException e)
            {
                throw new CoinsEResponseException("Could not parse response from Coins-E.", e);
            }

            AssertSuccessStatus(jsonObj);

            return jsonObj;
        }

        public override async Task CancelOrder(OrderId orderId)
        {
            CoinsEOrderId coinsEOrderId = (CoinsEOrderId)orderId;

            JObject responseJson = await CallPrivate(CoinsEMethod.cancelorder, orderId, GetMarketUrl(coinsEOrderId.MarketId));
        }

        public override async Task<OrderId> CreateOrder(MarketId marketId, OrderType orderType, decimal quantity, decimal price)
        {
            JObject responseJson = await CallPrivate(CoinsEMethod.neworder, orderType, quantity, price, GetMarketUrl(marketId));

            return CoinsEMyOrder.Parse(responseJson.Value<JObject>("order")).OrderId;
        }

        public override void Dispose()
        {
            client.Dispose();
        }

        public override async Task<Model.AccountInfo> GetAccountInfo()
        {
            JObject responseJson = await CallPrivate(CoinsEMethod.getallwallets, WALLETS_LIST);

            return CoinsEAccountInfo.Parse(responseJson);
        }

        public async Task<List<CoinsECurrency>> GetCoins()
        {
            JArray coinsJson = (await CallPublic(COINS_LIST)).Value<JArray>("coins");

            return coinsJson.Select(coin => CoinsECurrency.Parse(coin as JObject)).ToList();
        }

        public override async Task<List<Market>> GetMarkets()
        {
            JObject marketsJson = (await CallPublic(MARKETS_DATA)).Value<JObject>("markets");
            
            return marketsJson.Properties().Select(
                 market => (Market)CoinsEMarket.Parse(market.Name, market.Value as JObject)
             ).ToList();
        }

        public override Task<List<Model.MarketTrade>> GetMarketTrades(MarketId marketId)
        {
            throw new NotImplementedException();
        }

        private string GetMarketUrl(MarketId marketId)
        {
            return this.BaseUrl + "market/"
                + Uri.EscapeUriString(marketId.ToString()) + "/";
        }

        public override async Task<Book> GetMarketDepth(MarketId marketId)
        {
            JObject responseJson = await CallPublic(this.GetMarketUrl(marketId) + MARKET_DEPTH_PATH);

            return CoinsEParsers.ParseMarketDepth(responseJson.Value<JObject>("marketdepth"));
        }

        public override async Task<List<Model.MyOrder>> GetMyActiveOrders(MarketId marketId, int? limit)
        {
            List<CoinsEMyOrder> activeOrders = await GetMyOrders(marketId, CoinsEOrderFilter.Active, null, limit);

            return activeOrders.ConvertAll(x => (Model.MyOrder)x);
        }

        public async Task<List<CoinsEMyOrder>> GetMyOrders(MarketId marketId,
            CoinsEOrderFilter filter, string cursor, int? limit)
        {
            JObject responseJson = await CallPrivate(CoinsEMethod.listorders, filter, cursor, limit, GetMarketUrl(marketId));

            return responseJson.Value<JArray>("orders").Select(
                 order => CoinsEMyOrder.Parse(order as JObject)
             ).ToList();
        }

        public override string GetNextNonce()
        {
            return DateTime.Now.Ticks.ToString();
        }

        public string BaseUrl { get; private set; }
        public override string Label
        {
            get { return "Coins-E"; }
        }
    }
}
