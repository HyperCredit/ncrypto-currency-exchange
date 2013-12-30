using System;
using System.Collections.Generic;

using Lostics.NCryptoExchange.Cryptsy;
using Lostics.NCryptoExchange.Model;
using Lostics.NCryptoExchange;

namespace Lostics.NCryptoExchangeExamples.Cryptsy
{
    public class GetMarkets : AbstractCryptsyExample
    {
        static void Main()
        {
            try
            {
                List<Market<CryptsyMarketId>> markets;

                using (CryptsyExchange cryptsy = GetExchange())
                {
                    markets = cryptsy.GetMarkets().Result;
                }

                foreach (Market<CryptsyMarketId> market in markets)
                {
                    Console.WriteLine(market.ToString());
                }
            }
            catch (ConfigurationException e)
            {
                Console.Error.WriteLine(e.Message);
            }

            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
