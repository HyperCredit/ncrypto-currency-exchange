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
        private readonly decimal currentVolume;
        private readonly decimal lastTrade;
        private readonly decimal highTrade;
        private readonly decimal lowTrade;
        private readonly DateTime created;

        public CryptsyMarket(CryptsyMarketId setMarketId, string baseCurrencyCode, string baseCurrencyName,
            string quoteCurrencyCode, string quoteCurrencyName, string label,
            decimal currentVolume, decimal lastTrade, decimal highTrade, decimal lowTrade, DateTime created)
            : base(setMarketId, baseCurrencyCode, baseCurrencyName, quoteCurrencyCode, quoteCurrencyName, label)
        {
            this.currentVolume = currentVolume;
            this.lastTrade = lastTrade;
            this.highTrade = highTrade;
            this.lowTrade = lowTrade;
            this.created = created;
        }

        public decimal CurrentVolume { get { return this.currentVolume; } }
        public decimal LastTrade { get { return this.lastTrade; } }
        public decimal HighTrade { get { return this.highTrade; } }
        public decimal LowTrade { get { return this.lowTrade; } }
        public DateTime Created { get { return this.created; } }
    }
}
