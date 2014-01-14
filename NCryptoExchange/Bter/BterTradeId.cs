using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.Bter
{
    public sealed class BterTradeId : AbstractIntBasedId, TradeId
    {
        public BterTradeId(int setValue) : base(setValue)
        {
        }
    }
}
