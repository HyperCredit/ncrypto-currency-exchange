using System;

namespace Lostics.NCryptoExchange.Model
{
    public class MyOrder<M, O> : Order
        where M : MarketId
        where O : OrderId
    {
        public MyOrder(O orderId, OrderType orderType,
            DateTime created, decimal price, decimal quantity, decimal originalQuantity,
            M marketId) : base(orderType, price, quantity)
        {
            this.OrderId = orderId;
            this.Created = created;
            this.OriginalQuantity = originalQuantity;
            this.MarketId = marketId;
        }

        public O OrderId { get; private set; }
        public DateTime Created { get; private set; }
        public decimal OriginalQuantity { get; private set; }
        public M MarketId { get; private set; }
    }
}
