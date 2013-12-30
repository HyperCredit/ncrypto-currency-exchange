using System;

namespace Lostics.NCryptoExchange.Model
{
    public class MyOrder<M, O> : Order
        where M : MarketId
        where O : OrderId
    {
        private readonly O orderId;
        private readonly M marketId;
        private readonly DateTime created;
        private readonly Quantity originalQuantity;

        public MyOrder(O orderId, OrderType orderType,
            DateTime created, Quantity price, Quantity quantity, Quantity originalQuantity,
            M marketId) : base(orderType, price, quantity)
        {
            this.orderId = orderId;
            this.created = created;
            this.originalQuantity = originalQuantity;
            this.marketId = marketId;
        }

        public O OrderId { get { return this.orderId; } }
        public DateTime Created { get { return this.created; } }
        public Quantity OriginalQuantity { get { return this.originalQuantity; } }
        public M MarketId { get { return this.marketId; } }
    }
}
