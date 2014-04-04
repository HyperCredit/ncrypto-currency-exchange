using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

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

        public static List<MarketId> ParsePairs(JObject json, PreludeQuoteCurrency quoteCurrency)
        {
            return new List<MarketId>();
        }
    }
}
