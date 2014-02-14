using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Vircurex
{
    public class VircurexExchange : IExchange, ICoinDataSource<VircurexCurrency>, IMarketTradesSource, ITrading
    {
        public const string DEFAULT_BASE_URL = "https://api.vircurex.com/api/";

        public const string HEADER_SIGN = "sign";
        public const string HEADER_KEY = "key";

        public const string PARAM_BASE = "base";
        public const string PARAM_ALT = "alt";
        public const string PARAM_SINCE = "since";

        private Dictionary<Method, string> privateKeys = new Dictionary<Method, string>();

        private HttpClient client = new HttpClient();

        public VircurexExchange()
        {
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
            url.Append(PARAM_ALT).Append("=").Append(Uri.EscapeUriString(quoteCurrencyCode));

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

            url.Append(PARAM_BASE).Append("=")
                .Append(Uri.EscapeUriString(baseCurrencyCode));
            url.Append("&").Append(PARAM_ALT)
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
            url.Append(PARAM_BASE).Append("=")
                .Append(Uri.EscapeUriString(baseCurrencyCode));

            url.Append("&").Append(PARAM_ALT)
                .Append("=").Append(Uri.EscapeUriString(quoteCurrencyCode));
            if (null != since)
            {
                url.Append("&").Append(PARAM_SINCE)
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
            return await CallPrivate<T>(BuildUrl(method, Format.Json));
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="url">Endpoint to make a request to</param>
        /// <returns>The raw JSON returned from Vircurex</returns>
        private async Task<T> CallPrivate<T>(string url)
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

        public async Task<List<Model.MyOrder>> GetMyActiveOrders(MarketId marketId, int? limit)
        {
            throw new NotImplementedException();
        }

        public async Task<Model.Book> GetMarketDepth(MarketId marketId)
        {
            VircurexMarketId vircurexMarketId = (VircurexMarketId)marketId;

            return VircurexParsers.ParseOrderBook(await CallPublic<JObject>(Method.orderbook,
                vircurexMarketId.BaseCurrencyCode, vircurexMarketId.QuoteCurrencyCode));
        }

        public string GetNextNonce()
        {
            return DateTime.Now.Ticks.ToString();
        }

        public void SetApiKey(Method method, string key)
        {
            this.privateKeys[method] = key;
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
    }
}
