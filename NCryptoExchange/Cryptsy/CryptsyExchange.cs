using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public class CryptsyExchange : AbstractExchange<CryptsyMarketId, CryptsyOrderId>
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
        /// Asserts that the response sent by Cryptsy indicates success, and throws a relevant
        /// exception otherwise.
        /// </summary>
        /// <param name="cryptsyResponse">Response from Cryptsy as JSON</param>
        private static void AssertResponseIsSuccess(JObject cryptsyResponse)
        {
            string success = cryptsyResponse.Value<string>("success");

            if (null == success)
            {
                throw new CryptsyResponseException("No success value returned in response from Cryptsy.");
            }

            if (!(success.Equals("1")))
            {
                string errorMessage = cryptsyResponse.Value<string>("error");

                if (null == errorMessage)
                {
                    throw new CryptsyFailureException("Error response returned from Cryptsy.");
                }
                else
                {
                    throw new CryptsyFailureException(errorMessage);
                }
            }
        }

        /// <summary>
        /// Make a call to Cryptsy's private API, where no return value is expected.
        /// </summary>
        /// <param name="request">The request to send to Cryptsy</param>
        /// <returns></returns>
        private async Task Call(FormUrlEncodedContent request)
        {
            request.Headers.Add(HEADER_SIGN, GenerateSHA512Signature(request, this.privateKey));
            request.Headers.Add(HEADER_KEY, this.publicKey);

            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            await ParseResponseToJson(response);
        }

        /// <summary>
        /// Make a call to Cryptsy's private API
        /// </summary>
        /// <param name="request">The request to send to Cryptsy</param>
        /// <returns>The return from Cryptsy as a JSON token</returns>
        private async Task<T> Call<T>(FormUrlEncodedContent request)
        {
            request.Headers.Add(HEADER_SIGN, GenerateSHA512Signature(request, this.privateKey));
            request.Headers.Add(HEADER_KEY, this.publicKey);

            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            return await ParseResponseToJson<T>(response);
        }

        public async override Task CancelOrder(CryptsyOrderId orderId)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateParameters(CryptsyMethod.cancelorder,
                orderId, (CryptsyMarketId)null, null));

            await Call(request);
        }

        public async Task CancelAllOrders()
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
            JObject returnObj = await Call<JObject>(request);

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
            JObject returnObj = await Call<JObject>(request);

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
            JObject returnObj = await Call<JObject>(request);

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

        public async override Task<AccountInfo> GetAccountInfo()
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateParameters(CryptsyMethod.getinfo,
                (CryptsyOrderId)null, (CryptsyMarketId)null, null));

            return CryptsyAccountInfo.Parse(await Call<JObject>(request));
        }

        public static CryptsyExchange GetExchange(FileInfo configurationFile)
        {
            if (!configurationFile.Exists)
            {
                WriteDefaultConfigurationFile(configurationFile);
                throw new ConfigurationException("No configuration file exists; blank default created. "
                    + "Please enter public and private key values and try again.");
            }

            Dictionary<string, string> properties = GetConfiguration(configurationFile);
            string publicKey = properties[PROPERTY_PUBLIC_KEY];
            string privateKey = properties[PROPERTY_PRIVATE_KEY];

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

        public override async Task<Book> GetMarketOrders(CryptsyMarketId marketId)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateParameters(CryptsyMethod.marketorders,
                (CryptsyOrderId)null, marketId, null));
            JObject marketOrdersJson = await Call<JObject>(request);

            return CryptsyParsers.ParseMarketOrders(marketOrdersJson);
        }

        public async override Task<List<Market<CryptsyMarketId>>> GetMarkets()
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateParameters(CryptsyMethod.getmarkets,
                (CryptsyOrderId)null, (CryptsyMarketId)null, null));
            JArray marketsJson = await Call<JArray>(request);

            return marketsJson.Select(
                market => (Market<CryptsyMarketId>)CryptsyMarket.Parse(market as JObject, this.defaultTimeZone)
            ).ToList();
        }

        public async Task<List<Transaction>> GetMyTransactions()
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateParameters(CryptsyMethod.mytransactions,
                (CryptsyOrderId)null, (CryptsyMarketId)null, null));
            JArray returnArray = await Call<JArray>(request);

            return CryptsyParsers.ParseTransactions(returnArray);
        }

        public async override Task<List<MarketTrade<CryptsyMarketId>>> GetMarketTrades(CryptsyMarketId marketId)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateParameters(CryptsyMethod.markettrades,
                (CryptsyOrderId)null, marketId, null));
            JArray marketTradesJson = await Call<JArray>(request);

            return marketTradesJson.Select(
                marketTrade => (MarketTrade<CryptsyMarketId>)CryptsyMarketTrade.Parse(marketTrade as JObject, marketId, this.defaultTimeZone)
            ).ToList();
        }

        public async override Task<List<MyTrade<CryptsyMarketId, CryptsyOrderId>>> GetMyTrades(CryptsyMarketId marketId, int? limit)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateParameters(CryptsyMethod.mytrades,
                (CryptsyOrderId)null, marketId, limit));
            JArray myTradesJson = await Call<JArray>(request);

            // XXX: Should use timezone provided by Cryptsy, not just presume.

            return myTradesJson.Select(myTradeJson => CryptsyParsers.ParseMyTrade(myTradeJson as JObject, marketId,
                defaultTimeZone)).ToList();
        }

        public async override Task<List<MyTrade<CryptsyMarketId, CryptsyOrderId>>> GetAllMyTrades(int? limit)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateParameters(CryptsyMethod.allmytrades,
                (CryptsyOrderId)null, (CryptsyMarketId)null, limit));
            JArray myTradesJson = await Call<JArray>(request);

            // XXX: Should use timezone provided by Cryptsy, not just presume.

            return myTradesJson.Select(myTradeJson => CryptsyParsers.ParseMyTrade(myTradeJson as JObject, null,
                defaultTimeZone)).ToList();
        }

        public async override Task<List<MyOrder<CryptsyMarketId, CryptsyOrderId>>> GetMyActiveOrders(CryptsyMarketId marketId, int? limit)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateParameters(CryptsyMethod.myorders,
                (CryptsyOrderId)null, marketId, limit));
            JArray myOrdersJson = await Call<JArray>(request);

            // XXX: Should use timezone provided by Cryptsy, not just presume it's EST

            return myOrdersJson.Select(myOrderJson => CryptsyParsers.ParseMyOrder(myOrderJson as JObject, marketId, defaultTimeZone)).ToList();
        }

        public async Task<List<MyOrder<CryptsyMarketId, CryptsyOrderId>>> GetAllMyActiveOrders(int? limit)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateParameters(CryptsyMethod.allmyorders,
                (CryptsyOrderId)null, (CryptsyMarketId)null, limit));
            JArray myOrdersJson = await Call<JArray>(request);

            // XXX: Should use timezone provided by Cryptsy, not just presume it's EST

            return myOrdersJson.Select(myOrderJson => CryptsyParsers.ParseMyOrder(myOrderJson as JObject, defaultTimeZone)).ToList();
        }

        public async override Task<Book> GetMarketDepth(CryptsyMarketId marketId)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(GenerateParameters(CryptsyMethod.depth,
                (CryptsyOrderId)null, marketId, null));
            JObject returnObj = await Call<JObject>(request);

            return CryptsyParsers.ParseMarketDepthBook(returnObj, marketId);
        }

        /// <summary>
        /// Gets the "return" property from the response from Cryptsy, and returns
        /// it as a JToken. In case of an error response, throws a suitable Exception
        /// instead.
        /// </summary>
        /// <param name="response">The HTTP response to read from</param>
        /// <returns>The returned content from Cryptsy as a JToken.</returns>
        /// <exception cref="IOException">Where there was a problem reading the
        /// response from Cryptsy.</exception>
        /// <exception cref="CryptsyFailureException">Where Cryptsy returned an error.</exception>
        /// <exception cref="CryptsyResponseException">Where there was a problem
        /// parsing the response from Cryptsy.</exception>
        private async Task<T> ParseResponseToJson<T>(HttpResponseMessage response)
        {
            JObject jsonObj;

            try
            {
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
                throw new CryptsyResponseException("Could not parse response from Cryptsy.", e);
            }

            // Log the response from Cryptsy if configured to do so
            if (null != this.DumpResponse)
            {
                WriteResponseToFile(jsonObj);
            }

            AssertResponseIsSuccess(jsonObj);

            return jsonObj.Value<T>("return");
        }

        /// <summary>
        /// Gets the "return" property from the response from Cryptsy, and returns
        /// it as a JToken. In case of an error response, throws a suitable Exception
        /// instead.
        /// </summary>
        /// <param name="response">The HTTP response to read from</param>
        /// <exception cref="IOException">Where there was a problem reading the
        /// response from Cryptsy.</exception>
        /// <exception cref="CryptsyFailureException">Where Cryptsy returned an error.</exception>
        /// <exception cref="CryptsyResponseException">Where there was a problem
        /// parsing the response from Cryptsy.</exception>
        private async Task ParseResponseToJson(HttpResponseMessage response)
        {
            JObject jsonObj;

            try
            {
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
                throw new CryptsyResponseException("Could not parse response from Cryptsy.", e);
            }

            // Log the response from Cryptsy if configured to do so
            if (null != this.DumpResponse)
            {
                WriteResponseToFile(jsonObj);
            }

            AssertResponseIsSuccess(jsonObj);
        }

        private void WriteResponseToFile(JToken jsonObj)
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
        public override string Label
        {
            get { return "Cryptsy"; }
        }
    }
}
