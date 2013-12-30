using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public sealed class CryptsyTradeId : AbstractStringBasedId, TradeId
    {
        public CryptsyTradeId(string setValue) : base(setValue)
        {
        }
    }
}
