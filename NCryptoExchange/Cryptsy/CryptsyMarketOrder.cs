using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public class CryptsyMarketOrder : MarketOrder
    {
        public CryptsyMarketOrder(OrderType orderType, decimal price, decimal quantity) : base(orderType, price, quantity)
        {
        }

        public static CryptsyMarketOrder ParseBuy(JObject jsonOrder)
        {
            return new CryptsyMarketOrder(OrderType.Buy,
                jsonOrder.Value<decimal>("buyprice"), jsonOrder.Value<decimal>("quantity"));
        }

        public static CryptsyMarketOrder ParseSell(JObject jsonOrder)
        {
            return new CryptsyMarketOrder(OrderType.Sell,
                jsonOrder.Value<decimal>("sellprice"), jsonOrder.Value<decimal>("quantity"));
        }
    }
}
