using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lostics.NCryptoExchange.Cryptsy;
using System.Threading.Tasks;
using System.Net.Http;

namespace Lostics.NCryptoExchangeTest.Cryptsy
{
    [TestClass]
    public class CryptsyTest
    {
        [TestMethod]
        public void SignRequestTest()
        {
            CryptsyExchange cryptsy = new CryptsyExchange("64d00dc4ee1c2b9551eabbdc831972d4ce2bcac5",
                "topsecret");

            string request = "method=getinfo&nonce=1388246959";
            string expected = "6dd05bfe3104a70768cf76a30474176db356818d3556e536c31d158fc2c3adafa096df144b46b2ccb1ff6128d6a0a07746695eca061547b25fd676c9614e6718";
            string actual = cryptsy.GenerateSignature(request);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetRawAccountInfoTest()
        {
            CryptsyExchange cryptsy = new CryptsyExchange("64d00dc4ee1c2b9551eabbdc831972d4ce2bcac5",
                "topsecret");
            Task<string> response = cryptsy.GetRawAccountInfo();

            response.Wait();

            System.Console.Write(response.Result);
        }
    }
}
