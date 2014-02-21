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
using Newtonsoft.Json;

namespace Lostics.NCryptoExchange.Kraken
{
    /// <summary>
    /// Wrapper around the Kraken (https://www.cryptsy.com/) API.
    /// 
    /// To use, requires a public and private key (these can be generated from the
    /// "Settings" page within Kraken, once logged in).
    /// </summary>
    public class KrakenExchange : AbstractSha512Exchange, IMarketTradesSource, IExchangeWithTrading
    {
        public const string HEADER_SIGN = "Sign";
        public const string HEADER_KEY = "Key";

        public const string PARAMETER_CURRENCY_CODE = "currencycode";
        public const string PARAMETER_MARKET_ID = "marketid";
        public const string PARAMETER_LIMIT = "limit";
        public const string PARAMETER_NONCE = "nonce";
        public const string PARAMETER_ORDER_ID = "orderid";
        public const string PARAMETER_ORDER_TYPE = "ordertype";
        public const string PARAMETER_PRICE = "price";
        public const string PARAMETER_QUANTITY = "quantity";

        private readonly TimeZoneInfo defaultTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        private HttpClient client = new HttpClient();
        private DirectoryInfo dumpResponse = null;
        private readonly string publicUrl = "https://api.kraken.com/0/public";
        private readonly string privateUrl = "https://api.kraken.com/0/private";

        /// <summary>
        /// Asserts that the response sent by Kraken indicates success, and throws a relevant
        /// exception otherwise.
        /// </summary>
        /// <param name="cryptsyResponse">Response from Kraken as JSON</param>
        private static void AssertResponseIsSuccess(JObject cryptsyResponse)
        {
            string success = cryptsyResponse.Value<string>("success");

            if (null == success)
            {
                throw new KrakenResponseException("No success value returned in response from Kraken.");
            }

            if (!(success.Equals("1")))
            {
                string errorMessage = cryptsyResponse.Value<string>("error");

                if (null == errorMessage)
                {
                    throw new KrakenFailureException("Error response returned from Kraken.");
                }
                else
                {
                    throw new KrakenFailureException(errorMessage);
                }
            }
        }

        /// <summary>
        /// Make a call to a private API method.
        /// </summary>
        /// <param name="method">The method to call on the Kraken API</param>
        /// <returns>Parsed JSON returned from Kraken</returns>
        private async Task<T> CallPrivate<T>(PrivateMethod method)
            where T : JToken
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>(PARAMETER_NONCE, this.GetNextNonce())
            });

            return await CallPrivate<T>(method, request);
        }

        /// <summary>
        /// Make a call to a private API method.
        /// </summary>
        /// <param name="method">The method to call on the Kraken API</param>
        /// <returns>Parsed JSON returned from Kraken</returns>
        private async Task<T> CallPrivate<T>(PrivateMethod method, Dictionary<string, string> parameterDictionary)
            where T : JToken
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();

            foreach (string parameterName in parameterDictionary.Keys)
            {
                parameters.Add(new KeyValuePair<string, string>(parameterName, parameterDictionary[parameterName]));
            }

            FormUrlEncodedContent request = new FormUrlEncodedContent(parameters);

            return await CallPrivate<T>(method, request);
        }

        /// <summary>
        /// Make a call to a private API method.
        /// </summary>
        /// <param name="method">The method to call on the Kraken API</param>
        /// <param name="request">A request, containing the POST parameters. Authentication headers
        /// will be added to this.</param>
        /// <returns>The JSON data object returned from Kraken</returns>
        private async Task<T> CallPrivate<T>(PrivateMethod method, FormUrlEncodedContent request)
            where T : JToken
        {
            string url = this.privateUrl + Enum.GetName(typeof(PrivateMethod), method);

            request.Headers.Add(this.SignHeader, GenerateSHA512Signature(request));
            request.Headers.Add(this.KeyHeader, this.PublicKey);

            try
            {
                JObject resultJson;
                HttpResponseMessage result = await client.PostAsync(url, request);

                using (Stream jsonStream = await result.Content.ReadAsStreamAsync())
                {
                    using (StreamReader jsonStreamReader = new StreamReader(jsonStream))
                    {
                        using (JsonReader jsonReader = new JsonTextReader(jsonStreamReader))
                        {
                            JsonSerializer serializer = new JsonSerializer();

                            resultJson = serializer.Deserialize<JObject>(jsonReader);
                        }
                    }
                }

                AssertResponseIsSuccess(resultJson);

                return resultJson.Value<T>("data");
            }
            catch (ArgumentException e)
            {
                throw new KrakenResponseException("Could not parse response from Kraken.", e);
            }
        }

        public async Task CancelOrder(OrderId orderId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { PARAMETER_ORDER_ID, orderId.ToString() }
            };
            JObject response = await CallPrivate<JObject>(PrivateMethod.CancelOrder, parameters);

            // TODO: Check the response.

            return;
        }

        public async Task<OrderId> CreateOrder(MarketId marketId,
                OrderType orderType, decimal quantity,
                decimal price)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            this.client.Dispose();
        }

        public async override Task<AccountInfo> GetAccountInfo()
        {
            throw new NotImplementedException();
        }

        public override string GetNextNonce()
        {
            return DateTime.Now.Ticks.ToString();
        }

        public async Task<Book> GetMarketOrders(MarketId marketId)
        {
            throw new NotImplementedException();
        }

        public async override Task<List<Market>> GetMarkets()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Transaction>> GetMyTransactions()
        {
            throw new NotImplementedException();
        }

        public async Task<List<MarketTrade>> GetMarketTrades(MarketId marketId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<MyTrade>> GetMyTrades(MarketId marketId, int? limit)
        {
            throw new NotImplementedException();
        }

        public async override Task<List<MyOrder>> GetMyActiveOrders(MarketId marketId, int? limit)
        {
            throw new NotImplementedException();
        }

        public async override Task<Book> GetMarketDepth(MarketId marketId)
        {
            throw new NotImplementedException();
        }

        public enum PublicMethod
        {
            Assets,
            AssetPairs,
            Depth,
            Spread,
            Ticker,
            Time,
            Trades
        }

        public enum PrivateMethod
        {
            AddOrder,
            Balance,
            CancelOrder,
            ClosedOrders,
            Ledgers,
            OpenOrders,
            OpenPositions,
            QueryLedgers,
            QueryOrders,
            QueryTrades,
            TradeBalance,
            TradeVolume,
            TradesHistory,
        }

        /// <summary>
        /// Directory to dump responses from Kraken into, as plain text.
        /// Normally left unset (null), in which case responses are not dumped.
        /// </summary>
        public DirectoryInfo DumpResponse { get; set; }
        public override string Label
        {
            get { return "Kraken"; }
        }
        public override string SignHeader
        {
            get { return HEADER_SIGN; }
        }
        public override string KeyHeader
        {
            get { return HEADER_KEY; }
        }
    }
}
