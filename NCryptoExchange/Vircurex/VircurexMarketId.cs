using Lostics.NCryptoExchange.Model;
using System;

namespace Lostics.NCryptoExchange.Vircurex
{
    public sealed class VircurexMarketId : AbstractStringBasedId, MarketId
    {
        public VircurexMarketId(string setValue) : base(setValue)
        {
        }

        internal static VircurexMarketId Parse(Newtonsoft.Json.Linq.JToken marketIdToken)
        {
            if (marketIdToken.Type != Newtonsoft.Json.Linq.JTokenType.String)
            {
                throw new VircurexResponseException("Expected market ID as a string but encountered token type \""
                    + marketIdToken.Type + "\".");
            }

            return new VircurexMarketId(marketIdToken.ToString());
        }
    }
}
