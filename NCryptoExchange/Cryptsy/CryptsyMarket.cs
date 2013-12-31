using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public class CryptsyMarket : Market<CryptsyMarketId>
    {
        public CryptsyMarket(CryptsyMarketId setMarketId, string baseCurrencyCode, string baseCurrencyName,
            string quoteCurrencyCode, string quoteCurrencyName, string label,
            decimal currentVolume, decimal lastTrade, decimal highTrade, decimal lowTrade, DateTime created)
            : base(setMarketId, baseCurrencyCode, baseCurrencyName, quoteCurrencyCode, quoteCurrencyName, label)
        {
            this.CurrentVolume = currentVolume;
            this.LastTrade = lastTrade;
            this.HighTrade = highTrade;
            this.LowTrade = lowTrade;
            this.Created = created;
        }

        public decimal CurrentVolume { get; private set; }
        public decimal LastTrade { get; private set; }
        public decimal HighTrade { get; private set; }
        public decimal LowTrade { get; private set; }
        public DateTime Created { get; private set; }
    }
}
