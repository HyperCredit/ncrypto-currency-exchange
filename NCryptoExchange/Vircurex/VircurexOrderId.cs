using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.Vircurex
{
    public sealed class VircurexOrderId
        : AbstractIntBasedId, OrderId
    {
        public VircurexOrderId(int setValue)
            : base(setValue)
        {
        }
    }
}
