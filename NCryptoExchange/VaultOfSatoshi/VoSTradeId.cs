using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.VaultOfSatoshi
{
    public sealed class VoSTradeId : AbstractIntBasedId, TradeId
    {
        public VoSTradeId(int setValue) : base(setValue)
        {
        }
    }
}
