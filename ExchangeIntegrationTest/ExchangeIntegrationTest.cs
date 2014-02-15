using Lostics.NCryptoExchange.Model;
using Lostics.NCryptoExchange.Vircurex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExchangeIntegrationTest
{
    public class ExchangeIntegrationTest
    {
        public static void Main(string[] argv)
        {
            using (VircurexExchange vircurex = new VircurexExchange())
            {
                vircurex.PublicKey = "rnicoll";
                vircurex.DefaultPrivateKey = "topsecret";

                vircurex.GetMyActiveOrders(VircurexExchange.OrderReleased.Released).Wait();
                VircurexMarketId marketId = new VircurexMarketId("VTC", "BTC");
                VircurexOrderId unreleasedOrderId = vircurex.CreateUnreleasedOrder(marketId,
                    OrderType.Sell, 1m, 0.005m).Result;
                VircurexOrderId releasedOrderId = vircurex.ReleaseOrder(unreleasedOrderId).Result;

            }
        }
    }
}
