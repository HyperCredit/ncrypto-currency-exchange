using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;

namespace Lostics.NCryptoExchange.CoinsE
{
    public class CoinsEMarketTrade : MarketTrade<CoinsEMarketId>
    {
        public CoinsEMarketTrade(TradeId tradeId, OrderType tradeType,
            DateTime dateTime, decimal price,
            decimal quantity, decimal fee,
            CoinsEMarketId marketId)
            : base(tradeId, tradeType, dateTime, price, quantity, fee, marketId)
        {
        }

        public static CoinsEMarketTrade Parse(JObject jObject, CoinsEMarketId marketId, TimeZoneInfo defaultTimeZone)
        {
            throw new NotImplementedException();
        }
    }
}
