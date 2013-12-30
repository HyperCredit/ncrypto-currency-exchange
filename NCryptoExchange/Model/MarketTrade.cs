using System;
namespace Lostics.NCryptoExchange.Model
{
    public class MarketTrade<O, T> : Trade<O, T>
        where O : OrderId
        where T : TradeId
    {
        public MarketTrade(T tradeId, OrderType tradeType,
            DateTime dateTime, Quantity price,
            Quantity quantity, Quantity fee) : base(tradeId, tradeType, dateTime, price, quantity, fee)
        {
        }
    }
}
