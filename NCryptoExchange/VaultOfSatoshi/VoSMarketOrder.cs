using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;

namespace Lostics.NCryptoExchange.VaultOfSatoshi
{
    public class VoSMarketOrder : MarketOrder
    {
        public VoSMarketOrder(VoSOrderId orderId,
            OrderType orderType, decimal price, decimal totalQuantity,
            decimal filled, DateTime createdAt, DateTime updatedAt)
            : base (orderType, price, totalQuantity - filled)
        {
            this.OrderId = orderId;
            this.TotalQuantity = totalQuantity;
            this.Filled = filled;
            this.CreatedAt = createdAt;
            this.UpdatedAt = updatedAt;
        }

        public static VoSMarketOrder Parse(JObject orderJson)
        {
            VoSOrderId orderId = new VoSOrderId(orderJson.Value<int>("id"));
            OrderType orderType = orderJson.Value<bool>("bid")
                ? OrderType.Buy
                : OrderType.Sell;

            return new VoSMarketOrder(orderId, orderType,
                (orderJson.Value<long>("rate") * VoSExchange.PRICE_UNIT), orderJson.Value<decimal>("amount"),
                orderJson.Value<decimal>("filled"), orderJson.Value<DateTime>("created_at"),
                orderJson.Value<DateTime>("updated_at"));
        }

        public VoSOrderId OrderId { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal Filled { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
    }
}
