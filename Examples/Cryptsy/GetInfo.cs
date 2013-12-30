using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Lostics.NCryptoExchange.Cryptsy;
using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchangeExamples.Cryptsy
{
    public class GetInfo : AbstractCryptsyExample
    {
        static void Main()
        {
            try
            {
                Task<AccountInfo<Wallet>> response;

                using (CryptsyExchange cryptsy = GetExchange())
                {
                    response = cryptsy.GetAccountInfo();
                    response.Wait();
                }

                Console.WriteLine(response.Result.ToString());
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
