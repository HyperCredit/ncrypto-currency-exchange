using System;
namespace Lostics.NCryptoExchange.Model
{
    public class MyTrade<M, O> : Trade<M>
        where M : MarketId
        where O : OrderId
    {
        public MyTrade(TradeId tradeId, OrderType tradeType,
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
