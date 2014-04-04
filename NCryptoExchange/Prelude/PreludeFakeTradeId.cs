using Lostics.NCryptoExchange.Model;
using System;

namespace Lostics.NCryptoExchange.Prelude
{
    /// <summary>
    /// Primary key for a trade within Prelude, when examining market recent trades.
    /// Prelude doesn't provide primary keys for this data, so we use fake IDs
    /// instead.
    /// </summary>
    public sealed class PreludeFakeTradeId : TradeId
    {
        public PreludeFakeTradeId()
        {
            this.TradeId = Guid.NewGuid();
        }

        public override bool Equals(object obj)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return this.TradeId.GetHashCode();
        }

        public override string ToString()
        {
            return TradeId.ToString();
        }

        public Guid TradeId { get; set; }
    }
}
