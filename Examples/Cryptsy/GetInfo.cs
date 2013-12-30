using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Lostics.NCryptoExchange.Cryptsy;
using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchangeExamples.Cryptsy
{
    public class GetInfo
    {
        static void Main()
        {
            Task<AccountInfo<Wallet>> response;

            using (CryptsyExchange cryptsy = GetExchange())
            {
                response = cryptsy.GetAccountInfo();
                response.Wait();
            }

            Console.WriteLine(response.Result.ToString());

            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static CryptsyExchange GetExchange()
        {
            return new CryptsyExchange("publicKey",
                "privateKey");
        }
    }
}
