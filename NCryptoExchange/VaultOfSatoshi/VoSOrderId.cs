using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.VaultOfSatoshi
{
    public sealed class VoSOrderId : AbstractIntBasedId, OrderId
    {
        public VoSOrderId(int value) : base (value)
        {
        }
    }
}
