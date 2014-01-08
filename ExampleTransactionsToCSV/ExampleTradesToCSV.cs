using Lostics.NCryptoExchange.Cryptsy;
using Lostics.NCryptoExchange.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExample
{
    public class ExampleTradesToCSV
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

            foreach (MyTrade<CryptsyMarketId, CryptsyOrderId> trade in cryptsy.GetAllMyTrades(null).Result)
            {
                Console.WriteLine(trade.TradeId.ToString() + ","
                    + Enum.GetName(typeof(OrderType), trade.TradeType)
                    );
            }

            Console.WriteLine("Requests completed, press any key to exit.");
            Console.ReadKey();
        }
    }
}
