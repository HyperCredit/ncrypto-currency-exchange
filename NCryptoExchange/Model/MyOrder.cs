using System;

namespace Lostics.NCryptoExchange.Model
{
    public class MyOrder : Order
    {
        public MyOrder(OrderId orderId, OrderType orderType,
            DateTime created, decimal price, decimal quantity, decimal originalQuantity,
            MarketId marketId) : base(orderType, price, quantity)
        {
            this.OrderId = orderId;
            this.Created = created;
            this.OriginalQuantity = originalQuantity;
            this.MarketId = marketId;
        }

        public OrderId OrderId { get; private set; }
        public DateTime Created { get; private set; }
        public decimal OriginalQuantity { get; private set; }
        public MarketId MarketId { get; private set; }
    }
}
