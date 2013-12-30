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
        private readonly Price currentVolume;
        private readonly Price lastTrade;
        private readonly Price highTrade;
        private readonly Price lowTrade;
        private readonly DateTime created;

        public CryptsyMarket(CryptsyMarketId setMarketId, string baseCurrencyCode, string baseCurrencyName,
            string quoteCurrencyCode, string quoteCurrencyName, string label,
            Price currentVolume, Price lastTrade, Price highTrade, Price lowTrade, DateTime created)
            : base(setMarketId, baseCurrencyCode, baseCurrencyName, quoteCurrencyCode, quoteCurrencyName, label)
        {
            this.currentVolume = currentVolume;
            this.lastTrade = lastTrade;
            this.highTrade = highTrade;
            this.lowTrade = lowTrade;
            this.created = created;
        }

        public Price CurrentVolume { get { return this.currentVolume; } }
        public Price LastTrade { get { return this.lastTrade; } }
        public Price HighTrade { get { return this.highTrade; } }
        public Price LowTrade { get { return this.lowTrade; } }
        public DateTime Created { get { return this.created; } }
    }
}
