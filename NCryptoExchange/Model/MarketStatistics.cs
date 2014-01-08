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

        public MarketStatistics(decimal setVolume24H, decimal lastTrade, decimal highTrade, decimal lowTrade)
        {
            this.Volume24H = setVolume24H;
            this.LastTrade = lastTrade;
            this.HighTrade = highTrade;
            this.LowTrade = lowTrade;
        }

        public decimal Volume24H { get; set; }
        public decimal LastTrade { get; set; }
        public decimal HighTrade { get; set; }
        public decimal LowTrade { get; set; }
    }
}
