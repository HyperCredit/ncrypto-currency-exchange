using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.Prelude
{
    public sealed class PreludeOrderId : AbstractIntBasedId, OrderId
    {
        public PreludeOrderId(int setValue) : base(setValue)
        {
        }
    }
}
