using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lostics.NCryptoExchange.VaultOfSatoshi
{
    public class VoSMyOrder : MyOrder
    {
        public VoSMyOrder(OrderId orderId, OrderType orderType,
            DateTime created, decimal price, decimal quantity, decimal originalQuantity,
            MarketId marketId) : base(orderId, orderType, created, price, quantity, originalQuantity, marketId)
        {
        }

        public static List<MyOrder> Parse(JArray ordersJson)
        {
            throw new NotImplementedException();
        }
    }
}
