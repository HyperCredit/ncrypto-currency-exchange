using Lostics.NCryptoExchange.Cryptsy;
using Lostics.NCryptoExchange.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace Lostics.NCryptoExchangeExamples.Cryptsy
{
    public class GetData
    {
        public const string CONFIG_FILENAME = "cryptsy.conf";

        public static FileInfo GetDefaultConfigurationFile()
        {
            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent;
            return new FileInfo(Path.Combine(dir.FullName, CONFIG_FILENAME));
        }

        static void Main()
        {
            CryptsyExchange cryptsy = CryptsyExchange.GetExchange(GetDefaultConfigurationFile());

            cryptsy.DumpResponse = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent;

            cryptsy.GetAccountInfo().Wait();
            List<Market<CryptsyMarketId>> markets = cryptsy.GetMarkets().Result;

            if (markets.Count > 0)
            {
                Book marketDepth = cryptsy.GetMarketDepth(markets[0].MarketId).Result;
                MarketOrders orders = cryptsy.GetMarketOrders(markets[0].MarketId).Result;
                List<MarketTrade<CryptsyMarketId, CryptsyTradeId>> trades = cryptsy.GetMarketTrades(markets[0].MarketId).Result;
            }

            cryptsy.GetAllMyOrders(null).Wait();
            cryptsy.GetAllMyTrades(null).Wait();
            cryptsy.CancelAllOrders().Wait();

            Console.WriteLine("Requests completed, press any key to exit.");
            Console.ReadKey();
        }
    }
}
