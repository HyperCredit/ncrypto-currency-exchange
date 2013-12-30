using System;

namespace Lostics.NCryptoExchange.Model
{
    public class MarketOrder : IComparable<MarketOrder>
    {
        private readonly OrderType orderType;
        private readonly Quantity price;
        private readonly Quantity quantity;

        public MarketOrder(OrderType orderType, Quantity price, Quantity quantity)
        {
            this.orderType = orderType;
            this.price = price;
            this.quantity = quantity;
        }

        public int CompareTo(MarketOrder other)
        {
            if (this.orderType == other.orderType)
            {
                if (this.price.Equals(other.price))
                {
                    return this.quantity.CompareTo(other.quantity);
                }
                else
                {
                    return this.price.CompareTo(other.price);
                }
            }
            else
            {
                switch (this.orderType)
                {
                    case OrderType.Sell:
                        return -1;
                    default:
                        return 1;
                }
            }
        }

        public override string ToString()
        {
            return orderType.ToString().ToUpper() + " "
                + quantity + " at "
                + price + " each.";
        }

        public OrderType OrderType { get { return this.orderType;  } }
        public Quantity Quantity { get { return this.quantity; } }
        public Quantity Price { get { return this.price;  } }
    }
}
