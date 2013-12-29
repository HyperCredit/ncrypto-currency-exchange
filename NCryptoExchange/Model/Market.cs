using System;

namespace Lostics.NCryptoExchange.Model
{
    public abstract class Market<I> where I: MarketId
    {
        private readonly I marketId;
        private readonly string baseCurrencyCode;
        private readonly string baseCurrencyName;
        private readonly string quoteCurrencyCode;
        private readonly string quoteCurrencyName;
        private readonly string label;

        public Market(I setMarketId, string baseCurrencyCode, string baseCurrencyName,
            string quoteCurrencyCode, string quoteCurrencyName, string label)
        {
            this.marketId = setMarketId;
            this.baseCurrencyCode = baseCurrencyCode;
            this.baseCurrencyName = baseCurrencyName;
            this.quoteCurrencyCode = quoteCurrencyCode;
            this.quoteCurrencyName = quoteCurrencyName;
            this.label = label;
        }

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

        public I MarketId { get { return this.marketId; } }
        public string BaseCurrencyCode { get { return this.baseCurrencyCode; } }
        public string BaseCurrencyName { get { return this.baseCurrencyName; } }
        public string QuoteCurrencyCode { get { return this.quoteCurrencyCode; } }
        public string QuoteCurrencyName { get { return this.quoteCurrencyName; } }
        public string Label { get { return this.label; } }
    }
}
