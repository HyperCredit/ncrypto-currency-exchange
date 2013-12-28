using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lostics.NCryptoExchange.Cryptsy;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchangeTest.Cryptsy
{
    [TestClass]
    public class CryptsyTest
    {
        [TestMethod]
        public void GetRawAccountInfoTest()
        {
            CryptsyExchange cryptsy = new CryptsyExchange("64d00dc4ee1c2b9551eabbdc831972d4ce2bcac5", "topsecret");

            Task<string> accountInfo = cryptsy.GetRawAccountInfo();

            accountInfo.Wait();

            System.Console.Write(accountInfo.Result);
        }
    }
}
