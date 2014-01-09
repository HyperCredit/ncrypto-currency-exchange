using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Model
{
    public class MarketStatistics
    {
        public MarketStatistics()
        {
        }

        public decimal Volume24HBase { get; set; }
        public decimal LastTrade { get; set; }
        public decimal HighTrade { get; set; }
        public decimal LowTrade { get; set; }
    }
}
