using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public class CryptsyMarketTrade : MarketTrade<CryptsyMarketId>
    {
        public CryptsyMarketTrade(TradeId tradeId, OrderType tradeType,
            DateTime dateTime, decimal price,
            decimal quantity, decimal fee,
            CryptsyMarketId marketId)
            : base(tradeId, tradeType, dateTime, price, quantity, fee, marketId)
        {
        }

        public static CryptsyMarketTrade Parse(JObject jsonTrade, CryptsyMarketId defaultMarketId, TimeZoneInfo timeZone)
        {
            DateTime tradeDateTime = DateTime.Parse(jsonTrade.Value<string>("datetime"));
            JToken marketIdToken = jsonTrade["marketid"];
            CryptsyMarketId marketId = null == marketIdToken
                ? defaultMarketId
                : CryptsyMarketId.Parse(marketIdToken);
            CryptsyTradeId tradeId = CryptsyTradeId.Parse(jsonTrade["tradeid"]);
            OrderType orderType = (OrderType)Enum.Parse(typeof(OrderType), jsonTrade.Value<string>("initiate_ordertype"));

            tradeDateTime = TimeZoneInfo.ConvertTimeToUtc(tradeDateTime, timeZone);

            return new CryptsyMarketTrade(tradeId,
                orderType, tradeDateTime,
                jsonTrade.Value<decimal>("tradeprice"),
                jsonTrade.Value<decimal>("quantity"), jsonTrade.Value<decimal>("fee"),
                marketId
            );
        }
    }
}
