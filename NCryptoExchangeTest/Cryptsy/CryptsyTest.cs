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
        public void GenerateAccountInfoRequestTest()
        {
            CryptsyExchange cryptsy = new CryptsyExchange("64d00dc4ee1c2b9551eabbdc831972d4ce2bcac5", "topsecret");
            Task<FormUrlEncodedContent> request = cryptsy.GenerateAccountInfoRequest();

            request.Wait();

            Task<string> content = request.Result.ReadAsStringAsync();

            content.Wait();

            System.Console.Write(content.Result);
            System.Console.Write(request.Result.Headers.ToString());
        }

        [TestMethod]
        public void GetRawAccountInfoTest()
        {
            CryptsyExchange cryptsy = new CryptsyExchange("64d00dc4ee1c2b9551eabbdc831972d4ce2bcac5", "topsecret");
            Task<string> response = cryptsy.GetRawAccountInfo();

            response.Wait();

            System.Console.Write(response.Result);
        }
    }
}
