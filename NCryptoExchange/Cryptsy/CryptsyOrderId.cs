using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public sealed class CryptsyOrderId : AbstractStringBasedId, OrderId
    {
        public CryptsyOrderId(string setValue) : base(setValue)
        {
        }
    }
}
