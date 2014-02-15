using System;

using Lostics.NCryptoExchange.Model;
using Lostics.NCryptoExchange.Vircurex;
using Lostics.NCryptoExchange.Cryptsy;
using System.IO;
using System.Collections.Generic;

namespace ExchangeIntegrationTest
{
    public class ExchangeIntegrationTest
    {
        public static void Main(string[] argv)
        {
            TestCryptsy();
            // TestVircurex();

            Console.WriteLine("Press any key to quit.");

            Console.ReadKey();
        }

        /// <summary>
        /// Rough test which finds market IDs, then fetches recent transactions and dumps
        /// them to CSV.
        /// </summary>
        private static void TestCryptsy()
        {
            string fileName = System.IO.Path.GetTempFileName();
            fileName = fileName.Replace(".tmp", ".csv");
            Dictionary<string, CryptsyMarket> markets = new Dictionary<string, CryptsyMarket>();
            string[] marketCodes = new string[] {
                "DOGE/BTC",
                "LTC/BTC",
                "QRK/BTC",
                "VTC/BTC"
            };

            using (StreamWriter writer = new StreamWriter(new FileStream(fileName, FileMode.Create)))
            {
                using (CryptsyExchange cryptsy = new CryptsyExchange())
                {
                    cryptsy.PublicKey = "";
                    cryptsy.PrivateKey = "topsecret";

                    foreach (Market market in cryptsy.GetMarkets().Result)
                    {
                        CryptsyMarket cryptsyMarket = (CryptsyMarket)market;
                        markets[cryptsyMarket.BaseCurrencyCode + "/"
                            + cryptsyMarket.QuoteCurrencyCode] = cryptsyMarket;
                    }

                    foreach (string marketCode in marketCodes)
                    {
                        CryptsyMarket market = markets[marketCode];

                        writer.WriteLine("Date,Time,Side,Trade ID,Exchange,Base currency,Quantity,Quote currency,Price,Cost,Fee,Fee currency");

                        foreach (MyTrade trade in cryptsy.GetMyTrades(market.MarketId, null).Result)
                        {
                            System.Text.StringBuilder line = new System.Text.StringBuilder();

                            line.Append(trade.DateTime.ToString("yyyy-MM-dd")).Append(",")
                                .Append(trade.DateTime.ToString("HH:mm:ss")).Append(",")
                                .Append(Enum.GetName(typeof(OrderType), trade.TradeType)).Append(",")
                                .Append(trade.OrderId).Append(",")
                                .Append("Cryptsy").Append(",")
                                .Append(market.BaseCurrencyCode).Append(",")
                                .Append(trade.Quantity).Append(",")
                                .Append(market.QuoteCurrencyCode).Append(",")
                                .Append(trade.Price).Append(",")
                                .Append(",")
                                .Append(trade.Fee).Append(",")
                                .Append(market.QuoteCurrencyCode);

                            writer.WriteLine(line.ToString());
                        }
                    }
                }
            }
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
