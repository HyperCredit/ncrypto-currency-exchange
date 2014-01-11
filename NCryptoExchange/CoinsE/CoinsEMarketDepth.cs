using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.CoinsE
{
    public class CoinsEMarketDepth : MarketDepth
    {
        public CoinsEMarketDepth(int setNumberOfOrders,
            decimal setPrice, decimal setQuantity, decimal setCummulativeQuantity)
            : base(setPrice, setQuantity)
        {
            this.NumberOfOrders = setNumberOfOrders;
            this.CummulativeQuantity = setCummulativeQuantity;
        }

        /// <summary>
        /// Parse a market order out of a market depth list
        /// </summary>
        /// <param name="depthJson"></param>
        /// <returns></returns>
        public static MarketDepth ParseMarketDepth(JObject depthJson)
        {
            return new CoinsEMarketDepth(depthJson.Value<int>("n"),
                depthJson.Value<decimal>("r"), depthJson.Value<decimal>("q"),
                depthJson.Value<decimal>("cq"));
        }

        public decimal CummulativeQuantity { get; private set; }
        public int NumberOfOrders { get; private set; }
    }
}
