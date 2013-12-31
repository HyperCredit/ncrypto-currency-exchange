using System;
namespace Lostics.NCryptoExchange.Model
{
    public class MyTrade<M, O, T> : Trade<M, T>
        where M : MarketId
        where O : OrderId
        where T : TradeId
    {
        public MyTrade(T tradeId, OrderType tradeType,
            DateTime dateTime, decimal price,
            decimal quantity, decimal fee,
            M marketId, O orderId)
            : base(tradeId, tradeType, dateTime, price, quantity, fee, marketId)
        {
            this.OrderId = orderId;
        }

        public OrderId OrderId { get; private set; }
    }
}
