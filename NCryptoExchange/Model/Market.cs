using System;

namespace Lostics.NCryptoExchange.Model
{
    public virtual class Market<I> where I: MarketId
    {
        private readonly I marketId;
        private string baseCurrencyCode;
        private string baseCurrencyName;
        private string quoteCurrencyCode;
        private string quoteCurrencyName;
        private string label;

        public I MarketId { get { return this.marketId; } }
        public string BaseCurrencyCode { get { return this.baseCurrencyCode; } }
        public string BaseCurrencyName { get { return this.baseCurrencyName; } }
        public string QuoteCurrencyCode { get { return this.quoteCurrencyCode; } }
        public string QuoteCurrencyName { get { return this.quoteCurrencyName; } }
        public string Label { get { return this.label; } }

        public override bool Equals(Object o)
        {
            Market<I> other = (Market<I>)o;

            return this.marketId.Equals(other.marketId);
        }

        public override int GetHashCode()
        {
            return this.marketId.GetHashCode();
        }

        public override string ToString()
        {
            return this.label;
        }
    }
}
