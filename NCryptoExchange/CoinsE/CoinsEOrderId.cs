using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.CoinsE
{
    public sealed class CoinsEOrderId : OrderId
    {
        public CoinsEOrderId(CoinsEMarketId setMarketId, string setValue)
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

            CoinsEOrderId other = (CoinsEOrderId)obj;

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

        public CoinsEMarketId MarketId { get; private set; }
        public string Value { get; private set; }
    }
}
