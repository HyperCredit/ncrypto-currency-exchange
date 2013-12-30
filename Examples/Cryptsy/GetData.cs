using Lostics.NCryptoExchange.Cryptsy;
using System;
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
            cryptsy.GetMarkets().Wait();

            Console.WriteLine("Requests completed, press any key to exit.");
            Console.ReadKey();
        }
    }
}
