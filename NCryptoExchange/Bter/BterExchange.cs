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

namespace Lostics.NCryptoExchange.Bter
{
    public class BterExchange : AbstractSha512Exchange, IMarketTradesSource
    {
        public const string BASE_PUBLIC_URL = "http://bter.com/api/1/";
        public const string BASE_PRIVATE_URL = "http://bter.com/api/1/private/";

        public const string HEADER_SIGN = "SIGN";
        public const string HEADER_KEY = "KEY";

        public const string PARAMETER_NONCE = "Nonce";

        private HttpClient client = new HttpClient();

        public BterExchange()
        {
        }

        private void AssertResponseStatusSuccess(JObject resultObj)
        {
            string status = resultObj.Value<string>("result");

            if (status != "true")
            {
                throw new BterResponseException("Response from Bter was not a success status. Received: "
                    + resultObj.ToString());
            }

        }

        public static string BuildPublicUrl(Method method)
        {
            return BASE_PUBLIC_URL + Enum.GetName(typeof(Method), method);
        }

        public static string BuildPrivateUrl(Method method)
        {
            return BASE_PRIVATE_URL + Enum.GetName(typeof(Method), method);
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="method">The method to call on the Bter API</param>
        /// <returns>The raw JSON returned from Bter</returns>
        private async Task<T> CallPublic<T>(Method method)
            where T : JToken
        {
            return (T)JToken.Parse(await CallPublic(BuildPublicUrl(method)));
        }

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="method">The method to call on the Bter API</param>
        /// <param name="quoteCurrencyCode">A quote currency code to append to the URL</param>
        /// <returns>The raw JSON returned from Bter</returns>
        private async Task<T> CallPublic<T>(Method method, BterMarketId marketId)
            where T : JToken
        {
            StringBuilder url = new StringBuilder(BuildPublicUrl(method));

            url.Append("/")
                .Append(Uri.EscapeUriString(marketId.Value));

            return (T)JToken.Parse(await CallPublic(url.ToString()));
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

        /// <summary>
        /// Make a call to a public (non-authenticated) API
        /// </summary>
        /// <param name="url">Endpoint to make a request to</param>
        /// <returns>The raw JSON returned from Bter</returns>
        private async Task<string> CallPublic(string url, FormUrlEncodedContent request)
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

        /// <summary>
        /// Make a call to a private (authenticated) API
        /// </summary>
        /// <param name="method">The method to call on the Bter API</param>
        /// <returns>The raw JSON returned from Bter</returns>
        private async Task<T> CallPrivate<T>(Method method)
            where T : JToken
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(PARAMETER_NONCE, this.GetNextNonce())
            });

            this.SignRequest(request);

            return await CallPrivate<T>(method, request);
        }

        /// <summary>
        /// Make a call to a private (authenticated) API
        /// </summary>
        /// <param name="method">The method to call on the Bter API</param>
        /// <param name="request">The request (containing POST parameters) to send</param>
        /// <returns>The raw JSON returned from Bter</returns>
        private async Task<T> CallPrivate<T>(Method method, FormUrlEncodedContent request)
        {
            string url = BuildPrivateUrl(method);

            try
            {
                HttpResponseMessage response = await client.PostAsync(url, request);
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

        public override async Task<AccountInfo> GetAccountInfo()
        {
            JObject fundsJson = await CallPrivate<JObject>(Method.getfunds);

            List<Wallet> wallets = BterParsers.ParseWallets(fundsJson);

            return new AccountInfo(wallets);
        }

        public override async Task<List<Market>> GetMarkets()
        {
            JObject marketsJson = await CallPublic<JObject>(Method.tickers);

            return BterMarket.ParseMarkets(marketsJson);
        }

        public async Task<List<MarketTrade>> GetMarketTrades(MarketId marketId)
        {
            BterMarketId bterMarketId = (BterMarketId)marketId;

            return BterMarketTrade.Parse(marketId,
                await CallPublic<JObject>(Method.trade, bterMarketId));
        }

        public async Task<List<BterMarketId>> GetPairs()
        {
            JArray pairsJson = await CallPublic<JArray>(Method.tickers);

            return BterMarketId.ParsePairs(pairsJson);
        }

        public override async Task<List<Model.MyOrder>> GetMyActiveOrders(MarketId marketId, int? limit)
        {
            throw new NotImplementedException();
        }

        public override async Task<Model.Book> GetMarketDepth(MarketId marketId)
        {
            BterMarketId bterMarketId = (BterMarketId)marketId;

            return BterParsers.ParseOrderBook(await CallPublic<JObject>(Method.depth, bterMarketId));
        }

        public override string GetNextNonce()
        {
            return DateTime.Now.Ticks.ToString();
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
            get { return "Bter"; }
        }

        public enum Method
        {
            cancelorder,
            depth,
            getfunds,
            getorder,
            orderlist,
            pairs,
            placeorder,
            tickers,
            ticker,
            trade
        }
    }
}
