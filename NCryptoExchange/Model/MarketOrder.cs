using System;

namespace Lostics.NCryptoExchange.Model
{
    public class MarketOrder : Order
    {
        public MarketOrder(OrderType orderType, Price price, Price quantity)
            : base (orderType, price, quantity)
        {
        }
    }
}
