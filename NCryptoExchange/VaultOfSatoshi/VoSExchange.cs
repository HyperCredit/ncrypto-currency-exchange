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
    public class VoSExchange : AbstractExchange, ICoinDataSource<VoSCurrency>
    {
        public const string DEFAULT_BASE_URL = "https://api.vaultofsatoshi.com/";
        public const string DEFAULT_PUBLIC_URL = DEFAULT_BASE_URL + "public/";
        public const string DEFAULT_PRIVATE_URL = DEFAULT_BASE_URL + "info/";

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

            url.Append(marketId.ToUrlParameters());

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
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="method">The method to call on the VoS API</param>
        /// <returns>The raw JSON returned from VoS</returns>
        private async Task<T> CallPrivate<T>(Method method)
            where T : JToken
        {
            return (T)JToken.Parse(await CallPrivate(BuildPrivateUrl(method)));
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="method">The method to call on the VoS API</param>
        /// <param name="quoteCurrencyCode">A quote currency code to append to the URL</param>
        /// <returns>The raw JSON returned from VoS</returns>
        private async Task<T> CallPrivate<T>(Method method, VoSMarketId marketId)
            where T : JToken
        {
            StringBuilder url = new StringBuilder(BuildPublicUrl(method));

            url.Append("?").Append(marketId.BaseCurrencyCodeParameter)
                .Append("&").Append(marketId.QuoteCurrencyCodeParameter);

            return (T)JToken.Parse(await CallPrivate(url.ToString()));
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="url">Endpoint to make a request to</param>
        /// <returns>The raw JSON returned from VoS</returns>
        private async Task<string> CallPrivate(string url)
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

        public override async Task<AccountInfo> GetAccountInfo()
        {
            throw new NotImplementedException();
        }

        public async Task<List<VoSCurrency>> GetCoins()
        {
            throw new NotImplementedException();
        }

        public override async Task<List<Market>> GetMarkets()
        {
            throw new NotImplementedException();
        }

        public override async Task<List<MarketTrade>> GetMarketTrades(MarketId marketId)
        {
            throw new NotImplementedException();
        }

        public override async Task<List<MyTrade>> GetMyTrades(MarketId marketId, int? limit)
        {
            throw new NotImplementedException();
        }

        public override async Task<List<MyTrade>> GetAllMyTrades(int? limit)
        {
            throw new NotImplementedException();
        }

        public override async Task<List<MyOrder>> GetMyActiveOrders(MarketId marketId, int? limit)
        {
            throw new NotImplementedException();
        }

        public override async Task<Book> GetMarketDepth(MarketId marketId)
        {
            JObject jsonObj = await this.CallPublic<JObject>(Method.orderbook, (VoSMarketId)marketId);
            return VoSParsers.ParseOrderBook(jsonObj.Value<JObject>("data"));
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

        public override string Label
        {
            get { return "Vault of Satoshi"; }
        }

        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }

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
