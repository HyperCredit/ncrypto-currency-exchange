using System;

namespace Lostics.NCryptoExchange.Model
{
    public abstract class Trade
    {
        public Trade(TradeId tradeId,
            DateTime dateTime, decimal price,
            decimal quantity,
            MarketId marketId)
        {
            this.Price = price;
            this.Quantity = quantity;
            this.TradeId = tradeId;
            this.DateTime = dateTime;
            this.MarketId = marketId;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Trade))
            {
                return false;
            }

            Trade other = (Trade)obj;

            return this.TradeId.Equals(other.TradeId);
        }

        public override int GetHashCode()
        {
            return this.TradeId.GetHashCode();
        }

        public override string ToString()
        {
            return "Traded "
                + this.Quantity + " at "
                + this.Price + " each";
        }

        public TradeId TradeId { get; private set; }
        public DateTime DateTime { get; private set; }
        public MarketId MarketId { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal Price { get; private set; }
    }
}
