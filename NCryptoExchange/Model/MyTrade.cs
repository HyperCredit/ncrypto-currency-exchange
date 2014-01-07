using System;
namespace Lostics.NCryptoExchange.Model
{
    public class MyTrade<M, O> : Trade<M>
        where M : MarketId
        where O : OrderId
    {
        public MyTrade(TradeId tradeId, OrderType tradeType,
            DateTime dateTime, decimal price, decimal fee,
            decimal quantity,
            M marketId, O orderId)
            : base(tradeId, dateTime, price, quantity, marketId)
        {
            this.Fee = fee;
            this.OrderId = orderId;
            this.TradeType = tradeType;
        }

        public override string ToString()
        {
            return Enum.GetName(TradeType.GetType(), TradeType) + " "
                + this.Quantity + " at "
                + this.Price + " each";
        }

        public decimal Fee { get; private set; }
        public OrderId OrderId { get; private set; }
        public OrderType TradeType { get; private set; }
    }
}
