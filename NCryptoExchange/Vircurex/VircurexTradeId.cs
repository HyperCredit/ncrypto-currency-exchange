using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.Vircurex
{
    public sealed class VircurexTradeId : AbstractStringBasedId, TradeId
    {
        public VircurexTradeId(string setValue) : base(setValue)
        {
        }

        internal static VircurexTradeId Parse(Newtonsoft.Json.Linq.JToken tradeIdToken)
        {
            if (tradeIdToken.Type != Newtonsoft.Json.Linq.JTokenType.String)
            {
                throw new VircurexResponseException("Expected trade ID as a string but encountered token type \""
                    + tradeIdToken.Type + "\".");
            }

            return new VircurexTradeId(tradeIdToken.ToString());
        }
    }
}
