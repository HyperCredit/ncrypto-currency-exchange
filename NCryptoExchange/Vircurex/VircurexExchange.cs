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
    public class VircurexExchange : AbstractExchange
    {
        public const string DEFAULT_BASE_URL = "https://vircurex.com/api/";

        public const string HEADER_SIGN = "sign";
        public const string HEADER_KEY = "key";

        public const string PARAM_BASE = "base";
        public const string PARAM_ALT = "alt";

        public const string PROPERTY_PUBLIC_KEY = "public_key";
        public const string PROPERTY_PRIVATE_KEY = "private_key";

        private readonly string publicKey;
        private readonly byte[] privateKey;
        private HttpClient client = new HttpClient();

        public VircurexExchange(string publicKey, string privateKey)
        {
            this.BaseUrl = DEFAULT_BASE_URL;

            this.publicKey = publicKey;
            this.privateKey = System.Text.Encoding.ASCII.GetBytes(privateKey);
        }

        private string BuildPublicUrl(Method method,Format format)
        {
 	        throw new NotImplementedException();
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="method">The method to call on the Vircurex API</param>
        /// <returns>The raw JSON returned from Vircurex</returns>
        private async Task<JObject> CallPublic(Method method)
        {
            return await CallPublic(BuildPublicUrl(method, Format.Json));
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="method">The method to call on the Vircurex API</param>
        /// <param name="baseCurrencyCode">An optional base currency code to append to the URL</param>
        /// <param name="quoteCurrencyCode">An optional quote currency code to append to the URL</param>
        /// <returns>The raw JSON returned from Vircurex</returns>
        private async Task<JObject> CallPublic(Method method, string baseCurrencyCode, string quoteCurrencyCode)
        {

            StringBuilder url = new StringBuilder(BuildPublicUrl(method, Format.Json));

            if (null != baseCurrencyCode
                || null != quoteCurrencyCode)
            {
                url.Append("?");

                if (null != baseCurrencyCode)
                {
                    url.Append(PARAM_BASE).Append("=")
                        .Append(Uri.EscapeUriString(baseCurrencyCode));
                    if (null != quoteCurrencyCode)
                    {
                        url.Append("&");
                    }
                }

                if (null != quoteCurrencyCode)
                {
                    url.Append(PARAM_ALT).Append("=").Append(Uri.EscapeUriString(quoteCurrencyCode));
                }
            }

            return await CallPublic(url.ToString());
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="url">Endpoint to make a request to</param>
        /// <returns>The raw JSON returned from Vircurex</returns>
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
                throw new VircurexResponseException("Could not parse response from Vircurex.", e);
            }

            return jsonObj;
        }

        public override void Dispose()
        {
            client.Dispose();
        }

        public override async Task<Model.AccountInfo> GetAccountInfo()
        {
            throw new NotImplementedException();
        }

        public async Task<List<VircurexCurrency>> GetCoins()
        {
            return VircurexCurrency.Parse(await CallPublic(Method.get_currency_info));
        }

        public static VircurexExchange GetExchange(FileInfo configurationFile)
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
        }

        public override async Task<List<Market>> GetMarkets()
        {
            Dictionary<string, string> currencyShortCodeToLabel = new Dictionary<string, string>();

            foreach (VircurexCurrency currency in await this.GetCoins())
            {
                currencyShortCodeToLabel.Add(currency.CurrencyCode, currency.Label);
            }

            JObject marketsJson = await CallPublic(Method.get_currency_info);
            List<Market> markets = new List<Market>();

            foreach (JProperty baseProperty in marketsJson.Properties())
            {
                string baseCurrency = baseProperty.Name;

                foreach (JProperty quoteProperty in (baseProperty.Value as JObject).Properties())
                {
                    markets.Add(VircurexMarket.Parse(currencyShortCodeToLabel, baseCurrency, quoteProperty));
                }
            }

            return markets;
        }

        public override async Task<Book> GetMarketOrders(MarketId marketId)
        {
            VircurexMarketId vircurexMarketId = (VircurexMarketId)marketId;

            return VircurexParsers.ParseMarketOrders(await CallPublic(Method.orderbook,
                vircurexMarketId.BaseCurrencyCode, vircurexMarketId.QuoteCurrencyCode));
        }

        public async Task<Dictionary<MarketId, Book>> GetMarketOrdersAlt(string quoteCurrencyCode)
        {
            return VircurexParsers.ParseMarketOrdersAlt(await CallPublic(Method.orderbook_alt,
                null, quoteCurrencyCode));
        }

        public override Task<List<Model.MarketTrade>> GetMarketTrades(MarketId marketId)
        {
            throw new NotImplementedException();
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

        public override Task<Model.Book> GetMarketDepth(MarketId marketId)
        {
            throw new NotImplementedException();
        }

        public override async Task CancelOrder(OrderId orderId)
        {
            throw new NotImplementedException();
        }

        public override async Task CancelMarketOrders(MarketId marketId)
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

        private static void WriteDefaultConfigurationFile(FileInfo file)
        {
            using (StreamWriter writer = new StreamWriter(new FileStream(file.FullName, FileMode.CreateNew)))
            {
                writer.WriteLine("# Configuration file for specifying API public & private key.");
                writer.WriteLine(PROPERTY_PUBLIC_KEY + "=");
                writer.WriteLine(PROPERTY_PRIVATE_KEY + "=");
            }
        }

        public string BaseUrl { get; private set; }
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
            orderbook
        }
    }
}
