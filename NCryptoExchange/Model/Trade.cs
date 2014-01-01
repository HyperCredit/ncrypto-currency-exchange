using System;

namespace Lostics.NCryptoExchange.Model
{
    public abstract class Trade<M>
        where M : MarketId
    {
        public Trade(TradeId tradeId, OrderType tradeType,
            DateTime dateTime, decimal price,
            decimal quantity, decimal fee,
            M marketId)
        {
            this.TradeId = tradeId;
            this.TradeType = tradeType;
            this.DateTime = dateTime;
            this.Price = price;
            this.Quantity = quantity;
            this.Fee = fee;
            this.MarketId = marketId;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Trade<M>))
            {
                return false;
            }

            Trade<M> other = (Trade<M>)obj;

            return this.TradeId.Equals(other.TradeId);
        }

        public override int GetHashCode()
        {
            return this.TradeId.GetHashCode();
        }

        public override string ToString()
        {
            return this.TradeType.ToString() + " "
                + this.Quantity + " at "
                + this.Price + " each";
        }

        public TradeId TradeId { get; private set; }
        public OrderType TradeType { get; private set; }
        public DateTime DateTime { get; private set; }
        public decimal Price { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal Fee { get; private set; }
        public M MarketId { get; private set; }
    }
}
