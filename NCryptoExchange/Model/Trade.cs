using System;

namespace Lostics.NCryptoExchange.Model
{
    public abstract class Trade<O, T>
        where O : OrderId
        where T : TradeId
    {
        private readonly T tradeId;
        private readonly OrderType tradeType;
        private readonly DateTime dateTime;
        private readonly Quantity price;
        private readonly Quantity quantity;
        private readonly Quantity fee;

        public Trade(T tradeId, OrderType tradeType,
            DateTime dateTime, Quantity price,
            Quantity quantity, Quantity fee)
        {
            this.tradeId = tradeId;
            this.tradeType = tradeType;
            this.dateTime = dateTime;
            this.price = price;
            this.quantity = quantity;
            this.fee = fee;
        }

        public T TradeId { get { return this.tradeId; } }
        public OrderType OrderType { get { return this.tradeType; } }
        public DateTime DateTime { get { return this.dateTime; } }
        public Quantity Price { get { return this.price; } }
        public Quantity Quantity { get { return this.quantity; } }
        public Quantity Fee { get { return this.fee; } }
    }
}
