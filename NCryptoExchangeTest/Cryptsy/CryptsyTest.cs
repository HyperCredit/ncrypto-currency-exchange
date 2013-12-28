using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lostics.NCryptoExchange.Cryptsy;
using System.Threading.Tasks;
using System.Net.Http;
using Lostics.NCryptoExchange.Model;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

            FormUrlEncodedContent request = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(CryptsyExchange.PARAM_METHOD, CryptsyExchange.METHOD_GET_INFO),
                new KeyValuePair<string, string>(CryptsyExchange.PARAM_NONCE, "1388246959")
            });
            string expected = "6dd05bfe3104a70768cf76a30474176db356818d3556e536c31d158fc2c3adafa096df144b46b2ccb1ff6128d6a0a07746695eca061547b25fd676c9614e6718";
            Task task = cryptsy.SignRequest(request);

            task.Wait();

            foreach (string actual in request.Headers.GetValues(CryptsyExchange.HEADER_SIGN))
            {
                Assert.AreEqual(expected, actual);
                return;
            }

            Assert.Fail("No signature header found.");
        }

        [TestMethod]
        public void CalculateFeesTest()
        {
            CryptsyExchange cryptsy = new CryptsyExchange("64d00dc4ee1c2b9551eabbdc831972d4ce2bcac5",
                "topsecret");
            Task<Fees> response = cryptsy.CalculateFees(OrderType.Buy, new Quantity(0.05), new Quantity(1.0));

            System.Console.Write(response.Result);
        }
    }
}
