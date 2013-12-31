using System;

namespace Lostics.NCryptoExchange.Model
{
    /// <summary>
    /// A market for trading a currency pair.
    /// </summary>
    /// <typeparam name="I">The type of ID this market wraps around.</typeparam>
    public abstract class Market<I> where I: MarketId
    {
        public Market(I setMarketId, string baseCurrencyCode, string baseCurrencyName,
            string quoteCurrencyCode, string quoteCurrencyName, string label)
        {
            this.MarketId = setMarketId;
            this.BaseCurrencyCode = baseCurrencyCode;
            this.BaseCurrencyName = baseCurrencyName;
            this.QuoteCurrencyCode = quoteCurrencyCode;
            this.QuoteCurrencyName = quoteCurrencyName;
            this.Label = label;
        }

        public override bool Equals(Object obj)
        {
            if (!(obj is Market<I>))
            {
                return false;
            }

            Market<I> other = (Market<I>)obj;

            // Market IDs must be able to differentiate between each other (i.e.,
            // a Cryptsy market ID must only ever match another Cryptsy market ID),
            // such that markets on different exchanges are never considered the same

            return this.MarketId.Equals(other.MarketId);
        }

        public override int GetHashCode()
        {
            return this.MarketId.GetHashCode();
        }

        public override string ToString()
        {
            return this.Label;
        }

        public I MarketId { get; private set; }
        public string BaseCurrencyCode { get; private set; }
        public string BaseCurrencyName { get; private set; }
        public string QuoteCurrencyCode { get; private set; }
        public string QuoteCurrencyName { get; private set; }
        public string Label { get; private set; }
    }
}
