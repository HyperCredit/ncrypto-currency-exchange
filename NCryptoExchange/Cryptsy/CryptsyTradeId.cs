using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public sealed class CryptsyTradeId : AbstractStringBasedId, TradeId
    {
        public CryptsyTradeId(string setValue) : base(setValue)
        {
        }

        internal static CryptsyTradeId Parse(Newtonsoft.Json.Linq.JToken tradeIdToken)
        {
            if (tradeIdToken.Type != Newtonsoft.Json.Linq.JTokenType.String)
            {
                throw new CryptsyResponseException("Expected trade ID as a string but encountered token type \""
                    + tradeIdToken.Type + "\".");
            }

            return new CryptsyTradeId(tradeIdToken.ToString());
        }
    }
}
