using Lostics.NCryptoExchange.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public sealed class CryptsyMarketId : AbstractStringBasedId, MarketId
    {
        public CryptsyMarketId(string setValue) : base(setValue)
        {
        }

        internal static CryptsyMarketId Parse(Newtonsoft.Json.Linq.JToken marketIdToken)
        {
            if (marketIdToken.Type != Newtonsoft.Json.Linq.JTokenType.String)
            {
                throw new CryptsyResponseException("Expected market ID as a string but encountered token type \""
                    + marketIdToken.Type + "\".");
            }

            return new CryptsyMarketId(marketIdToken.ToString());
        }
    }
}
