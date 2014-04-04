using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Lostics.NCryptoExchange.Prelude
{
    public sealed class PreludeMarketId : AbstractStringBasedId, MarketId
    {
        public PreludeMarketId(string id) : base(id)
        {
            string[] parts = id.Split(new [] { '_' });

            if (parts.Length != 2)
            {
                throw new ArgumentException("Invalid market ID; expected two currency codes separated by the '_' character, but found \""
                    + id + "\".");
            }

            this.BaseCurrencyCode = parts[0].ToUpper();
            this.QuoteCurrencyCode = parts[1].ToUpper();
        }

        public static List<PreludeMarketId> ParsePairs(JArray pairsJson)
        {
            List<PreludeMarketId> pairs = new List<PreludeMarketId>();

            foreach (JToken pairJson in pairsJson)
            {
                if (!(pairJson is JValue))
                {
                    continue;
                }

                pairs.Add(new PreludeMarketId(pairJson.ToString()));
            }

            return pairs;
        }

        public string BaseCurrencyCode { get; private set; }
        public string QuoteCurrencyCode { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string BaseCurrencyMethodPostfix
        {
            get
            {
                switch (this.BaseCurrencyCode)
                {
                    case "BTC":
                        return "";
                    default:
                        return "-" + BaseCurrencyCode;
                }
            }
        }
    }
}
