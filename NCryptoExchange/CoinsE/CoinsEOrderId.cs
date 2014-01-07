using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.CoinsE
{
    public sealed class CoinsEOrderId : AbstractLongBasedId, OrderId
    {
        public CoinsEOrderId(long setValue) : base(setValue)
        {
        }
    }
}
