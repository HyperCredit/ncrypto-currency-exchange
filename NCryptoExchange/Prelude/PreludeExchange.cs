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

namespace Lostics.NCryptoExchange.Prelude
{
    public class PreludeExchange : AbstractSha512Exchange, IMarketTradesSource
    {
        public const string BASE_URL = "https://api.prelude.io/";

        public const string HEADER_SIGN = "SIGN";
        public const string HEADER_KEY = "KEY";

        public const string PARAMETER_NONCE = "Nonce";

        private HttpClient client = new HttpClient();

        public PreludeExchange()
        {
        }

        private void AssertResponseStatusSuccess(JObject resultObj)
        {
            string status = resultObj.Value<string>("status");

            if (status != "success")
            {
                throw new PreludeResponseException("Response from Prelude was not a success status. Received: "
                    + resultObj.ToString());
            }

        }

        public static string BuildPublicUrl(Method method, PreludeQuoteCurrency quoteCurrency)
        {
            switch (method)
            {
                case Method.pairings:
                    return BASE_URL + Enum.GetName(typeof(Method), method) + "/"
                        + Uri.EscapeUriString(Enum.GetName(typeof(PreludeQuoteCurrency), quoteCurrency).ToLower());
                    break;
                default:
                return BASE_URL + Enum.GetName(typeof(Method), method)
                    + Uri.EscapeUriString(GetQuoteCurrencyMethodPostfix(quoteCurrency)) + "/";
            }
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="method">The method to call on the Prelude API</param>
        /// <param name="quoteCurrency">A quote currency code to append to the method URL</param>
        /// <returns>The raw JSON returned from Prelude</returns>
        private async Task<T> CallPublic<T>(Method method, PreludeQuoteCurrency quoteCurrencyCode)
            where T : JToken
        {
            string url = BuildPublicUrl(method, quoteCurrencyCode);

            return (T)JToken.Parse(await CallPublic(url));
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="method">The method to call on the Prelude API</param>
        /// <param name="marketId">A market ID to append to the method URL</param>
        /// <returns>The raw JSON returned from Prelude</returns>
        private async Task<T> CallPublic<T>(Method method, PreludeMarketId marketId)
            where T : JToken
        {
            string url = BuildPublicUrl(method, marketId.QuoteCurrencyCode)
                + Uri.EscapeUriString(marketId.BaseCurrencyCode.ToLower());

            return (T)JToken.Parse(await CallPublic(url));
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="url">Endpoint to make a request to</param>
        /// <returns>The raw JSON returned from Prelude</returns>
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
                throw new PreludeResponseException("Could not parse response from Prelude.", e);
            }
        }

        public override void Dispose()
        {
            client.Dispose();
        }

        /// <summary>
        /// Formats a date and time in a manner suitable for use with Prelude.
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

        public override async Task<List<Market>> GetMarkets()
        {
            throw new NotImplementedException();
        }

        public async Task<List<MarketTrade>> GetMarketTrades(MarketId marketId)
        {
            PreludeMarketId preludeMarketId = (PreludeMarketId)marketId;

            return PreludeMarketTrade.Parse(marketId,
                await CallPublic<JObject>(Method.last, preludeMarketId));
        }

        public override async Task<List<Model.MyOrder>> GetMyActiveOrders(MarketId marketId, int? limit)
        {
            throw new NotImplementedException();
        }

        public override async Task<Model.Book> GetMarketDepth(MarketId marketId)
        {
            PreludeMarketId preludeMarketId = (PreludeMarketId)marketId;

            return PreludeParsers.ParseOrderBook(await CallPublic<JObject>(Method.combined, preludeMarketId));
        }

        public override string GetNextNonce()
        {
            return DateTime.Now.Ticks.ToString();
        }

        public async Task<List<PreludeMarketId>> GetPairs()
        {
            List<PreludeMarketId> pairs = new List<PreludeMarketId>();

            foreach (PreludeQuoteCurrency quoteCurrency in Enum.GetValues(typeof(PreludeQuoteCurrency)))
            {
                JObject response = await CallPublic<JObject>(Method.pairings, quoteCurrency);
                foreach (PreludeMarketId marketId in PreludeMarketId.ParsePairs(response, quoteCurrency
                )) {
                    pairs.Add(marketId);
                }
            }

            return pairs;
        }

        /// <summary>
        /// Return the (unescaped) path element to be inserted between the method,
        /// and quote currency, in the URL. This is required because Prelude has
        /// inconsistent URL naming depending on base currency.
        /// </summary>
        static string GetQuoteCurrencyMethodPostfix(PreludeQuoteCurrency quoteCurrency)
        {
            switch (quoteCurrency)
            {
                case PreludeQuoteCurrency.BTC:
                    return "";
                default:
                    return "-" + Enum.GetName(typeof(PreludeQuoteCurrency), quoteCurrency).ToLower();
            }
        }

        public override string SignHeader
        {
            get { return HEADER_SIGN; }
        }
        public override string KeyHeader
        {
            get { return HEADER_KEY; }
        }
        public override string Label
        {
            get { return "Prelude"; }
        }

        public enum Method
        {
            combined,
            last,
            pairings,
            statistics,
        }
    }
}
