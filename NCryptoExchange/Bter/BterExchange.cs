using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Bter
{
    public class BterExchange : AbstractExchange
    {
        public const string DEFAULT_BASE_URL = "http://bter.com/api/1/";

        public const string PATH_PAIRS = "pairs";

        private HttpClient client = new HttpClient();

        public BterExchange()
        {
        }

        public static string BuildPublicUrl(Method method)
        {
            return DEFAULT_BASE_URL + Enum.GetName(typeof(Method), method);
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="method">The method to call on the Bter API</param>
        /// <returns>The raw JSON returned from Bter</returns>
        private async Task<JObject> CallPublic(Method method)
        {
            return JObject.Parse(await CallPublic(BuildPublicUrl(method)));
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="method">The method to call on the Bter API</param>
        /// <param name="quoteCurrencyCode">A quote currency code to append to the URL</param>
        /// <returns>The raw JSON returned from Bter</returns>
        private async Task<JObject> CallPublic(Method method, BterMarketId marketId)
        {
            StringBuilder url = new StringBuilder(BuildPublicUrl(method));

            url.Append("/")
                .Append(Uri.EscapeUriString(marketId.Value));

            return JObject.Parse(await CallPublic(url.ToString()));
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="url">Endpoint to make a request to</param>
        /// <returns>The raw JSON returned from Bter</returns>
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
                throw new BterResponseException("Could not parse response from Bter.", e);
            }
        }

        public override void Dispose()
        {
            client.Dispose();
        }

        /// <summary>
        /// Formats a date and time in a manner suitable for use with Bter.
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

        public override async Task<List<Market>> GetMarkets()
        {
            JObject marketsJson = await CallPublic(Method.tickers);

            return BterMarket.ParseMarkets(marketsJson);
        }

        public override async Task<Book> GetMarketOrders(MarketId marketId)
        {
            BterMarketId bterMarketId = (BterMarketId)marketId;

            return BterParsers.ParseMarketOrders(await CallPublic(Method.depth, bterMarketId));
        }

        public override async Task<List<MarketTrade>> GetMarketTrades(MarketId marketId)
        {
            BterMarketId bterMarketId = (BterMarketId)marketId;

            return BterParsers.ParseMarketTrades(marketId,
                await CallPublic(Method.trade, bterMarketId));
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

        public override string Label
        {
            get { return "Bter"; }
        }

        public enum Method
        {
            depth,
            pairs,
            tickers,
            ticker,
            trade
        }
    }
}
