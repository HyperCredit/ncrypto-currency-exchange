using System;

namespace Lostics.NCryptoExchange.Model
{
    public class MarketOrder : MarketDepth
    {
        public MarketOrder(OrderType orderType, decimal price, decimal quantity)
            : base (price, quantity)
        {
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
    }
}
