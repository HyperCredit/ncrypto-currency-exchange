using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.Prelude
{
    public class PreludeMarket : Market
    {
        public PreludeMarket(PreludeMarketId id, MarketStatistics statistics)
            : base(id, id.BaseCurrencyCode, Enum.GetName(typeof(PreludeQuoteCurrency), id.QuoteCurrencyCode),
                id.ToString(), statistics)
        {
        }
    }
}
