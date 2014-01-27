using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.VaultOfSatoshi
{
    public class VoSMarketTrade : MarketTrade
    {
        public VoSMarketTrade(TradeId tradeId, OrderType tradeType,
            DateTime createdAt, decimal price,
            decimal quantity, MarketId marketId)
            : base(tradeId, createdAt, price, quantity, marketId)
        {
            this.TradeType = tradeType;
        }

        public static VoSMarketTrade Parse(MarketId marketId, JObject trade)
        {
            OrderType orderType;

            if (trade.Value<bool>("bid"))
            {
                orderType = OrderType.Buy;
            }
            else
            {
                orderType = OrderType.Sell;
            }

            return new VoSMarketTrade(new VoSTradeId(trade.Value<int>("id")), orderType,
                trade.Value<DateTime>("created_at"), trade.Value<long>("rate") * VoSExchange.PRICE_UNIT,
                trade.Value<decimal>("amount"), marketId);
        }

        public OrderType TradeType { get; private set; }
    }
}
