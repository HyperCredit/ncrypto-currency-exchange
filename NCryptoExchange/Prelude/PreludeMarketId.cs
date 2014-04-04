using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lostics.NCryptoExchange.Prelude
{
    public sealed class PreludeMarketId : AbstractStringBasedId, MarketId
    {
        public PreludeMarketId(string baseCode, PreludeQuoteCurrency quoteCode)
            : base(baseCode + "_" + quoteCode)
        {
            this.BaseCurrencyCode = baseCode;
            this.QuoteCurrencyCode = quoteCode;
        }

        public string BaseCurrencyCode { get; private set; }
        public PreludeQuoteCurrency QuoteCurrencyCode { get; private set; }

        public static List<MarketId> ParsePairs(JObject pairsJson, PreludeQuoteCurrency quoteCurrency)
        {
            // TODO: Verify the quote currency matches the "from" currency in the JSON

            return pairsJson.Value<JArray>("pairings").Select(
                trade => (MarketId)ParseSingle((JObject)trade, quoteCurrency)
            ).ToList();
        }

        private static PreludeMarketId ParseSingle(JObject pairJson, PreludeQuoteCurrency quoteCurrency)
        {
            return new PreludeMarketId(pairJson.Value<string>("pair"), quoteCurrency);
        }
    }
}
