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
            string filename = DateTime.Now.ToFileTime() + ".csv";
            FileInfo file = new FileInfo(
                Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName,
                filename)
            );

            using (FileStream stream = new FileStream(file.FullName, FileMode.CreateNew))
            {
                using (TextWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine("Trade ID, Date, Order, Market, Price, Quantity, Fee");
                    WriteTradesToCSV(writer, cryptsy, cryptsy.GetAllMyTrades(null).Result);
                }
            }

            Console.WriteLine("Requests completed, press any key to exit.");
            Console.ReadKey();
        }

        private static void WriteTradesToCSV(TextWriter writer, CryptsyExchange exchange,
            List<MyTrade<CryptsyMarketId, CryptsyOrderId>> trades)
        {
            foreach (MyTrade<CryptsyMarketId, CryptsyOrderId> trade in trades)
            {
                writer.WriteLine(trade.TradeId.ToString() + ","
                    + trade.DateTime + ","
                    + Enum.GetName(typeof(OrderType), trade.TradeType) + ","
                    + trade.MarketId + ","
                    + trade.Price + ","
                    + trade.Quantity + ","
                    + trade.Fee
                );
            }
        }
    }
}
