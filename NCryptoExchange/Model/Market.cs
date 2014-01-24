using System;

namespace Lostics.NCryptoExchange.Model
{
    /// <summary>
    /// A market for trading a currency pair.
    /// </summary>
    public abstract class Market
    {
        public Market(MarketId setMarketId, string baseCurrencyCode, string quoteCurrencyCode, string label,
            MarketStatistics statistics)
        {
            this.MarketId = setMarketId;
            this.BaseCurrencyCode = baseCurrencyCode;
            this.QuoteCurrencyCode = quoteCurrencyCode;
            this.Label = label;
            this.Statistics = statistics;
        }

        public override bool Equals(Object obj)
        {
            if (!(obj is Market))
            {
                return false;
            }

            Market other = (Market)obj;

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

        public MarketId MarketId { get; private set; }
        public string BaseCurrencyCode { get; private set; }
        public string QuoteCurrencyCode { get; private set; }
        public string Label { get; private set; }
        public MarketStatistics Statistics { get; private set; }
    }
}
