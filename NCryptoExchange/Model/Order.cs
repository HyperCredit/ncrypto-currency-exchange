using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.Model
{
    public abstract class Order : IComparable<Order>
    {
        private readonly OrderType orderType;
        private readonly Price price;
        private readonly Price quantity;

        public  Order(OrderType orderType, Price price, Price quantity)
        {
            this.orderType = orderType;
            this.price = price;
            this.quantity = quantity;
        }

        public int CompareTo(Order other)
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
        public Price Quantity { get { return this.quantity; } }
        public Price Price { get { return this.price;  } }
    }
}
