using System;
namespace Lostics.NCryptoExchange.Model
{
    public class MyTrade<O, T> : Trade<O, T>
        where O : OrderId
        where T : TradeId
    {
        public MyTrade(T tradeId, OrderType tradeType,
            DateTime dateTime, Quantity price,
            Quantity quantity, Quantity fee) : base(tradeId, tradeType, dateTime, price, quantity, fee)
        {
        }
    }
}
