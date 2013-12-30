using System;

namespace Lostics.NCryptoExchange.Model
{
    public class MyOrder<M, O>
        where M : MarketId
        where O : OrderId
    {
        private readonly O orderId;
        private readonly M marketId;
        private readonly DateTime created;
        private readonly OrderType orderType;
        private readonly Quantity price;
        private readonly Quantity quantity;
        private readonly Quantity originalQuantity;

        public MyOrder(O orderId, OrderType orderType,
            DateTime created, Quantity price, Quantity quantity, Quantity originalQuantity,
            M marketId)
        {
            this.orderId = orderId;
            this.orderType = orderType;
            this.created = created;
            this.price = price;
            this.quantity = quantity;
            this.originalQuantity = originalQuantity;
            this.marketId = marketId;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MyOrder<M, O>))
            {
                return false;
            }

            MyOrder<M, O> other = (MyOrder<M, O>)obj;

            return this.orderId.Equals(other.orderId);
        }

        public override int GetHashCode()
        {
            return this.orderId.GetHashCode();
        }

        public override string ToString()
        {
            return this.orderType.ToString() + " "
                + this.quantity + " at "
                + this.price + " each";
        }

        public O OrderId { get { return this.orderId; } }
        public OrderType OrderType { get { return this.orderType; } }
        public DateTime Created { get { return this.created; } }
        public Quantity Price { get { return this.price; } }
        public Quantity Quantity { get { return this.quantity; } }
        public Quantity OriginalQuantity { get { return this.originalQuantity; } }
        public M MarketId { get { return this.marketId; } }
    }
}
