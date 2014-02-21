using Lostics.NCryptoExchange.Model;
using System;

namespace Lostics.NCryptoExchange.Kraken
{
    public sealed class KrakenMarketId : MarketId
    {
        public KrakenMarketId(string setBaseCurrencyCode, string setQuoteCurrencyCode)
        {
            this.BaseCurrencyCode = setBaseCurrencyCode;
            this.QuoteCurrencyCode = setQuoteCurrencyCode;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            KrakenMarketId other = (KrakenMarketId)obj;

            return other.BaseCurrencyCode.Equals(this.BaseCurrencyCode)
                && other.QuoteCurrencyCode.Equals(this.QuoteCurrencyCode);
        }

        public override int GetHashCode()
        {
            int hash = 1;

            hash = (hash * 31) + this.BaseCurrencyCode.GetHashCode();
            hash = (hash * 31) + this.QuoteCurrencyCode.GetHashCode();

            return hash;
        }

        public override string ToString()
        {
            return this.BaseCurrencyCode + "/"
                + this.QuoteCurrencyCode;
        }

        public string BaseCurrencyCode { get; private set; }
        public string QuoteCurrencyCode { get; private set; }
    }
}
