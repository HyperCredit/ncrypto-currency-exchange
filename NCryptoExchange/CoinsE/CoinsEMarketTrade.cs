using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;

namespace Lostics.NCryptoExchange.CoinsE
{
    public class CoinsEMarketTrade : MarketTrade<CoinsEMarketId>
    {
        public CoinsEMarketTrade(TradeId tradeId,
            DateTime dateTime, decimal price,
            decimal quantity, CoinsEMarketId marketId,
            CoinsEOrderNumber buyOrderId, CoinsEOrderNumber sellOrderId,
            string status)
            : base(tradeId, dateTime, price, quantity, marketId)
        {
            this.BuyOrderId = buyOrderId;
            this.SellOrderId = sellOrderId;
            this.Status = status;
        }

        public static CoinsEMarketTrade Parse(JObject jObject)
        {
            CoinsETradeId tradeId = new CoinsETradeId(jObject.Value<string>("id"));
            CoinsEMarketId marketId = new CoinsEMarketId(jObject.Value<string>("pair"));
            CoinsEOrderNumber buyOrderId = new CoinsEOrderNumber(jObject.Value<long>("buy_order_no"));
            CoinsEOrderNumber sellOrderId = new CoinsEOrderNumber(jObject.Value<long>("sell_order_no"));
            DateTime dateTime = CoinsEParsers.ParseTime(jObject.Value<int>("created"));
            
            return new CoinsEMarketTrade(tradeId, dateTime, jObject.Value<decimal>("rate"),
                jObject.Value<decimal>("quantity"), marketId,
                buyOrderId, sellOrderId, jObject.Value<string>("status"));
        }

        public CoinsEOrderNumber BuyOrderId { get; private set; }

        public CoinsEOrderNumber SellOrderId { get; private set; }

        public string Status { get; private set; }
    }
}
