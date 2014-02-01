using Lostics.NCryptoExchange.Model;
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
    public class VircurexExchange : AbstractExchange, ICoinDataSource<VircurexCurrency>
    {
        public const string DEFAULT_BASE_URL = "https://vircurex.com/api/";

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

        public static string BuildPublicUrl(Method method,Format format)
        {
            return DEFAULT_BASE_URL + Enum.GetName(typeof(Method), method)
                + "." + Enum.GetName(typeof(Format), format).ToLower();
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="method">The method to call on the Vircurex API</param>
        /// <returns>The raw JSON returned from Vircurex</returns>
        private async Task<JObject> CallPublic(Method method)
        {
            return JObject.Parse(await CallPublic(BuildPublicUrl(method, Format.Json)));
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="method">The method to call on the Vircurex API</param>
        /// <param name="quoteCurrencyCode">A quote currency code to append to the URL</param>
        /// <returns>The raw JSON returned from Vircurex</returns>
        private async Task<JObject> CallPublic(Method method, string quoteCurrencyCode)
        {
            StringBuilder url = new StringBuilder(BuildPublicUrl(method, Format.Json));
            url.Append("?");
            url.Append(PARAM_ALT).Append("=").Append(Uri.EscapeUriString(quoteCurrencyCode));

            return JObject.Parse(await CallPublic(url.ToString()));
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="method">The method to call on the Vircurex API</param>
        /// <param name="baseCurrencyCode">A base currency code to append to the URL</param>
        /// <param name="quoteCurrencyCode">A quote currency code to append to the URL</param>
        /// <returns>The raw JSON returned from Vircurex</returns>
        private async Task<JObject> CallPublic(Method method, string baseCurrencyCode, string quoteCurrencyCode)
        {
            StringBuilder url = new StringBuilder(BuildPublicUrl(method, Format.Json));

            url.Append("?");

            url.Append(PARAM_BASE).Append("=")
                .Append(Uri.EscapeUriString(baseCurrencyCode));
            url.Append("&").Append(PARAM_ALT)
                .Append("=").Append(Uri.EscapeUriString(quoteCurrencyCode));

            return JObject.Parse(await CallPublic(url.ToString()));
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
        private async Task<JArray> CallPublic(Method method, string baseCurrencyCode, string quoteCurrencyCode,
            VircurexOrderId since)
        {
            StringBuilder url = new StringBuilder(BuildPublicUrl(method, Format.Json));

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

            return JArray.Parse(await CallPublic(url.ToString()));
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="url">Endpoint to make a request to</param>
        /// <returns>The raw JSON returned from Vircurex</returns>
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
                throw new VircurexResponseException("Could not parse response from Vircurex.", e);
            }
        }

        public override void Dispose()
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

        public override async Task<Model.AccountInfo> GetAccountInfo()
        {
            throw new NotImplementedException();
        }

        public async Task<List<VircurexCurrency>> GetCoins()
        {
            return VircurexCurrency.Parse(await CallPublic(Method.get_currency_info));
        }

        /* public static VircurexExchange GetExchange(FileInfo configurationFile)
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

            return new VircurexExchange(publicKey, privateKey);
        } */

        public override async Task<List<Market>> GetMarkets()
        {
            JObject marketsJson = await CallPublic(Method.get_info_for_currency);

            return VircurexMarket.ParseMarkets(marketsJson);
        }

        public async Task<Dictionary<MarketId, Book>> GetMarketOrdersAlt(string quoteCurrencyCode)
        {
            return VircurexParsers.ParseMarketOrdersAlt(quoteCurrencyCode,
                await CallPublic(Method.orderbook_alt, quoteCurrencyCode));
        }

        public override async Task<List<MarketTrade>> GetMarketTrades(MarketId marketId)
        {
            VircurexMarketId vircurexMarketId = (VircurexMarketId)marketId;

            return VircurexParsers.ParseMarketTrades(marketId,
                await CallPublic(Method.trades,
                    vircurexMarketId.BaseCurrencyCode, vircurexMarketId.QuoteCurrencyCode, null));
        }

        public async Task<List<MarketTrade>> GetMarketTrades(VircurexMarketId vircurexMarketId, VircurexOrderId since)
        {
            return VircurexParsers.ParseMarketTrades(vircurexMarketId,
                await CallPublic(Method.trades,
                    vircurexMarketId.BaseCurrencyCode, vircurexMarketId.QuoteCurrencyCode, since));
        }

        public override async Task<List<Model.MyTrade>> GetMyTrades(MarketId marketId, int? limit)
        {
            throw new NotImplementedException();
        }

        public override async Task<List<Model.MyTrade>> GetAllMyTrades(int? limit)
        {
            throw new NotImplementedException();
        }

        public override async Task<List<Model.MyOrder>> GetMyActiveOrders(MarketId marketId, int? limit)
        {
            throw new NotImplementedException();
        }

        public override async Task<Model.Book> GetMarketDepth(MarketId marketId)
        {
            VircurexMarketId vircurexMarketId = (VircurexMarketId)marketId;

            return VircurexParsers.ParseOrderBook(await CallPublic(Method.orderbook,
                vircurexMarketId.BaseCurrencyCode, vircurexMarketId.QuoteCurrencyCode));
        }

        public override async Task CancelOrder(OrderId orderId)
        {
            throw new NotImplementedException();
        }

        public override async Task<OrderId> CreateOrder(MarketId marketId, OrderType orderType, decimal quantity, decimal price)
        {
            throw new NotImplementedException();
        }

        public override string GetNextNonce()
        {
            return DateTime.Now.Ticks.ToString();
        }

        public void SetApiKey(Method method, string key)
        {
            this.privateKeys[method] = key;
        }

        public override string Label
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
            get_currency_info,
            get_info_for_currency,
            orderbook_alt,
            orderbook,
            trades
        }
    }
}
