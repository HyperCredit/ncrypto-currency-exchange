using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.Prelude
{
    public sealed class PreludeTradeId : AbstractIntBasedId, TradeId
    {
        public PreludeTradeId(int setValue) : base(setValue)
        {
        }
    }
}
