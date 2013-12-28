using Lostics.NCryptoExchange.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public class CryptsyMarketId : MarketId
    {
        private readonly int value;

        public int Value
        {
            get { return this.value; }
        }

        public CryptsyMarketId(int setValue)
        {
            this.value = setValue;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CryptsyMarketId))
            {
                return false;
            }

            CryptsyMarketId other = (CryptsyMarketId)obj;

            return other.value == this.value;
        }

        public override int GetHashCode()
        {
            return this.value;
        }
    }
}
