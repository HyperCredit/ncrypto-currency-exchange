using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Vircurex
{
    public class VircurexExchange : IExchange, ICoinDataSource<VircurexCurrency>, IMarketTradesSource, IExchangeWithTrading
    {
        public const string DEFAULT_BASE_URL = "https://api.vircurex.com/api/";

        public const string HEADER_SIGN = "sign";
        public const string HEADER_KEY = "key";

        // These values are used only for sending to Vircurex
        private const int ORDER_RELEASED = 1;
        private const int ORDER_UNRELEASED = 0;

        public const string PARAMETER_ACCOUNT = "account";
        public const string PARAMETER_AMOUNT = "amount";
        public const string PARAMETER_ALT = "alt";
        public const string PARAMETER_BASE = "base";
        public const string PARAMETER_ID = "id";
        public const string PARAMETER_ORDER_ID = "orderid";
        public const string PARAMETER_ORDER_TYPE = "ordertype";
        public const string PARAMETER_ORDER_RELEASED = "otype";
        public const string PARAMETER_SINCE = "since";
        public const string PARAMETER_TOKEN = "token";
        public const string PARAMETER_TIMESTAMP = "timestamp";
        public const string PARAMETER_UNIT_PRICE = "unitprice";

        private HttpClient client = new HttpClient();

        public VircurexExchange()
        {
            this.PrivateKeys = new Dictionary<Method, string>();
        }

        private void AssertResponseStatusSuccess(JObject response)
        {
            int status = response.Value<int>("status");

            if (0 == status)
            {
                return;
            }

            throw new VircurexResponseException("Response from Vircurex was not a success status. Received: "
                + response.Value<string>("statustext"));
        }

        /// <summary>
        /// Build the token for authenticating with Vircurex
        /// </summary>
        /// <param name="method">The method being called. Used as part of the message to
        /// be hashed, and to find the relevant private key.</param>
        /// <param name="parameters">The parameters to the request being sent to Vircurex.</param>
        /// <param name="timestamp"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public string BuildToken(Method method, List<KeyValuePair<string, string>> parameters,
            string timestamp, string id)
        {
            SHA256Managed hashstring = new SHA256Managed();
            byte[] bytes = Encoding.ASCII.GetBytes(BuildTokenMessage(method, parameters, timestamp, id));
            byte[] hash = hashstring.ComputeHash(bytes);

            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Build the message to be hashed, to generate the authentication token.
        /// </summary>
        /// <param name="method">The method being called. Used as part of the message to
        /// be hashed, and to find the relevant private key.</param>
        /// <param name="parameters">The parameters to the request being sent to Vircurex.</param>
        /// <param name="timestamp"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public string BuildTokenMessage(Method method,
            List<KeyValuePair<string, string>> parameters, string timestamp, string id)
        {
            string secret;
            string username = this.PublicKey;

            if (!this.PrivateKeys.TryGetValue(method, out secret))
            {
                secret = this.DefaultPrivateKey;
            }

            StringBuilder tokenMessageBuilder = new StringBuilder(secret + ";"
                + username + ";"
                + timestamp + ";"
                + id + ";");

            switch (method)
            {
                case Method.create_released_order:
                    // This is a special case, where i'
                    tokenMessageBuilder.Append(Enum.GetName(typeof(Method), Method.create_order));
                    break;
                default:
                    tokenMessageBuilder.Append(Enum.GetName(typeof(Method), method));
                    break;
            }

            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                if (method == Method.read_orders
                    && parameter.Key == PARAMETER_ORDER_RELEASED)
                {
                    // For whatever reason, read_orders doesn't include order type in its hash
                    continue;
                }
                
                tokenMessageBuilder.Append(";").Append(parameter.Value);
            }

            return tokenMessageBuilder.ToString();
        }

        public static string BuildUrl(Method method, Format format)
        {
            return DEFAULT_BASE_URL + Enum.GetName(typeof(Method), method)
                + "." + Enum.GetName(typeof(Format), format).ToLower();
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="method">The method to call on the Vircurex API</param>
        /// <returns>The raw JSON returned from Vircurex</returns>
        private async Task<T> CallPublic<T>(Method method)
            where T : JToken
        {
            return await CallPublic<T>(BuildUrl(method, Format.Json));
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="method">The method to call on the Vircurex API</param>
        /// <param name="quoteCurrencyCode">A quote currency code to append to the URL</param>
        /// <returns>The raw JSON returned from Vircurex</returns>
        private async Task<T> CallPublic<T>(Method method, string quoteCurrencyCode)
            where T : JToken
        {
            StringBuilder url = new StringBuilder(BuildUrl(method, Format.Json));
            url.Append("?");
            url.Append(PARAMETER_ALT).Append("=").Append(Uri.EscapeUriString(quoteCurrencyCode));

            return await CallPublic<T>(url.ToString());
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="method">The method to call on the Vircurex API</param>
        /// <param name="baseCurrencyCode">A base currency code to append to the URL</param>
        /// <param name="quoteCurrencyCode">A quote currency code to append to the URL</param>
        /// <returns>The raw JSON returned from Vircurex</returns>
        private async Task<T> CallPublic<T>(Method method,
            string baseCurrencyCode, string quoteCurrencyCode)
            where T : JToken
        {
            StringBuilder url = new StringBuilder(BuildUrl(method, Format.Json));

            url.Append("?");

            url.Append(PARAMETER_BASE).Append("=")
                .Append(Uri.EscapeUriString(baseCurrencyCode));
            url.Append("&").Append(PARAMETER_ALT)
                .Append("=").Append(Uri.EscapeUriString(quoteCurrencyCode));

            return await CallPublic<T>(url.ToString());
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API. This currently only works
        /// with the trades API, which returns arrays instead of an object.
        /// </summary>
        /// <param name="method">The method to call on the Vircurex API</param>
        /// <param name="baseCurrencyCode">A base currency code to append to the URL</param>
        /// <param name="quoteCurrencyCode">A quote currency code to append to the URL</param>
        /// <param name="since">An optional order ID to return trades since.</param>
        /// <returns>The raw JSON returned from Vircurex</returns>
        private async Task<T> CallPublic<T>(Method method, string baseCurrencyCode, string quoteCurrencyCode,
            VircurexOrderId since)
            where T : JToken
        {
            StringBuilder url = new StringBuilder(BuildUrl(method, Format.Json));

            url.Append("?");
            url.Append(PARAMETER_BASE).Append("=")
                .Append(Uri.EscapeUriString(baseCurrencyCode));

            url.Append("&").Append(PARAMETER_ALT)
                .Append("=").Append(Uri.EscapeUriString(quoteCurrencyCode));
            if (null != since)
            {
                url.Append("&").Append(PARAMETER_SINCE)
                    .Append("=").Append(Uri.EscapeUriString(since.ToString()));
            }

            return await CallPublic<T>(url.ToString());
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="url">Endpoint to make a request to</param>
        /// <returns>The raw JSON returned from Vircurex</returns>
        private async Task<T> CallPublic<T>(string url)
            where T : JToken
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);

                using (Stream jsonStream = await response.Content.ReadAsStreamAsync())
                {
                    using (StreamReader jsonStreamReader = new StreamReader(jsonStream))
                    {
                        using (JsonReader jsonReader = new JsonTextReader(jsonStreamReader))
                        {
                            JsonSerializer serializer = new JsonSerializer();

                            T result = serializer.Deserialize<T>(jsonReader);
                            JObject resultObj = result as JObject;

                            if (null != resultObj)
                            {
                                AssertResponseStatusSuccess(resultObj);
                            }

                            return result;
                        }
                    }
                }
            }
            catch (ArgumentException e)
            {
                throw new VircurexResponseException("Could not parse response from Vircurex.", e);
            }
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="method">The method to call on the Vircurex API</param>
        /// <returns>The raw JSON returned from Vircurex</returns>
        private async Task<T> CallPrivate<T>(Method method)
            where T : JToken
        {
            return await CallPrivate<T>(method, new List<KeyValuePair<string, string>>());
        }

        /// <summary>
        /// Make a call to a private API.
        /// </summary>
        /// <param name="method">The method to call on the Vircurex API</param>
        /// <param name="parameters">Parameters to send as part of the request.
        /// <returns>The raw JSON returned from Vircurex</returns>
        private async Task<T> CallPrivate<T>(Method method, List<KeyValuePair<string, string>> parameters)
            where T : JToken
        {
            StringBuilder url = new StringBuilder(BuildUrl(method, Format.Json));
            string timestamp = this.GetNextNonce();
            string id = timestamp + "." + DateTime.Now.Millisecond;
            string token = BuildToken(method, parameters, timestamp, id);

            url.Append("?")
                .Append(PARAMETER_ACCOUNT).Append("=")
                .Append(Uri.EscapeUriString(this.PublicKey)).Append("&")
                .Append(PARAMETER_ID).Append("=")
                .Append(Uri.EscapeUriString(id)).Append("&")
                .Append(PARAMETER_TOKEN).Append("=")
                .Append(Uri.EscapeUriString(token)).Append("&")
                .Append(PARAMETER_TIMESTAMP).Append("=")
                .Append(Uri.EscapeUriString(timestamp));

            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                url.Append("&").Append(Uri.EscapeUriString(parameter.Key))
                    .Append("=").Append(Uri.EscapeUriString(parameter.Value));
            }

            T result;

            try
            {
                HttpResponseMessage response = await client.GetAsync(url.ToString());

                using (Stream jsonStream = await response.Content.ReadAsStreamAsync())
                {
                    using (StreamReader jsonStreamReader = new StreamReader(jsonStream))
                    {
                        using (JsonReader jsonReader = new JsonTextReader(jsonStreamReader))
                        {
                            JsonSerializer serializer = new JsonSerializer();

                            result = serializer.Deserialize<T>(jsonReader);
                        }
                    }
                }
            }
            catch (ArgumentException e)
            {
                throw new VircurexResponseException("Could not parse response from Vircurex.", e);
            }

            JObject resultObj = result as JObject;

            if (null != resultObj)
            {
                AssertResponseStatusSuccess(resultObj);
            }

            // Dump out a copy for reference
            string fileName = System.IO.Path.GetTempFileName();
            using (StreamWriter writer = new StreamWriter(new FileStream(fileName, FileMode.Create)))
            {
                writer.Write(result.ToString());
            }

            return result;
        }

        public async Task CancelOrder(OrderId orderId)
        {
            await CancelOrder(orderId, OrderReleased.Released);
        }

        public async Task CancelOrder(OrderId orderId, OrderReleased orderReleased)
        {
            VircurexOrderId vircurexOrderId = (VircurexOrderId)orderId;
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(PARAMETER_ORDER_ID, vircurexOrderId.Value.ToString()),
                new KeyValuePair<string, string>(PARAMETER_ORDER_RELEASED, orderReleased == OrderReleased.Unreleased
                    ? ORDER_UNRELEASED.ToString()
                    : ORDER_RELEASED.ToString())
            };

            JObject response = await CallPrivate<JObject>(Method.delete_order, parameters);
        }

        public async Task<OrderId>
            CreateOrder(MarketId marketId, OrderType orderType, decimal quantity, decimal price)
        {
            VircurexMarketId vircurexMarketId = (VircurexMarketId)marketId;
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(PARAMETER_ORDER_TYPE, Enum.GetName(typeof(OrderType), orderType).ToUpper()),
                new KeyValuePair<string, string>(PARAMETER_AMOUNT, quantity.ToString()),
                new KeyValuePair<string, string>("currency1", vircurexMarketId.BaseCurrencyCode),
                new KeyValuePair<string, string>(PARAMETER_UNIT_PRICE, price.ToString()),
                new KeyValuePair<string, string>("currency2", vircurexMarketId.QuoteCurrencyCode)
            };

            JObject response = await CallPrivate<JObject>(Method.create_released_order, parameters);

            return new VircurexOrderId(response.Value<int>("orderid"));
        }

        /// <summary>
        /// Create a new order, but do not release it for trading.
        /// </summary>
        /// <param name="marketId"></param>
        /// <param name="orderType"></param>
        /// <param name="quantity"></param>
        /// <param name="price"></param>
        /// <returns>The unreleased order ID. Note that this IS NOT the same as the
        /// released order ID.</returns>
        public async Task<VircurexOrderId>
            CreateUnreleasedOrder(MarketId marketId, OrderType orderType, decimal quantity, decimal price)
        {
            VircurexMarketId vircurexMarketId = (VircurexMarketId)marketId;
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(PARAMETER_ORDER_TYPE, Enum.GetName(typeof(OrderType), orderType).ToUpper()),
                new KeyValuePair<string, string>(PARAMETER_AMOUNT, quantity.ToString()),
                new KeyValuePair<string, string>("currency1", vircurexMarketId.BaseCurrencyCode),
                new KeyValuePair<string, string>(PARAMETER_UNIT_PRICE, price.ToString()),
                new KeyValuePair<string, string>("currency2", vircurexMarketId.QuoteCurrencyCode)
            };

            JObject response = await CallPrivate<JObject>(Method.create_order, parameters);

            return new VircurexOrderId(response.Value<int>("orderid"));
        }

        public void Dispose()
        {
            client.Dispose();
        }

        /// <summary>
        /// Formats a date and time in a manner suitable for use with Vircurex.
        /// </summary>
        /// <param name="dateTimeUtc">A date and time, must be in the UTC timezone.</param>
        /// <returns>A formatted string</returns>
        public string FormatDateTime(DateTime dateTimeUtc)
        {
            return dateTimeUtc.ToString("s");
        }

        public async Task<Model.AccountInfo> GetAccountInfo()
        {
            return VircurexParsers.ParseAccountInfo(await CallPrivate<JObject>(Method.get_balances));
        }

        public async Task<List<VircurexCurrency>> GetCoins()
        {
            return VircurexCurrency.Parse(await CallPublic<JObject>(Method.get_currency_info));
        }

        public async Task<List<Market>> GetMarkets()
        {
            JObject marketsJson = await CallPublic<JObject>(Method.get_info_for_currency);

            return VircurexMarket.ParseMarkets(marketsJson);
        }

        public async Task<Dictionary<MarketId, Book>> GetMarketOrdersAlt(string quoteCurrencyCode)
        {
            return VircurexParsers.ParseMarketOrdersAlt(quoteCurrencyCode,
                await CallPublic<JObject>(Method.orderbook_alt, quoteCurrencyCode));
        }

        public async Task<List<MarketTrade>> GetMarketTrades(MarketId marketId)
        {
            VircurexMarketId vircurexMarketId = (VircurexMarketId)marketId;

            return VircurexParsers.ParseMarketTrades(marketId,
                await CallPublic<JArray>(Method.trades,
                    vircurexMarketId.BaseCurrencyCode, vircurexMarketId.QuoteCurrencyCode, null));
        }

        public async Task<List<MarketTrade>> GetMarketTrades(VircurexMarketId vircurexMarketId, VircurexOrderId since)
        {
            return VircurexParsers.ParseMarketTrades(vircurexMarketId,
                await CallPublic<JArray>(Method.trades,
                    vircurexMarketId.BaseCurrencyCode, vircurexMarketId.QuoteCurrencyCode, since));
        }

        public async Task<List<MyOrder>> GetMyActiveOrders(OrderReleased orderReleased)
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(PARAMETER_ORDER_RELEASED,
                    orderReleased == OrderReleased.Unreleased
                        ? ORDER_UNRELEASED.ToString()
                        : ORDER_RELEASED.ToString())
            };

            return VircurexParsers.ParseMyActiveOrders(await CallPrivate<JObject>(Method.read_orders, parameters));
        }

        public async Task<List<MyOrder>> GetMyActiveOrders(MarketId marketId, int? limit)
        {
            return (await GetMyActiveOrders(OrderReleased.Released))
                .Where(order => order.MarketId.Equals(marketId))
                .ToList();
        }

        public async Task<Model.Book> GetMarketDepth(MarketId marketId)
        {
            VircurexMarketId vircurexMarketId = (VircurexMarketId)marketId;

            return VircurexParsers.ParseOrderBook(await CallPublic<JObject>(Method.orderbook,
                vircurexMarketId.BaseCurrencyCode, vircurexMarketId.QuoteCurrencyCode));
        }

        public async Task<List<MyTrade>> GetOrderExecutions(OrderId orderId)
        {
            VircurexOrderId vircurexOrderId = (VircurexOrderId)orderId;
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(PARAMETER_ORDER_ID, vircurexOrderId.Value.ToString())
            };

            JObject response = await CallPrivate<JObject>(Method.read_orderexecutions, parameters);

            return VircurexParsers.ParseOrderExecutions(response);
        }

        public string GetNextNonce()
        {
            return FormatDateTime(DateTime.Now.ToUniversalTime());
        }

        public async Task<VircurexOrderId> ReleaseOrder(VircurexOrderId unreleasedOrderId)
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(PARAMETER_ORDER_ID, unreleasedOrderId.Value.ToString())
            };

            JObject response = await CallPrivate<JObject>(Method.release_order, parameters);

            return new VircurexOrderId(response.Value<int>("orderid"));
        }

        public void SetApiKey(Method method, string key)
        {
            this.PrivateKeys[method] = key;
        }
        
        public string SignHeader
        {
            get { return HEADER_SIGN; }
        }
        public string KeyHeader
        {
            get { return HEADER_KEY; }
        }
        public string Label
        {
            get { return "Vircurex"; }
        }

        public enum Format
        {
            Json,
            Xml
        }

        public enum Method
        {
            create_order,
            create_released_order,
            delete_order,
            get_balance,
            get_balances,
            get_currency_info,
            get_info_for_currency,
            orderbook_alt,
            orderbook,
            read_order,
            read_orders,
            read_orderexecutions,
            release_order,
            trades
        }

        public enum OrderReleased
        {
            Unreleased,
            Released
        }

        public string PublicKey { get; set; }
        /// <summary>
        /// Default secret to use for API calls.
        /// </summary>
        public string DefaultPrivateKey { get; set; }
        /// <summary>
        /// Secrets to use when making API calls. If these are all the same,
        /// you can use DefaultPrivateKey instead of specifying them for each
        /// method.
        /// </summary>
        public Dictionary<Method, string> PrivateKeys { get; private set; }
    }
}
