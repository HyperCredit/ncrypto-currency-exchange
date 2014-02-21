using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.Kraken
{
    public sealed class KrakenOrderId
        : AbstractIntBasedId, OrderId
    {
        public KrakenOrderId(int setValue)
            : base(setValue)
        {
        }
    }
}
