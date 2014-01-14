using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.Bter
{
    public class BterMarket : Market
    {
        public BterMarket(BterMarketId id, MarketStatistics statistics)
            : base(id, id.BaseCurrencyCode, id.QuoteCurrencyCode,
                id.ToString(), statistics)
        {
        }

        public static List<Market> ParseMarkets(JObject marketsJson)
        {
            List<Market> markets = new List<Market>();

            foreach (JProperty marketProperty in marketsJson.Properties())
            {
                BterMarketId marketId = new BterMarketId(marketProperty.Name);
                JObject marketJson = (JObject)marketProperty.Value;

                MarketStatistics marketStats = new MarketStatistics()
                {
                    HighTrade = marketJson.Value<decimal>("high"),
                    LastTrade = marketJson.Value<decimal>("last"),
                    LowTrade = marketJson.Value<decimal>("low"),
                    Volume24HBase = marketJson.Value<decimal>("vol_" + marketId.BaseCurrencyCode.ToLower())
                };

                markets.Add(new BterMarket(marketId, marketStats));
            }

            return markets;
        }
    }
}
