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
    public class CryptsyExchange : IExchange<CryptsyMarketId, CryptsyOrderId, CryptsyTradeId, Wallet>
    {
        public const string HEADER_SIGN = "Sign";
        public const string HEADER_KEY = "Key";

        public const string PARAM_CURRENCY_CODE = "currencycode";
        public const string PARAM_MARKET_ID = "marketid";
        public const string PARAM_METHOD = "method";
        public const string PARAM_LIMIT = "limit";
        public const string PARAM_NONCE = "nonce";
        public const string PARAM_ORDER_ID = "orderid";
        public const string PARAM_ORDER_TYPE = "ordertype";
        public const string PARAM_PRICE = "price";
        public const string PARAM_QUANTITY = "quantity";
        public const string PROPERTY_PUBLIC_KEY = "public_key";
        public const string PROPERTY_PRIVATE_KEY = "private_key";

        private readonly TimeZoneInfo defaultTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

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

        public async Task CancelOrder(CryptsyOrderId orderId)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateRequest(CryptsyMethod.cancelorder,
                orderId, (CryptsyMarketId)null, null));

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            await GetReturnAsJToken(response);
        }

        public async Task CancelAllOrders()
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateRequest(CryptsyMethod.cancelallorders,
                (CryptsyOrderId)null, (CryptsyMarketId)null, null));

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            await GetReturnAsJToken(response);
        }

        public async Task CancelMarketOrders(CryptsyMarketId marketId)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateRequest(CryptsyMethod.cancelmarketorder,
                (CryptsyOrderId)null, marketId, null));

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            await GetReturnAsJToken(response);
        }

        public async Task<Fees> CalculateFees(OrderType orderType, Price quantity,
                Price price)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateOrderRequest(CryptsyMethod.calculatefees,
                orderType, quantity, price));

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            JObject returnObj = (JObject)await GetReturnAsJToken(response);

            return new Fees(Price.Parse(returnObj["fee"]),
                Price.Parse(returnObj["net"]));
        }

        public async Task<CryptsyOrderId> CreateOrder(CryptsyMarketId marketId,
                OrderType orderType, Price quantity,
                Price price)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateOrderRequest(CryptsyMethod.createorder,
                orderType, quantity, price));

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
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(PARAM_METHOD, System.Enum.GetName(typeof(CryptsyMethod), CryptsyMethod.generatenewaddress)),
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce()),
                new KeyValuePair<string, string>(PARAM_CURRENCY_CODE, currencyCode)
            });

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            JObject returnObj = (JObject)await GetReturnAsJToken(response);

            return Address.Parse(returnObj);
        }

        private KeyValuePair<string, string>[] GenerateRequest(CryptsyMethod method,
            CryptsyOrderId orderId, CryptsyMarketId marketId, int? limit)
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(PARAM_METHOD, System.Enum.GetName(typeof(CryptsyMethod), method)),
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce())
            };

            if (null != marketId)
            {
                parameters.Add(new KeyValuePair<string, string>(PARAM_MARKET_ID, marketId.ToString()));
            }

            if (null != orderId)
            {
                parameters.Add(new KeyValuePair<string, string>(PARAM_MARKET_ID, orderId.ToString()));
            }

            if (null != limit)
            {
                parameters.Add(new KeyValuePair<string, string>(PARAM_LIMIT, limit.ToString()));
            }

            return parameters.ToArray();
        }

        private KeyValuePair<string, string>[] GenerateOrderRequest(CryptsyMethod method,
            OrderType orderType, Price quantity, Price price)
        {
            return new[] {
                new KeyValuePair<string, string>(PARAM_METHOD, System.Enum.GetName(typeof(CryptsyMethod), method)),
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce()),
                new KeyValuePair<string, string>(PARAM_ORDER_TYPE, orderType.ToString()),
                new KeyValuePair<string, string>(PARAM_QUANTITY, quantity.ToString()),
                new KeyValuePair<string, string>(PARAM_PRICE, price.ToString())
            };
        }

        public async Task<AccountInfo<Wallet>> GetAccountInfo()
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateRequest(CryptsyMethod.getinfo,
                (CryptsyOrderId)null, (CryptsyMarketId)null, null));

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            JObject returnObj = (JObject)await GetReturnAsJToken(response);

            return CryptsyAccountInfo.Parse(returnObj);
        }

        public static CryptsyExchange GetExchange(FileInfo configurationFile)
        {
            string publicKey = null;
            string privateKey = null;

            if (!configurationFile.Exists)
            {
                WriteDefaultFile(configurationFile);
                throw new ConfigurationException("No configuration file exists; blank default created. "
                    + "Please enter public and private key values and try again.");
            }

            using (StreamReader reader = new StreamReader(new FileStream(configurationFile.FullName, FileMode.Open)))
            {
                string line = reader.ReadLine();

                while (null != line)
                {
                    line = line.Trim();

                    // Ignore comment lines
                    if (!line.StartsWith("#"))
                    {
                        string[] parts = line.Split(new[] { '=' });
                        if (parts.Length >= 2)
                        {
                            string name = parts[0].Trim().ToLower();

                            switch (name)
                            {
                                case PROPERTY_PUBLIC_KEY:
                                    publicKey = parts[1].Trim();
                                    break;
                                case PROPERTY_PRIVATE_KEY:
                                    privateKey = parts[1].Trim();
                                    break;
                                default:
                                    Console.Error.WriteLine("Found unknown property \""
                                        + parts[0] + "\".");
                                    break;
                            }
                        }
                    }

                    line = reader.ReadLine();
                }
            }

            if (null == publicKey)
            {
                throw new ConfigurationException("No public key specified in configuration file \""
                    + configurationFile.FullName + "\".");
            }

            if (null == privateKey)
            {
                throw new ConfigurationException("No public key specified in configuration file \""
                    + configurationFile.FullName + "\".");
            }

            return new CryptsyExchange(publicKey, privateKey);
        }

        private static void WriteDefaultFile(FileInfo file)
        {
            using (StreamWriter writer = new StreamWriter(new FileStream(file.FullName, FileMode.CreateNew)))
            {
                writer.WriteLine("# Configuration file for specifying API public & private key.");
                writer.WriteLine(PROPERTY_PUBLIC_KEY + "=");
                writer.WriteLine(PROPERTY_PRIVATE_KEY + "=");
            }
        }

        public string GetNextNonce()
        {
            return DateTime.Now.Ticks.ToString();
        }

        public async Task<List<MarketOrder>> GetMarketOrders(CryptsyMarketId marketId)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateRequest(CryptsyMethod.marketorders,
                (CryptsyOrderId)null, marketId, null));

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            JObject returnObj = (JObject)await GetReturnAsJToken(response);

            List<MarketOrder> buyOrders = Parsers.ParseMarketOrders(OrderType.Buy, (JArray)returnObj["buyorders"]);
            List<MarketOrder> sellOrders = Parsers.ParseMarketOrders(OrderType.Sell, (JArray)returnObj["sellorders"]);

            buyOrders.AddRange(sellOrders);

            return buyOrders;
        }

        public async Task<List<Market<CryptsyMarketId>>> GetMarkets()
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateRequest(CryptsyMethod.getmarkets,
                (CryptsyOrderId)null, (CryptsyMarketId)null, null));

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            JArray returnArray = (JArray)await GetReturnAsJToken(response);

            return Parsers.ParseMarkets(returnArray, defaultTimeZone);
        }

        public async Task<List<Transaction>> GetMyTransactions()
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateRequest(CryptsyMethod.mytransactions,
                (CryptsyOrderId)null, (CryptsyMarketId)null, null));

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            JArray returnArray = (JArray)await GetReturnAsJToken(response);

            return Parsers.ParseTransactions(returnArray);
        }

        public async Task<List<MarketTrade<CryptsyMarketId, CryptsyTradeId>>> GetMarketTrades(CryptsyMarketId marketId)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateRequest(CryptsyMethod.markettrades,
                (CryptsyOrderId)null, marketId, null));

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            JArray returnArray = (JArray)await GetReturnAsJToken(response);

            return Parsers.ParseMarketTrades(returnArray, marketId);
        }

        public async Task<List<MyTrade<CryptsyMarketId, CryptsyOrderId, CryptsyTradeId>>> GetMyTrades(CryptsyMarketId marketId, int? limit)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateRequest(CryptsyMethod.mytrades,
                (CryptsyOrderId)null, marketId, limit));

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            JArray returnArray = (JArray)await GetReturnAsJToken(response);

            return Parsers.ParseMyTrades(returnArray, marketId);
        }

        public async Task<List<MyTrade<CryptsyMarketId, CryptsyOrderId, CryptsyTradeId>>> GetAllMyTrades(int? limit)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateRequest(CryptsyMethod.allmytrades,
                (CryptsyOrderId)null, (CryptsyMarketId)null, limit));

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            JArray returnArray = (JArray)await GetReturnAsJToken(response);

            return Parsers.ParseMyTrades(returnArray, null);
        }

        public async Task<List<MyOrder<CryptsyMarketId, CryptsyOrderId>>> GetMyOrders(CryptsyMarketId marketId, int? limit)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateRequest(CryptsyMethod.myorders,
                (CryptsyOrderId)null, marketId, limit));

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            JArray returnArray = (JArray)await GetReturnAsJToken(response);

            return Parsers.ParseMyOrders(returnArray, marketId);
        }

        public async Task<List<MyOrder<CryptsyMarketId, CryptsyOrderId>>> GetAllMyOrders(int? limit)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateRequest(CryptsyMethod.allmyorders,
                (CryptsyOrderId)null, (CryptsyMarketId)null, limit));

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            JArray returnArray = (JArray)await GetReturnAsJToken(response);

            return Parsers.ParseMyOrders(returnArray, null);
        }

        public async Task<Book> GetMarketDepth(CryptsyMarketId marketId)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateRequest(CryptsyMethod.depth,
                (CryptsyOrderId)null, marketId, null));

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            JObject returnArray = (JObject)await GetReturnAsJToken(response);

            return Parsers.ParseMarketDepthBook(returnArray, marketId);
        }

        /// <summary>
        /// Gets the "return" property from the response from Cryptsy, and returns
        /// it as a JToken. In case of an error response, throws a suitable Exception
        /// instead.
        /// </summary>
        /// <param name="response">The HTTP response to read from</param>
        /// <returns>The returned content from Cryptsy as a JToken. May be null
        /// if no return was provided.</returns>
        /// <exception cref="IOException">Where there was a problem reading the
        /// response from Cryptsy.</exception>
        /// <exception cref="CryptsyFailureException">Where Cryptsy returned an error.</exception>
        /// <exception cref="CryptsyResponseException">Where there was a problem
        /// parsing the response from Cryptsy.</exception>
        private static async Task<JToken> GetReturnAsJToken(HttpResponseMessage response)
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

            return jsonObj["return"];
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
