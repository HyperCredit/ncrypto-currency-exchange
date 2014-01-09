using Lostics.NCryptoExchange.CoinsE;
using Lostics.NCryptoExchange.Cryptsy;
using Lostics.NCryptoExchange.Model;
using Lostics.NCryptoExchange;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExample
{
    public class ExampleTradesToCSV
    {
        public const string COINS_E_CONFIG_FILENAME = "coins_e.conf";
        public const string CRYPTSY_CONFIG_FILENAME = "cryptsy.conf";

        public static FileInfo GetCryptsyConfigurationFile()
        {
            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent;
            return new FileInfo(Path.Combine(dir.FullName, CRYPTSY_CONFIG_FILENAME));
        }

        public static FileInfo GetCoinsEConfigurationFile()
        {
            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent;
            return new FileInfo(Path.Combine(dir.FullName, COINS_E_CONFIG_FILENAME));
        }

        static void Main()
        {
            string filename = DateTime.Now.ToFileTime() + ".csv";
            FileInfo file = new FileInfo(
                Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName,
                filename)
            );

            using (CryptsyExchange cryptsy = CryptsyExchange.GetExchange(GetCryptsyConfigurationFile()))
            {
                using (CoinsEExchange coinsE = CoinsEExchange.GetExchange(GetCoinsEConfigurationFile()))
                {
                    using (FileStream stream = new FileStream(file.FullName, FileMode.CreateNew))
                    {
                        using (TextWriter writer = new StreamWriter(stream))
                        {
                            writer.WriteLine("Exchange,Trade ID,Date,Order,Market,Price,Quantity,Fee");
                            WriteTradesToCSV(writer, cryptsy);
                            WriteTradesToCSV(writer, coinsE);
                        }
                    }
                }
            }

            Console.WriteLine("Requests completed, press any key to exit.");
            Console.ReadKey();
        }

        private static void WriteTradesToCSV(TextWriter writer, AbstractExchange exchange)
        {
            foreach (MyTrade trade in exchange.GetAllMyTrades(null).Result)
            {
                writer.WriteLine(exchange.Label + ","
                    + trade.TradeId.ToString() + ","
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
