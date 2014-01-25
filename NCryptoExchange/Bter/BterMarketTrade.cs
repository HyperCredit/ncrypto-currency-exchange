using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Bter
{
    public class BterMarketTrade : MarketTrade
    {
        public BterMarketTrade(TradeId tradeId, OrderType tradeType,
            DateTime dateTime, decimal price,
            decimal quantity, MarketId marketId)
            : base(tradeId, dateTime, price, quantity, marketId)
        {
            this.TradeType = tradeType;
        }

        public static List<MarketTrade> Parse(MarketId marketId, JObject tradesJson)
        {
            return tradesJson.Value<JArray>("data").Select(
                trade => (MarketTrade)ParseSingle(marketId, (JObject)trade)
            ).ToList();
        }

        internal static BterMarketTrade ParseSingle(MarketId marketId, JObject trade)
        {
            OrderType orderType;

            switch (trade.Value<string>("type"))
            {
                case "buy":
                    orderType = OrderType.Buy;
                    break;
                case "sell":
                    orderType = OrderType.Sell;
                    break;
                default:
                    throw new BterResponseException("Found unknown trade type \""
                        + trade.Value<string>("type") + "\", expected \"buy\" or \"sell\".");
            }

            return new BterMarketTrade(new BterTradeId(trade.Value<int>("tid")), orderType,
                BterParsers.ParseDateTime(trade.Value<int>("date")), trade.Value<decimal>("price"),
                trade.Value<decimal>("amount"), marketId);
        }

        public OrderType TradeType { get; private set; }
        public decimal Fee { get; private set; }
    }
}
