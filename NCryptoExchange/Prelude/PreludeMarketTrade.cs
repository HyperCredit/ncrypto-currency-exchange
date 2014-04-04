using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Prelude
{
    public class PreludeMarketTrade : MarketTrade
    {
        public PreludeMarketTrade(TradeId tradeId, 
            DateTime dateTime, decimal price,
            decimal quantity, MarketId marketId)
            : base(tradeId, dateTime, price, quantity, marketId)
        {
        }

        public static List<MarketTrade> Parse(MarketId marketId, JObject tradesJson)
        {
            return tradesJson.Value<JArray>("orders").Select(
                trade => (MarketTrade)ParseSingle(marketId, (JObject)trade)
            ).ToList();
        }

        internal static PreludeMarketTrade ParseSingle(MarketId marketId, JObject trade)
        {
            return new PreludeMarketTrade(new PreludeFakeTradeId(),
                PreludeParsers.ParseDateTime(trade.Value<int>("timestamp")), trade.Value<decimal>("rate"),
                trade.Value<decimal>("amount"), marketId);
        }
    }
}
