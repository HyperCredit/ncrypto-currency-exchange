using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.CoinEx
{
    public class CoinExMarketTrade : MarketTrade
    {
        public CoinExMarketTrade(TradeId tradeId, OrderType tradeType,
            DateTime createdAt, decimal price,
            decimal quantity, MarketId marketId)
            : base(tradeId, createdAt, price, quantity, marketId)
        {
            this.TradeType = tradeType;
        }

        public static CoinExMarketTrade Parse(MarketId marketId, JObject trade)
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

            return new CoinExMarketTrade(new CoinExTradeId(trade.Value<int>("id")), orderType,
                trade.Value<DateTime>("created_at"), trade.Value<decimal>("rate"),
                trade.Value<decimal>("amount"), marketId);
        }

        public OrderType TradeType { get; private set; }
    }
}
