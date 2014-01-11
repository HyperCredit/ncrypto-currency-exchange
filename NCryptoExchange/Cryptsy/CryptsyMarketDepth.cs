using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public class CryptsyMarketDepth : MarketDepth
    {
        public CryptsyMarketDepth(decimal price, decimal quantity) : base(price, quantity)
        {
        }

        /// <summary>
        /// Parse a market order from the Cryptsy buy order list
        /// </summary>
        /// <param name="jsonOrder"></param>
        /// <returns></returns>
        public static CryptsyMarketDepth ParseBuy(JObject jsonOrder)
        {
            return new CryptsyMarketDepth(jsonOrder.Value<decimal>("buyprice"), jsonOrder.Value<decimal>("quantity"));
        }

        /// <summary>
        /// Parse a market order out of a market depth list
        /// </summary>
        /// <param name="orderType"></param>
        /// <param name="depthJson"></param>
        /// <returns></returns>
        public static CryptsyMarketDepth ParseMarketDepth(JArray depthJson)
        {
            return new CryptsyMarketDepth(depthJson[0].Value<decimal>(),
                    depthJson[1].Value<decimal>());
        }

        /// <summary>
        /// Parse a market order from the Cryptsy sell order list
        /// </summary>
        public static CryptsyMarketDepth ParseSell(JObject jsonOrder)
        {
            return new CryptsyMarketDepth(jsonOrder.Value<decimal>("sellprice"), jsonOrder.Value<decimal>("quantity"));
        }
    }
}
