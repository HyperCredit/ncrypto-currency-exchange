using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.VaultOfSatoshi
{
    public class VoSExchange : AbstractSha512Exchange, ICoinDataSource<VoSCurrency>, ITrading
    {
        public const string HEADER_SIGN = "Api-Sign";
        public const string HEADER_KEY = "Api-Key";

        public const string DEFAULT_BASE_URL = "https://api.vaultofsatoshi.com/";
        public const string DEFAULT_PUBLIC_URL = DEFAULT_BASE_URL + "public/";
        public const string DEFAULT_PRIVATE_URL = DEFAULT_BASE_URL + "info/";

        public const string PARAMETER_NONCE = "Nonce";

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
        private async Task<T> CallPrivate<T>(Method method)
            where T : JToken
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[]
            {
                GenerateNonceParameter()
            });

            return (T)JToken.Parse(await CallPrivate(method, request));
        }

        /// <summary>
        /// Make a call to a private API method.
        /// </summary>
        /// <param name="method">The method to call on the VoS API</param>
        /// <param name="quoteCurrencyCode">A quote currency code to append to the URL</param>
        /// <returns>Parsed JSON returned from VoS</returns>
        private async Task<T> CallPrivate<T>(Method method, VoSMarketId marketId)
            where T : JToken
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>(PARAMETER_NONCE, this.GetNextNonce()),
                marketId.BaseCurrencyCodeKeyValuePair,
                marketId.QuoteCurrencyCodeKeyValuePair
            });

            return (T)JToken.Parse(await CallPrivate(method, request));
        }

        /// <summary>
        /// Make a call to a private API method.
        /// </summary>
        /// <param name="method">The method to call on the VoS API</param>
        /// <param name="quoteCurrencyCode">A quote currency code to append to the URL</param>
        /// <returns>Parsed JSON returned from VoS</returns>
        private async Task<T> CallPrivate<T>(Method method, VoSOrderId orderId)
            where T : JToken
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>(PARAMETER_NONCE, this.GetNextNonce()),
                orderId.KeyValuePair
            });

            return (T)JToken.Parse(await CallPrivate(method, request));
        }

        /// <summary>
        /// Make a call to a private API method.
        /// </summary>
        /// <param name="method">The method to call on the VoS API</param>
        /// <param name="request">A request, containing the POST parameters. Authentication headers
        /// will be added to this.</param>
        /// <returns>The raw JSON returned from VoS</returns>
        private async Task<string> CallPrivate(Method method, FormUrlEncodedContent request)
        {
            string url = DEFAULT_PRIVATE_URL + Enum.GetName(typeof(Method), method);

            this.SignRequest(request);

            try
            {
                HttpResponseMessage response = await client.PostAsync(url, request);
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

        public override void Dispose()
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

        public override async Task<AccountInfo> GetAccountInfo()
        {
            JObject response = await this.CallPrivate<JObject>(Method.account);

            return VoSAccountInfo.Parse(response.Value<JObject>("data"));
        }

        public async Task<List<VoSCurrency>> GetCoins()
        {
            JObject response = await this.CallPrivate<JObject>(Method.currency);

            return response
                .Value<JArray>("data")
                .Select(coin => VoSCurrency.Parse((JObject)coin)).ToList();
        }

        public override async Task<List<Market>> GetMarkets()
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

        public override async Task<List<MyOrder>> GetMyActiveOrders(MarketId marketId, int? limit)
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

            JObject response = JObject.Parse(await CallPrivate(Method.orders, new FormUrlEncodedContent(kvPairs)));

            return VoSMyOrder.Parse(response.Value<JArray>("data"));
        }

        public override async Task<Book> GetMarketDepth(MarketId marketId)
        {
            JObject jsonObj = await this.CallPublic<JObject>(Method.orderbook, (VoSMarketId)marketId);
            return VoSParsers.ParseOrderBook(jsonObj.Value<JObject>("data"));
        }

        public async Task CancelOrder(OrderId orderId)
        {
            JObject response = await CallPrivate<JObject>(Method.cancel, (VoSOrderId)orderId);

            // TODO: Check the respnse.

            return;
        }

        public async Task<OrderId> CreateOrder(MarketId marketId, OrderType orderType, decimal quantity, decimal price)
        {
            throw new NotImplementedException();
        }

        public override string GetNextNonce()
        {
            return DateTime.Now.Ticks.ToString();
        }

        public override string Label
        {
            get { return "Vault of Satoshi"; }
        }
        public override string SignHeader
        {
            get { return HEADER_SIGN; }
        }
        public override string KeyHeader
        {
            get { return HEADER_KEY; }
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
