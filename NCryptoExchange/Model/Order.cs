using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.Model
{
    public abstract class Order : IComparable<Order>
    {
        public  Order(OrderType orderType, decimal price, decimal quantity)
        {
            this.OrderType = orderType;
            this.Price = price;
            this.Quantity = quantity;
        }

        public int CompareTo(Order other)
        {
            if (this.OrderType == other.OrderType)
            {
                if (this.Price.Equals(other.Price))
                {
                    return this.Quantity.CompareTo(other.Quantity);
                }
                else
                {
                    return this.Price.CompareTo(other.Price);
                }
            }
            else
            {
                switch (this.OrderType)
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
            return OrderType.ToString().ToUpper() + " "
                + Quantity + " at "
                + Price + " each.";
        }

        public OrderType OrderType { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal Price { get; private set; }
    }
}
