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

        /// <summary>
        /// Parse a market order from the Cryptsy buy order list
        /// </summary>
        /// <param name="jsonOrder"></param>
        /// <returns></returns>
        public static CryptsyMarketOrder ParseBuy(JObject jsonOrder)
        {
            return new CryptsyMarketOrder(OrderType.Buy,
                jsonOrder.Value<decimal>("buyprice"), jsonOrder.Value<decimal>("quantity"));
        }

        /// <summary>
        /// Parse a market order out of a market depth list
        /// </summary>
        /// <param name="orderType"></param>
        /// <param name="depthJson"></param>
        /// <returns></returns>
        public static MarketOrder ParseMarketDepth(JArray depthJson, OrderType orderType)
        {
            return new MarketOrder(orderType, depthJson[0].Value<decimal>(),
                    depthJson[1].Value<decimal>());
        }

        /// <summary>
        /// Parse a market order from the Cryptsy sell order list
        /// </summary>
        public static CryptsyMarketOrder ParseSell(JObject jsonOrder)
        {
            return new CryptsyMarketOrder(OrderType.Sell,
                jsonOrder.Value<decimal>("sellprice"), jsonOrder.Value<decimal>("quantity"));
        }
    }
}
