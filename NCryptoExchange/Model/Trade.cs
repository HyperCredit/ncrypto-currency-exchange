using System;

namespace Lostics.NCryptoExchange.Model
{
    public abstract class Trade<M, T>
        where M : MarketId
        where T : TradeId
    {
        private readonly T tradeId;
        private readonly OrderType tradeType;
        private readonly DateTime dateTime;
        private readonly decimal price;
        private readonly decimal quantity;
        private readonly decimal fee;
        private readonly M marketId;

        public Trade(T tradeId, OrderType tradeType,
            DateTime dateTime, decimal price,
            decimal quantity, decimal fee,
            M marketId)
        {
            this.tradeId = tradeId;
            this.tradeType = tradeType;
            this.dateTime = dateTime;
            this.price = price;
            this.quantity = quantity;
            this.fee = fee;
            this.marketId = marketId;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Trade<M, T>))
            {
                return false;
            }

            Trade<M, T> other = (Trade<M, T>)obj;

            return this.tradeId.Equals(other.tradeId);
        }

        public override int GetHashCode()
        {
            return this.tradeId.GetHashCode();
        }

        public override string ToString()
        {
            return this.tradeType.ToString() + " "
                + this.quantity + " at "
                + this.price + " each";
        }

        public T TradeId { get { return this.tradeId; } }
        public OrderType TradeType { get { return this.tradeType; } }
        public DateTime DateTime { get { return this.dateTime; } }
        public decimal Price { get { return this.price; } }
        public decimal Quantity { get { return this.quantity; } }
        public decimal Fee { get { return this.fee; } }
        public M MarketId { get { return this.marketId; } }
    }
}
