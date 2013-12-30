using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.CoinsE
{
    public sealed class CoinsETradeId : AbstractStringBasedId, TradeId
    {
        public CoinsETradeId(string setValue) : base(setValue)
        {
        }

        internal static CoinsETradeId Parse(Newtonsoft.Json.Linq.JToken tradeIdToken)
        {
            if (tradeIdToken.Type != Newtonsoft.Json.Linq.JTokenType.String)
            {
                throw new CoinsEResponseException("Expected trade ID as a string but encountered token type \""
                    + tradeIdToken.Type + "\".");
            }

            return new CoinsETradeId(tradeIdToken.ToString());
        }
    }
}
