using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.Vircurex
{
    public sealed class VircurexTradeId : AbstractIntBasedId, TradeId
    {
        public VircurexTradeId(int setValue) : base(setValue)
        {
        }
    }
}
