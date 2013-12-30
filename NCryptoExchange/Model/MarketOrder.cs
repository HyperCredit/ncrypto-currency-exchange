using System;

namespace Lostics.NCryptoExchange.Model
{
    public class MarketOrder : Order
    {
        public MarketOrder(OrderType orderType, Quantity price, Quantity quantity)
            : base (orderType, price, quantity)
        {
        }
    }
}
