using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.Kraken
{
    public sealed class KrakenTradeId : AbstractIntBasedId, TradeId
    {
        public KrakenTradeId(int setValue) : base(setValue)
        {
        }
    }
}
