using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.CoinsE
{
    public sealed class CoinsEOrderId : AbstractStringBasedId, OrderId
    {
        public CoinsEOrderId(string setValue) : base(setValue)
        {
        }
    }
}
