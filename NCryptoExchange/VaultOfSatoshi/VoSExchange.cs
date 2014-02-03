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

namespace Lostics.NCryptoExchange.VaultOfSatoshi
{
    public class VoSExchange : IExchange, ICoinDataSource<VoSCurrency>, ITrading
    {
        public const string HEADER_SIGN = "Api-Sign";
        public const string HEADER_KEY = "Api-Key";

        public const string DEFAULT_BASE_URL = "https://api.vaultofsatoshi.com/";
        public const string PRIVATE_END_POINT = "info/";
        public const string PUBLIC_END_POINT = "public/";
        public const string DEFAULT_PUBLIC_URL = DEFAULT_BASE_URL + PUBLIC_END_POINT;
        public const string DEFAULT_PRIVATE_URL = DEFAULT_BASE_URL + PRIVATE_END_POINT;

        public const string PARAMETER_NONCE = "nonce";

        private HttpClient client = new HttpClient();

        public VoSExchange()
        {
        }

        public static string BuildPublicUrl(Method method)
        {
            return DEFAULT_PUBLIC_URL + Enum.GetName(typeof(Method), method);
        }

        public static string BuildPrivateUrl(Method method)
        {
            return DEFAULT_PRIVATE_URL + Enum.GetName(typeof(Method), method);
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="method">The method to call on the VoS API</param>
        /// <returns>The raw JSON returned from VoS</returns>
        private async Task<T> CallPublic<T>(Method method)
            where T : JToken
        {
            return (T)JToken.Parse(await CallPublic(BuildPublicUrl(method)));
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="method">The method to call on the VoS API</param>
        /// <param name="quoteCurrencyCode">A quote currency code to append to the URL</param>
        /// <returns>The raw JSON returned from VoS</returns>
        private async Task<T> CallPublic<T>(Method method, VoSMarketId marketId)
            where T : JToken
        {
            StringBuilder url = new StringBuilder(BuildPublicUrl(method));

            url.Append("?").Append(marketId.BaseCurrencyCodeParameter).Append("&")
                .Append(marketId.QuoteCurrencyCodeParameter);

            return (T)JToken.Parse(await CallPublic(url.ToString()));
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="url">Endpoint to make a request to</param>
        /// <returns>The raw JSON returned from VoS</returns>
        private async Task<string> CallPublic(string url)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                using (Stream jsonStream = await response.Content.ReadAsStreamAsync())
                {
                    using (StreamReader jsonStreamReader = new StreamReader(jsonStream))
                    {
                        return await jsonStreamReader.ReadToEndAsync();
                    }
                }
            }
            catch (ArgumentException e)
            {
                throw new VoSResponseException("Could not parse response from VoS.", e);
            }
        }

        /// <summary>
        /// Make a call to a private API method.
        /// </summary>
        /// <param name="method">The method to call on the VoS API</param>
        /// <returns>Parsed JSON returned from VoS</returns>
        private async Task<JObject> CallPrivate(Method method)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[]
            {
                GenerateNonceParameter()
            });

            return await CallPrivate(method, request);
        }

