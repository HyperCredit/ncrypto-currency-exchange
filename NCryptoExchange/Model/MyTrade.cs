using System;
namespace Lostics.NCryptoExchange.Model
{
    public class MyTrade<M, O, T> : Trade<M, T>
        where M : MarketId
        where O : OrderId
        where T : TradeId
    {
        private readonly O orderId;

        public MyTrade(T tradeId, OrderType tradeType,
            DateTime dateTime, decimal price,
            decimal quantity, decimal fee,
            M marketId, O orderId)
            : base(tradeId, tradeType, dateTime, price, quantity, fee, marketId)
        {
            this.orderId = orderId;
        }
    }
}
