using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public class CryptsyMarket : Market<CryptsyMarketId>
    {
        private long currentVolume;
        private Quantity lastTrade;
        private Quantity highTrade;
        private Quantity lowTrade;
        private DateTime created;
    }
}
