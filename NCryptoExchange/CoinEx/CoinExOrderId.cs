using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.CoinEx
{
    public sealed class CoinExOrderId : OrderId
    {
        public CoinExOrderId(CoinExMarketId setMarketId, string setValue)
        {
            this.MarketId = setMarketId;
            this.Value = setValue;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            CoinExOrderId other = (CoinExOrderId)obj;

            return other.MarketId.Equals(this.MarketId)
                && other.Value.Equals(this.Value);
        }

        public override int GetHashCode()
        {
            int hash = 1;

            hash = (hash * 31) + this.MarketId.GetHashCode();
            hash = (hash * 31) + this.Value.GetHashCode();

            return hash;
        }

        public override string ToString()
        {
            return this.Value;
        }

        public CoinExMarketId MarketId { get; private set; }
        public string Value { get; private set; }
    }
}
