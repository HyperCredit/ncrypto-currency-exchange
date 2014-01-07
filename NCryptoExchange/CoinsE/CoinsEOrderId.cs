using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.CoinsE
{
    public sealed class CoinsEOrderId : AbstractIntBasedId, OrderId
    {
        public CoinsEOrderId(int setValue) : base(setValue)
        {
        }
    }
}
