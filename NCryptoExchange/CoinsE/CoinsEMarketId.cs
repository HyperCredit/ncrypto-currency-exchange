using Lostics.NCryptoExchange.Model;
using System;

namespace Lostics.NCryptoExchange.CoinsE
{
    public sealed class CoinsEMarketId : AbstractStringBasedId, MarketId
    {
        public CoinsEMarketId(string setValue) : base(setValue)
        {
        }

        internal static CoinsEMarketId Parse(Newtonsoft.Json.Linq.JToken marketIdToken)
        {
            if (marketIdToken.Type != Newtonsoft.Json.Linq.JTokenType.String)
            {
                throw new CoinsEResponseException("Expected market ID as a string but encountered token type \""
                    + marketIdToken.Type + "\".");
            }

            return new CoinsEMarketId(marketIdToken.ToString());
        }
    }
}
