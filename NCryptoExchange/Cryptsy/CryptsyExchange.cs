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
    /// <summary>
    /// Wrapper around the Cryptsy (https://www.cryptsy.com/) API.
    /// 
    /// To use, requires a public and private key (these can be generated from the
    /// "Settings" page within Cryptsy, once logged in). It's suggested these are
    /// stored in a configuration file, and the method GetExchange()
    /// will load the keys from that file for you (provided with a path to
    /// a file that does not exist, it will create a blank file as a template).
    /// </summary>
    public class CryptsyExchange : AbstractExchange<CryptsyMarketId, CryptsyOrderId, CryptsyTradeId, Wallet>
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
        private DirectoryInfo dumpResponse = null;
        private readonly string privateUrl = "https://www.cryptsy.com/api";
        private readonly string publicKey;
        private readonly byte[] privateKey;

        public CryptsyExchange(string publicKey, string privateKey)
        {
            this.publicKey = publicKey;
            this.privateKey = System.Text.Encoding.ASCII.GetBytes(privateKey);
        }

        /// <summary>
        /// Make a call to Cryptsy's private API
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task Call(FormUrlEncodedContent request)
        {
            request.Headers.Add(HEADER_SIGN, await GenerateSHA512Signature(request, this.privateKey));
            request.Headers.Add(HEADER_KEY, this.publicKey);

            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            await ParseResponseToJson(response, null);
        }

        /// <summary>
        /// Make a call to Cryptsy's private API
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The return from Cryptsy as a JSON token</returns>
        private async Task<JToken> Call(FormUrlEncodedContent request, JTokenType returnType)
        {
            request.Headers.Add(HEADER_SIGN, await GenerateSHA512Signature(request, this.privateKey));
            request.Headers.Add(HEADER_KEY, this.publicKey);

            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            return await ParseResponseToJson(response, returnType);
        }

        public async override Task CancelOrder(CryptsyOrderId orderId)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateParameters(CryptsyMethod.cancelorder,
                orderId, (CryptsyMarketId)null, null));

            await Call(request);
        }

        public async override Task CancelAllOrders()
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateParameters(CryptsyMethod.cancelallorders,
                (CryptsyOrderId)null, (CryptsyMarketId)null, null));

            await Call(request);
        }

        public async override Task CancelMarketOrders(CryptsyMarketId marketId)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateParameters(CryptsyMethod.cancelmarketorder,
                (CryptsyOrderId)null, marketId, null));

            await Call(request);
        }

        public async Task<Fees> CalculateFees(OrderType orderType, decimal quantity,
                decimal price)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(
                GenerateOrderParameters(CryptsyMethod.calculatefees, null,
                    orderType, quantity, price));
            JObject returnObj = (JObject)await Call(request, JTokenType.Object);

            return new Fees(returnObj.Value<decimal>("fee"),
                returnObj.Value<decimal>("net"));
        }

        public async override Task<CryptsyOrderId> CreateOrder(CryptsyMarketId marketId,
                OrderType orderType, decimal quantity,
                decimal price)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(
                GenerateOrderParameters(CryptsyMethod.createorder, marketId,
                    orderType, quantity, price));
            JObject returnObj = (JObject)await Call(request, JTokenType.Object);

            return new CryptsyOrderId(returnObj["orderid"].ToString());
        }

        public override void Dispose()
        {
            this.client.Dispose();
        }

        public async Task<Address> GenerateNewAddress(string currencyCode)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(PARAM_METHOD, System.Enum.GetName(typeof(CryptsyMethod), CryptsyMethod.generatenewaddress)),
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce()),
                new KeyValuePair<string, string>(PARAM_CURRENCY_CODE, currencyCode)
            });
            JObject returnObj = (JObject)await Call(request, JTokenType.Object);

            return Address.Parse(returnObj);
        }

        /// <summary>
        /// Constructs parameters to be sent as part of a request to Cryptsy.
        /// </summary>
        /// <param name="method">The API method to call</param>
        /// <param name="orderId">An optional order ID to be passed as a parameter to the method</param>
        /// <param name="marketId">An optional market ID to be passsed as a parameter to the method</param>
        /// <param name="limit">An optional limit on number of returned items, to be passed as a parameter to the method</param>
        /// <returns>An array of key-value pairs</returns>
        private KeyValuePair<string, string>[] GenerateParameters(CryptsyMethod method,
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
                parameters.Add(new KeyValuePair<string, string>(PARAM_ORDER_ID, orderId.ToString()));
            }

            if (null != limit)
            {
                parameters.Add(new KeyValuePair<string, string>(PARAM_LIMIT, limit.ToString()));
            }

            return parameters.ToArray();
        }

        /// <summary>
        /// Constructs parameters to be sent as part of a request to Cryptsy.
        /// </summary>
        /// <param name="method">The API method to call</param>
        /// <param name="marketId">An optional market ID to be passsed as a parameter to the method</param>
        /// <returns>An array of key-value pairs</returns>
        private KeyValuePair<string, string>[] GenerateOrderParameters(CryptsyMethod method,
            CryptsyMarketId marketId, OrderType orderType, decimal quantity, decimal price)
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(PARAM_METHOD, System.Enum.GetName(typeof(CryptsyMethod), method)),
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce()),
                new KeyValuePair<string, string>(PARAM_ORDER_TYPE, orderType.ToString()),
                new KeyValuePair<string, string>(PARAM_QUANTITY, quantity.ToString()),
                new KeyValuePair<string, string>(PARAM_PRICE, price.ToString())
            };

            if (null != marketId)
            {
                parameters.Add(new KeyValuePair<string, string>(PARAM_MARKET_ID, marketId.ToString()));
            }

            return parameters.ToArray();
        }

        public async override Task<AccountInfo<Wallet>> GetAccountInfo()
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateParameters(CryptsyMethod.getinfo,
                (CryptsyOrderId)null, (CryptsyMarketId)null, null));
            JObject returnObj = (JObject)await Call(request, JTokenType.Object);

            return CryptsyParsers.ParseAccountInfo(returnObj);
        }

        public static CryptsyExchange GetExchange(FileInfo configurationFile)
        {
            string publicKey = null;
            string privateKey = null;

            if (!configurationFile.Exists)
            {
                WriteDefaultConfigurationFile(configurationFile);
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

        public override string GetNextNonce()
        {
            return DateTime.Now.Ticks.ToString();
        }

        public async Task<List<MarketOrder>> GetMarketOrders(CryptsyMarketId marketId)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateParameters(CryptsyMethod.marketorders,
                (CryptsyOrderId)null, marketId, null));
            JObject returnObj = (JObject)await Call(request, JTokenType.Object);

            List<MarketOrder> buyOrders = CryptsyParsers.ParseMarketOrders(OrderType.Buy, (JArray)returnObj["buyorders"]);
            List<MarketOrder> sellOrders = CryptsyParsers.ParseMarketOrders(OrderType.Sell, (JArray)returnObj["sellorders"]);

            buyOrders.AddRange(sellOrders);

            return buyOrders;
        }

        public async override Task<List<Market<CryptsyMarketId>>> GetMarkets()
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateParameters(CryptsyMethod.getmarkets,
                (CryptsyOrderId)null, (CryptsyMarketId)null, null));
            JArray returnArray = (JArray)await Call(request, JTokenType.Array);

            return CryptsyParsers.ParseMarkets(returnArray, defaultTimeZone);
        }

        public async override Task<List<Transaction>> GetMyTransactions()
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateParameters(CryptsyMethod.mytransactions,
                (CryptsyOrderId)null, (CryptsyMarketId)null, null));
            JArray returnArray = (JArray)await Call(request, JTokenType.Array);

            return CryptsyParsers.ParseTransactions(returnArray);
        }

        public async override Task<List<MarketTrade<CryptsyMarketId, CryptsyTradeId>>> GetMarketTrades(CryptsyMarketId marketId)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateParameters(CryptsyMethod.markettrades,
                (CryptsyOrderId)null, marketId, null));
            JArray returnArray = (JArray)await Call(request, JTokenType.Array);

            return CryptsyParsers.ParseMarketTrades(returnArray, marketId, defaultTimeZone);
        }

        public async override Task<List<MyTrade<CryptsyMarketId, CryptsyOrderId, CryptsyTradeId>>> GetMyTrades(CryptsyMarketId marketId, int? limit)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateParameters(CryptsyMethod.mytrades,
                (CryptsyOrderId)null, marketId, limit));
            JArray returnArray = (JArray)await Call(request, JTokenType.Array);

            // XXX: Should use timezone provided by Cryptsy, not just presume.

            return CryptsyParsers.ParseMyTrades(returnArray, marketId, defaultTimeZone);
        }

        public async override Task<List<MyTrade<CryptsyMarketId, CryptsyOrderId, CryptsyTradeId>>> GetAllMyTrades(int? limit)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateParameters(CryptsyMethod.allmytrades,
                (CryptsyOrderId)null, (CryptsyMarketId)null, limit));
            JArray returnArray = (JArray)await Call(request, JTokenType.Array);

            // XXX: Should use timezone provided by Cryptsy, not just presume.

            return CryptsyParsers.ParseMyTrades(returnArray, null, defaultTimeZone);
        }

        public async override Task<List<MyOrder<CryptsyMarketId, CryptsyOrderId>>> GetMyOrders(CryptsyMarketId marketId, int? limit)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateParameters(CryptsyMethod.myorders,
                (CryptsyOrderId)null, marketId, limit));
            JArray returnArray = (JArray)await Call(request, JTokenType.Array);

            // XXX: Should use timezone provided by Cryptsy, not just presume.

            return CryptsyParsers.ParseMyOrders(returnArray, marketId, defaultTimeZone);
        }

        public async override Task<List<MyOrder<CryptsyMarketId, CryptsyOrderId>>> GetAllMyOrders(int? limit)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateParameters(CryptsyMethod.allmyorders,
                (CryptsyOrderId)null, (CryptsyMarketId)null, limit));
            JArray returnArray = (JArray)await Call(request, JTokenType.Array);

            // XXX: Should use timezone provided by Cryptsy, not just presume.

            return CryptsyParsers.ParseMyOrders(returnArray, null, defaultTimeZone);
        }

        public async override Task<Book> GetMarketDepth(CryptsyMarketId marketId)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateParameters(CryptsyMethod.depth,
                (CryptsyOrderId)null, marketId, null));
            JObject returnObj = (JObject)await Call(request, JTokenType.Object);

            return CryptsyParsers.ParseMarketDepthBook(returnObj, marketId);
        }

        /// <summary>
        /// Gets the "return" property from the response from Cryptsy, and returns
        /// it as a JToken. In case of an error response, throws a suitable Exception
        /// instead.
        /// </summary>
        /// <param name="response">The HTTP response to read from</param>
        /// <param name="requiredType">The JSON token type of the return. If no
        /// return value is expected, pass in null.</param>
        /// <returns>The returned content from Cryptsy as a JToken. May be null
        /// if no return was provided.</returns>
        /// <exception cref="IOException">Where there was a problem reading the
        /// response from Cryptsy.</exception>
        /// <exception cref="CryptsyFailureException">Where Cryptsy returned an error.</exception>
        /// <exception cref="CryptsyResponseException">Where there was a problem
        /// parsing the response from Cryptsy.</exception>
        private async Task<JToken> ParseResponseToJson(HttpResponseMessage response, JTokenType? requiredType)
        {
            JObject jsonObj;

            try
            {
                jsonObj = await GetJsonFromResponse(response);
            }
            catch (ArgumentException e)
            {
                throw new CryptsyResponseException("Could not parse response from Cryptsy.", e);
            }

            // Log the response from Cryptsy if configured to do so
            if (null != this.DumpResponse)
            {
                string filename = DateTime.Now.Millisecond.ToString() + ".txt";
                FileInfo logFile = new FileInfo(Path.Combine(this.DumpResponse.FullName, filename));

                Console.WriteLine("Writing log to "
                    + logFile.FullName);

                using (StreamWriter dumpTo = new StreamWriter(new FileStream(logFile.FullName, FileMode.CreateNew)))
                {
                    dumpTo.Write(jsonObj.ToString());
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

            JToken returnObj = jsonObj["return"];

            // For methods where we expect a response, verify the type of the
            // response (typically an array or object)
            if (null != requiredType)
            {
                if (null == returnObj)
                {
                    throw new CryptsyResponseException("Expected \"return\" JSON token in response from Cryptsy, but was missing.");
                }

                if (returnObj.Type != requiredType)
                {
                    throw new CryptsyResponseException("Expected \"return\" JSON token in response from Cryptsy, but was missing or the wrong type.");
                }
            }

            return returnObj;
        }

        private static void WriteDefaultConfigurationFile(FileInfo file)
        {
            using (StreamWriter writer = new StreamWriter(new FileStream(file.FullName, FileMode.CreateNew)))
            {
                writer.WriteLine("# Configuration file for specifying API public & private key.");
                writer.WriteLine(PROPERTY_PUBLIC_KEY + "=");
                writer.WriteLine(PROPERTY_PRIVATE_KEY + "=");
            }
        }

        /// <summary>
        /// Directory to dump responses from Cryptsy into, as plain text.
        /// Normally left unset (null), in which case responses are not dumped.
        /// </summary>
        public DirectoryInfo DumpResponse { get; set; }
        public string PublicKey { get { return this.publicKey; } }
    }
}
