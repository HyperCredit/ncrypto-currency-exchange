using System;

namespace Lostics.NCryptoExchange.Model
{
    public class MarketOrder : Order
    {
        public MarketOrder(OrderType orderType, decimal price, decimal quantity)
            : base (orderType, price, quantity)
        {
        }
    }
}
