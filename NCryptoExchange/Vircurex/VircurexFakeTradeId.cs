using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.Vircurex
{
    /// <summary>
    /// Primary key for a trade within Vircurex. Vircurex doesn't provide
    /// actual unique keys, so we infer from order ID, and execution sequence.
    /// This is a kludge, and ideally Vircurex should provide actual unique IDs.
    /// </summary>
    public sealed class VircurexFakeTradeId : TradeId
    {
        public VircurexFakeTradeId(VircurexOrderId orderId, int orderExecutionIdx)
        {
            this.OrderId = orderId;
            this.OrderExecutionIdx = orderExecutionIdx;
        }

        public override bool Equals(object obj)
        {
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 1;

            hash = (hash * 31) + this.OrderId.GetHashCode();
            hash = (hash * 31) + this.OrderExecutionIdx;

            return hash;
        }

        public override string ToString()
        {
            return this.OrderId.Value + "-"
                + this.OrderExecutionIdx;
        }

        public VircurexOrderId OrderId { get; set; }
        public int OrderExecutionIdx { get; set; }
    }
}
