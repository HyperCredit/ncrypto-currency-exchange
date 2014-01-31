using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.VaultOfSatoshi
{
    public class VoSMarket : Market
    {
        public VoSMarket(VoSMarketId id)
            : base(id, id.BaseCurrencyCode, id.QuoteCurrencyCode,
                id.ToString(), new MarketStatistics())
        {
        }
    }
}
