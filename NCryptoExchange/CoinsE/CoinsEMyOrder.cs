using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;

namespace Lostics.NCryptoExchange.CoinsE
{
    public class CoinsEMyOrder : MyOrder<CoinsEMarketId, CoinsEOrderId>
    {
        public CoinsEMyOrder(CoinsEOrderId orderId, OrderType orderType,
            DateTime created, decimal price, decimal quantity, decimal originalQuantity,
            CoinsEMarketId marketId) : base(orderId, orderType, created, price, quantity, originalQuantity, marketId)
        {
        }

        public static CoinsEMyOrder Parse(JObject json)
        {
            DateTime dateTime = CoinsEParsers.ParseTime(json.Value<int>("created"));

            return new CoinsEMyOrder(new CoinsEOrderId(json.Value<string>("id")),
                CoinsEParsers.ParseOrderType(json.Value<string>("order_type")), dateTime,
                json.Value<decimal>("rate"), json.Value<decimal>("quantity_remaining"), json.Value<decimal>("quantity"),
                new CoinsEMarketId(json.Value<string>("pair"))
            );
        }

        public Boolean IsOpen { get; private set; }
        public string Status { get; private set; }
    }
}
