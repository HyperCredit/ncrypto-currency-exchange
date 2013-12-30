using Lostics.NCryptoExchange.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public sealed class CryptsyMarketId : AbstractStringBasedId, MarketId
    {
        public CryptsyMarketId(string setValue) : base(setValue)
        {
        }
    }
}
