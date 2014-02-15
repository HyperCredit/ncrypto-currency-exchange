using System;

using Lostics.NCryptoExchange.Model;
using Lostics.NCryptoExchange.Vircurex;

namespace ExchangeIntegrationTest
{
    public class ExchangeIntegrationTest
    {
        public static void Main(string[] argv)
        {
            TestVircurex();

            Console.WriteLine("Press any key to quit.");

            Console.ReadKey();
        }

        private static void TestVircurex()
        {
            using (VircurexExchange vircurex = new VircurexExchange())
            {
                Wallet btcWallet = null;

                vircurex.PublicKey = "rnicoll";
                vircurex.DefaultPrivateKey = "topsecret";

                vircurex.GetMyActiveOrders(VircurexExchange.OrderReleased.Released).Wait();

                // Try creating, releasing then deleting an order
                VircurexMarketId marketId = new VircurexMarketId("VTC", "BTC");
                VircurexOrderId unreleasedOrderId = vircurex.CreateUnreleasedOrder(marketId,
                    OrderType.Sell, 1m, 0.005m).Result;
                VircurexOrderId releasedOrderId = vircurex.ReleaseOrder(unreleasedOrderId).Result;
                vircurex.CancelOrder(releasedOrderId).Wait();

                foreach (Wallet wallet in vircurex.GetAccountInfo().Result.Wallets)
                {
                    if (wallet.CurrencyCode == "BTC")
                    {
                        btcWallet = wallet;
                    }
                }

                if (null == btcWallet)
                {
                    Console.WriteLine("BTC wallet not found.");
                }
                else
                {
                    Console.WriteLine("BTC balance: "
                        + btcWallet.Balance + "BTC");
                }

                // vircurex.GetOrderExecutions(new VircurexOrderId(5)).Wait();
            }
        }
    }
}
