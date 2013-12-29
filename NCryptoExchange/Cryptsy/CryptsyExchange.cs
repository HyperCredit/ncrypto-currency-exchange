using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public class CryptsyExchange : IExchange<CryptsyMarketId, CryptsyOrderId, Wallet> , IDisposable
    {
        public const string HEADER_SIGN = "Sign";
        public const string HEADER_KEY = "Key";

        public const string METHOD_CANCEL_ORDER = "cancelorder";
        public const string METHOD_CANCEL_ALL_ORDERS = "cancelallorders";
        public const string METHOD_CANCEL_MARKET_ORDERS = "cancelmarketorder";
        public const string METHOD_CALCULATE_FEES = "calculatefees";
        public const string METHOD_CREATE_ORDER = "createorder";
        public const string METHOD_GET_INFO = "getinfo";
        public const string METHOD_GET_MARKETS = "getmarkets";
        public const string METHOD_MARKET_ORDERS = "marketorders";

        public const string PARAM_MARKET_ID = "marketid";
        public const string PARAM_METHOD = "method";
        public const string PARAM_NONCE = "nonce";
        public const string PARAM_ORDER_ID = "orderid";
        public const string PARAM_ORDER_TYPE = "ordertype";
        public const string PARAM_PRICE = "price";
        public const string PARAM_QUANTITY = "quantity";

        private HttpClient client = new HttpClient();
        private readonly string publicUrl = "http://pubapi.cryptsy.com/api.php";
        private readonly string privateUrl = "https://www.cryptsy.com/api";
        private readonly string publicKey;
        private readonly byte[] privateKey;

        public byte[] PrivateKey { get { return this.privateKey; } }

        public CryptsyExchange(string publicKey, string privateKey)
        {
            this.publicKey = publicKey;
            this.privateKey = System.Text.Encoding.ASCII.GetBytes(privateKey);
        }

        /// <summary>
        /// Asserts that the response from Cryptsy indicates success, and throws an exception
        /// if not.
        /// </summary>
        /// <param name="jsonObj"></param>
        /// <exception cref="CryptsyFailureException">Where there was an error reported by Cryptsy.</exception>
        /// <exception cref="CryptsyResponseException">Where there was a problem parsing the response from Cryptsy.</exception>
        private void AssertSuccess(JObject jsonObj)
        {
            if (null == jsonObj["success"])
            {
                throw new CryptsyResponseException("No success value returned in response from Cryptsy.");
            }

            if (!(jsonObj["success"].ToString().Equals("1")))
            {
                string errorMessage = jsonObj["error"].ToString();

                if (null == errorMessage)
                {
                    throw new CryptsyFailureException("Error response returned from Cryptsy.");
                }
                else
                {
                    throw new CryptsyFailureException("Error response returned from Cryptsy: "
                        + errorMessage);
                }
            }
        }

        public async Task CancelOrder(CryptsyOrderId orderId)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(PARAM_METHOD, METHOD_CANCEL_ORDER),
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce()),
                new KeyValuePair<string, string>(PARAM_ORDER_ID, orderId.ToString())
            });

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            JObject jsonObj = await GetResponseAsJObject(response);

            AssertSuccess(jsonObj);

            return;
        }

        public async Task CancelAllOrders()
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(PARAM_METHOD, METHOD_CANCEL_ALL_ORDERS),
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce())
            });

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            JObject jsonObj = await GetResponseAsJObject(response);

            AssertSuccess(jsonObj);

            return;
        }

        public async Task CancelMarketOrders(CryptsyMarketId marketId)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(PARAM_METHOD, METHOD_CANCEL_MARKET_ORDERS),
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce()),
                new KeyValuePair<string, string>(PARAM_MARKET_ID, marketId.ToString())
            });

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            JObject jsonObj = await GetResponseAsJObject(response);

            AssertSuccess(jsonObj);

            return;
        }

        public async Task<Fees> CalculateFees(OrderType orderType, Quantity quantity,
                Quantity price)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(PARAM_METHOD, METHOD_CALCULATE_FEES),
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce()),
                new KeyValuePair<string, string>(PARAM_ORDER_TYPE, orderType.ToString()),
                new KeyValuePair<string, string>(PARAM_QUANTITY, quantity.ToString()),
                new KeyValuePair<string, string>(PARAM_PRICE, price.ToString())
            });

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            JObject returnObj = (JObject)await GetReturnAsJToken(response);

            return new Fees(Quantity.Parse(returnObj["fee"]),
                Quantity.Parse(returnObj["net"]));
        }

        public async Task<CryptsyOrderId> CreateOrder(CryptsyMarketId marketId,
                OrderType orderType, Quantity quantity,
                Quantity price)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(PARAM_METHOD, METHOD_CREATE_ORDER),
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce()),
                new KeyValuePair<string, string>(PARAM_ORDER_TYPE, orderType.ToString()),
                new KeyValuePair<string, string>(PARAM_QUANTITY, quantity.ToString()),
                new KeyValuePair<string, string>(PARAM_PRICE, price.ToString())
            });

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            JObject returnObj = (JObject)await GetReturnAsJToken(response);

            return new CryptsyOrderId(returnObj["orderid"].ToString());
        }

        public void Dispose() {
            this.client.Dispose();
        }

        public async Task<Address> GenerateNewAddress(string currencyCode)
        {
            throw new NotImplementedException();
        }

        public async Task<AccountInfo<Wallet>> GetAccountInfo()
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(PARAM_METHOD, METHOD_GET_INFO),
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce())
            });

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            JObject returnObj = (JObject)await GetReturnAsJToken(response);

            return CryptsyAccountInfo.Parse(returnObj);
        }

        public string GetNextNonce()
        {
            return DateTime.Now.Ticks.ToString();
        }

        public async Task<List<MarketOrder>> GetMarketOrders(CryptsyMarketId marketId)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(PARAM_METHOD, METHOD_MARKET_ORDERS),
                new KeyValuePair<string, string>(PARAM_MARKET_ID, marketId.ToString()),
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce())
            });

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            JObject returnObj = (JObject)await GetReturnAsJToken(response);

            List<MarketOrder> buyOrders = ParseMarketOrders(OrderType.Buy, (JArray)returnObj["buyorders"]);
            List<MarketOrder> sellOrders = ParseMarketOrders(OrderType.Sell, (JArray)returnObj["sellorders"]);

            buyOrders.AddRange(sellOrders);

            return buyOrders;
        }

        public async Task<List<Market<CryptsyMarketId>>> GetMarkets()
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(PARAM_METHOD, METHOD_GET_MARKETS),
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce())
            });

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            JArray returnObj = (JArray)await GetReturnAsJToken(response);
            List<Market<CryptsyMarketId>> markets = new List<Market<CryptsyMarketId>>();

            foreach (JToken marketToken in returnObj)
            {
                markets.Add(CryptsyMarket.Parse(marketToken));
            }

            return markets;
        }

        public async Task<List<Transaction>> GetMyTransactons()
        {
            throw new NotImplementedException();
        }

        public async Task<List<MarketTrade>> GetMarketTrades(CryptsyMarketId marketId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<MyTrade>> GetMyTrades(CryptsyMarketId marketId, int? limit)
        {
            throw new NotImplementedException();
        }

        public async Task<List<MyTrade>> GetAllMyTrades(int? limit)
        {
            throw new NotImplementedException();
        }

        public async Task<List<MyOrder>> GetMyOrders(CryptsyMarketId marketId, int? limit)
        {
            throw new NotImplementedException();
        }

        public async Task<List<MyOrder>> GetAllMyOrders(int? limit)
        {
            throw new NotImplementedException();
        }

        public async Task<List<MarketDepth>> GetMarketDepth(CryptsyMarketId marketId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the entire response from Cryptsy as a JObject.
        /// </summary>
        /// <param name="response">The HTTP response to read from</param>
        /// <returns>The response as a JObject</returns>
        /// <exception cref="IOException">Where there was a problem reading the response from Cryptsy.</exception>
        /// <exception cref="CryptsyResponseException">Where there was a problem parsing the response from Cryptsy.</exception>
        private async Task<JObject> GetResponseAsJObject(HttpResponseMessage response)
        {
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
                        throw new CryptsyResponseException("Could not parse response from Cryptsy.", e);
                    }
                }
            }

            return jsonObj;
        }

        /// <summary>
        /// Gets the "return" property from the response from Cryptsy, and returns
        /// it as a JObject. In case of an error response, throws a suitable Exception
        /// instead.
        /// </summary>
        /// <param name="response">The HTTP response to read from</param>
        /// <returns>The returned content from Cryptsy as a JObject</returns>
        /// <exception cref="IOException">Where there was a problem reading the response from Cryptsy.</exception>
        /// <exception cref="CryptsyResponseException">Where there was a problem parsing the response from Cryptsy.</exception>
        private async Task<JToken> GetReturnAsJToken(HttpResponseMessage response)
        {
            JObject jsonObj = await GetResponseAsJObject(response);

            AssertSuccess(jsonObj);

            return jsonObj["return"];
        }

        private List<MarketOrder> ParseMarketOrders(OrderType orderType, JArray jArray)
        {
            List<MarketOrder> orders = new List<MarketOrder>(jArray.Count);

            try
            {
                foreach (JObject jsonOrder in jArray)
                {
                    Quantity quantity = Quantity.Parse(jsonOrder["quantity"]);
                    Quantity price;

                    switch (orderType)
                    {
                        case OrderType.Buy:
                            price = Quantity.Parse(jsonOrder["buyprice"]);
                            break;
                        case OrderType.Sell:
                            price = Quantity.Parse(jsonOrder["sellprice"]);
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

        public async Task<FormUrlEncodedContent> SignRequest(FormUrlEncodedContent request)
        {
            HMAC digester = new HMACSHA512(this.PrivateKey);
            StringBuilder hex = new StringBuilder();
            byte[] requestBytes = System.Text.Encoding.ASCII.GetBytes(await request.ReadAsStringAsync());

            request.Headers.Add(HEADER_SIGN, BitConverter.ToString(digester.ComputeHash(requestBytes)).Replace("-", "").ToLower());
            request.Headers.Add(HEADER_KEY, this.publicKey);

            return request;
        }
    }
}
