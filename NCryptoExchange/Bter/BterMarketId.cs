using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Lostics.NCryptoExchange.Bter
{
    public sealed class BterMarketId : AbstractStringBasedId, MarketId
    {
        public BterMarketId(string id) : base(id)
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

        public static List<BterMarketId> ParsePairs(JArray pairsJson)
        {
            List<BterMarketId> pairs = new List<BterMarketId>();

            foreach (JToken pairJson in pairsJson)
            {
                if (!(pairJson is JValue))
                {
                    continue;
                }

                pairs.Add(new BterMarketId(pairJson.ToString()));
            }

            return pairs;
        }

        public string BaseCurrencyCode { get; private set; }
        public string QuoteCurrencyCode { get; private set; }
    }
}