        /// <summary>
        /// Make a call to a private API method.
        /// </summary>
        /// <param name="method">The method to call on the VoS API</param>
        /// <param name="quoteCurrencyCode">A quote currency code to append to the URL</param>
        /// <returns>Parsed JSON returned from VoS</returns>
        private async Task<JObject> CallPrivate(Method method, VoSMarketId marketId)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>(PARAMETER_NONCE, this.GetNextNonce()),
                marketId.BaseCurrencyCodeKeyValuePair,
                marketId.QuoteCurrencyCodeKeyValuePair
            });

            return await CallPrivate(method, request);
        }

        /// <summary>
        /// Make a call to a private API method.
        /// </summary>
        /// <param name="method">The method to call on the VoS API</param>
        /// <param name="quoteCurrencyCode">A quote currency code to append to the URL</param>
        /// <returns>Parsed JSON returned from VoS</returns>
        private async Task<JObject> CallPrivate(Method method, VoSOrderId orderId)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>(PARAMETER_NONCE, this.GetNextNonce()),
                orderId.KeyValuePair
            });

            return await CallPrivate(method, request);
        }

        /// <summary>
        /// Make a call to a private API method.
        /// </summary>
        /// <param name="method">The method to call on the VoS API</param>
        /// <param name="request">A request, containing the POST parameters. Authentication headers
        /// will be added to this.</param>
        /// <returns>The raw JSON returned from VoS</returns>
        private async Task<JObject> CallPrivate(Method method, FormUrlEncodedContent request)
        {
            string url = DEFAULT_PRIVATE_URL + Enum.GetName(typeof(Method), method);

            request.Headers.Add(this.SignHeader, GenerateSHA512Signature(method, request));
            request.Headers.Add(this.KeyHeader, this.PublicKey);

            try
            {
                HttpResponseMessage response = await client.PostAsync(url, request);
                using (Stream jsonStream = await response.Content.ReadAsStreamAsync())
                {
                    using (StreamReader jsonStreamReader = new StreamReader(jsonStream))
                    {
                        string responseContent = await jsonStreamReader.ReadToEndAsync();

                        using (StreamWriter contentWriter = new StreamWriter(new FileStream(System.IO.Path.GetTempFileName(), FileMode.Create)))
                        {
                            contentWriter.Write(responseContent);
                        }

                        JObject responseJson = JObject.Parse(responseContent);
                        string status = responseJson.Value<string>("status");

                        if (status == "error")
                        {
                            throw new VoSResponseException(responseJson.Value<string>("message"));
                        }

                        if (status != "success")
                        {
                            throw new VoSResponseException("Response from Vault of Satoshi was not a success status. Received: "
                                + status);
                        }

                        return responseJson;
                    }
                }
            }
            catch (ArgumentException e)
            {
                throw new VoSResponseException("Could not parse response from VoS.", e);
            }
        }

        public void Dispose()
        {
            client.Dispose();
        }

        /// <summary>
        /// Formats a date and time in a manner suitable for use with VoS.
        /// </summary>
        /// <param name="dateTimeUtc">A date and time, must be in the UTC timezone.</param>
        /// <returns>A formatted string</returns>
        public string FormatDateTime(DateTime dateTimeUtc)
        {
            return dateTimeUtc.ToString("s");
        }

        private KeyValuePair<string, string> GenerateNonceParameter()
        {
            return new KeyValuePair<string, string>(PARAMETER_NONCE, this.GetNextNonce());
        }

        public async Task<AccountInfo> GetAccountInfo()
        {
            JObject response = await this.CallPrivate(Method.account);

            return VoSAccountInfo.Parse(response.Value<JObject>("data"));
        }

        public async Task<List<VoSCurrency>> GetCoins()
        {
            JObject response = await this.CallPrivate(Method.currency);

            return response
                .Value<JArray>("data")
                .Select(coin => VoSCurrency.Parse((JObject)coin)).ToList();
        }

        public async Task<List<Market>> GetMarkets()
        {
            List<VoSCurrency> currencies = await GetCoins();
            IEnumerable<VoSCurrency> tradeableCurrencies = currencies.Where(currency => currency.Tradeable);
            List<Market> markets = new List<Market>();

            foreach (VoSCurrency baseCurrency in tradeableCurrencies.Where(currency => !currency.Virtual))
            {
                foreach (VoSCurrency quoteCurrency in tradeableCurrencies.Where(currency => currency.Virtual))
                {
                    VoSMarketId marketId = new VoSMarketId(baseCurrency.CurrencyCode, quoteCurrency.CurrencyCode);
                    markets.Add(new VoSMarket(marketId));
                }
            }

            return markets;
        }

        public async Task<List<MyOrder>> GetMyActiveOrders(MarketId marketId, int? limit)
        {
            return (await this.GetMyOrders(limit, (DateTime?)null, true))
                .Where(order => order.MarketId.Equals(marketId))
                .ToList();
        }

        public async Task<List<MyOrder>> GetMyOrders(int? limit, DateTime? after, bool openOnly)
        {
            List<KeyValuePair<string, string>> kvPairs = new List<KeyValuePair<string, string>>();

            kvPairs.Add(GenerateNonceParameter());
            if (null != limit)
            {
                kvPairs.Add(new KeyValuePair<string, string>("limit", limit.ToString()));
            }
            if (null != after)
            {
                kvPairs.Add(new KeyValuePair<string, string>("after", after.Value.Ticks.ToString()));
            }
            if (openOnly)
            {
                kvPairs.Add(new KeyValuePair<string, string>("openOnly", openOnly.ToString()));
            }

            JObject response = await CallPrivate(Method.orders, new FormUrlEncodedContent(kvPairs));

            return VoSMyOrder.Parse(response.Value<JArray>("data"));
        }

        public async Task<Book> GetMarketDepth(MarketId marketId)
        {
            JObject jsonObj = await this.CallPublic<JObject>(Method.orderbook, (VoSMarketId)marketId);
            return VoSParsers.ParseOrderBook(jsonObj.Value<JObject>("data"));
        }

        public async Task CancelOrder(OrderId orderId)
        {
            JObject response = await CallPrivate(Method.cancel, (VoSOrderId)orderId);

            // TODO: Check the respnse.

            return;
        }

        public async Task<OrderId> CreateOrder(MarketId marketId, OrderType orderType, decimal quantity, decimal price)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Vault of Satoshi have their own unique way of signing requests, where the message
        /// content is the URL, then null, then the parameters.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public string GenerateSHA512Signature(Method method, FormUrlEncodedContent request)
        {
            HMAC digester = new HMACSHA512(this.PrivateKeyBytes);
            byte[] message = GenerateMessageToSign(method, request);

            // Yes, VoS really has us encode the digest into hex the base64 the
            // hex.
            string digestedMessageHex = BitConverter.ToString(digester.ComputeHash(message)).Replace("-", "").ToLower();

            return System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(digestedMessageHex));
        }

        /// <summary>
        /// Builds the message to be signed with the private key, based on the given
        /// API method and request.
        /// </summary>
        /// <param name="method">The API method being called.</param>
        /// <param name="request">The parameters to be sent to the server; must
        /// include the nonce.</param>
        /// <returns></returns>
        public static byte[] GenerateMessageToSign(Method method, FormUrlEncodedContent request)
        {
            string endpoint = "/" + PRIVATE_END_POINT + Enum.GetName(typeof(Method), method);

            StringBuilder hex = new StringBuilder();
            byte[] endpointBytes = System.Text.Encoding.ASCII.GetBytes(endpoint);
            byte[] requestBytes = System.Text.Encoding.ASCII.GetBytes(request.ReadAsStringAsync().Result);
            byte[] message = new byte[endpointBytes.Length + requestBytes.Length + 1];
            int messageIdx;

            for (messageIdx = 0; messageIdx < endpointBytes.Length; messageIdx++)
            {
                message[messageIdx] = endpointBytes[messageIdx];
            }
            message[messageIdx++] = 0;
            for (int requestIdx = 0; requestIdx < requestBytes.Length; requestIdx++, messageIdx++)
            {
                message[messageIdx] = requestBytes[requestIdx];
            }

            return message;
        }

        public string GetNextNonce()
        {
            DateTime epoch = new DateTime(1970, 1, 1);

            // Have to use number of microseconds since the Epoch, so
            // need to divide by 10 to get from ticks to micros
            return ((DateTime.Now.Ticks - epoch.Ticks) / 10).ToString();
        }

        public string Label
        {
            get { return "Vault of Satoshi"; }
        }
        public string SignHeader
        {
            get { return HEADER_SIGN; }
        }
        public string KeyHeader
        {
            get { return HEADER_KEY; }
        }

        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public byte[] PrivateKeyBytes
        {
            get
            {
                return System.Text.Encoding.ASCII.GetBytes(this.PrivateKey);
            }
        }

        public enum Method
        {
            currency,
            account,
            balance,
            wallet_address,
            wallet_history,
            quote,
            orderbook,
            orders,
            order_detail,
            ticker,
            place,
            cancel
        }
    }
}
