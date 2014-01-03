using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.CoinsE
{
    public static class CoinsEParsers
    {
        internal static List<Market<CoinsEMarketId>> ParseMarkets(Newtonsoft.Json.Linq.JArray marketsJson)
        {
            List<Market<CoinsEMarketId>> markets = new List<Market<CoinsEMarketId>>();

            foreach (JObject marketObj in marketsJson) {
                CoinsEMarket market = new CoinsEMarket(new CoinsEMarketId(marketObj.Value<string>("pair")),
                    marketObj.Value<string>("c1"), marketObj.Value<string>("coin1"),
                    marketObj.Value<string>("c2"), marketObj.Value<string>("coin2"),
                    marketObj.Value<string>("pair"),
                    marketObj.Value<string>("status"), marketObj.Value<decimal>("trade_fee")
                );

                markets.Add(market);
            }

            return markets;
        }

        public static Book ParseMarketOrders(JObject jObject)
        {
            throw new NotImplementedException();
        }

        public static MyTrade<CoinsEMarketId, CoinsEOrderId> ParseMyTrade(JObject jObject, CoinsEMarketId marketId, TimeZoneInfo defaultTimeZone)
        {
            throw new NotImplementedException();
        }

        public static AccountInfo ParseAccountInfo(JObject jsonObj)
        {
            throw new NotImplementedException();
        }
    }
}
