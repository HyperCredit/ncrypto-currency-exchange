using Lostics.NCryptoExchange.VaultOfSatoshi;
using Lostics.NCryptoExchange.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Net.Http;

namespace Lostics.NCryptoExchange
{
    [TestClass]
    public class VaultOfSatoshiTest
    {
        [TestMethod]
        public void TestGetNextNonce()
        {
            using (VoSExchange exchange = new VoSExchange())
            {
                string nonce = exchange.GetNextNonce();

                // Should test we're within 10 seconds of local clock
            }
        }

        [TestMethod]
        public void TestFormatAsCurrencyObject()
        {
            using (VoSExchange exchange = new VoSExchange())
            {
                string actual = exchange.FormatAsCurrencyObject(0.00000132m, 8);
                string expected = "{\"precision\":8,\"value_int\":132,\"value\":\"0.00000132\"}";

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void TestGenerateMessageToSign()
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("order_currency", "BTC"),
                new KeyValuePair<string, string>("payment_currency", "USD"),
                new KeyValuePair<string, string>(VoSExchange.PARAMETER_NONCE, "1391466805814182")
            });

            using (VoSExchange exchange = new VoSExchange())
            {
                byte[] message = exchange.GenerateMessageToSign(VoSExchange.Method.ticker, request);

                Assert.AreEqual("/info/ticker\0order_currency=BTC&payment_currency=USD&nonce=1391466805814182",
                    System.Text.Encoding.ASCII.GetString(message));
            }
        }

        [TestMethod]
        public void TestGenerateSHA512Signature()
        {
            string publicKey = "mypublickey";
            string privateKey = "topsecret";

            FormUrlEncodedContent request = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("order_currency", "BTC"),
                new KeyValuePair<string, string>("payment_currency", "USD"),
                new KeyValuePair<string, string>(VoSExchange.PARAMETER_NONCE, "1391467880847682")
            });

            using (VoSExchange exchange = new VoSExchange()
                {
                    PublicKey = publicKey,
                    PrivateKey = privateKey
                }
            )
            {
                Assert.AreEqual(privateKey, System.Text.Encoding.ASCII.GetString(exchange.PrivateKeyBytes));

                string signature = exchange.GenerateSHA512Signature(VoSExchange.Method.ticker, request);

                Console.WriteLine(signature);

                Assert.AreEqual("OWVmM2NkMDA2ZjI2NzBhYzgyNDU0OWRjZWMxOGUwNmRhODE2OGMzZjM1MDMzNzc2MjI5NmE0ZjVmYzM3NDIzMTlmODZjODZkMDVjZDY0Y2UzMzQzM2Y5ZGYxNTM0YjNjMDkzODA5ZGFiNzAyZmVmYjgwMTQ2NTdkYjE1Yzk2ZDk=",
                    signature);
            }
        }

        [TestMethod]
        public void TestParseVoSMarketDepth()
        {
            JObject jsonObj = LoadTestData("orderbook.json");
            Book book = VoSParsers.ParseOrderBook(jsonObj.Value<JObject>("data"));

            Assert.AreEqual(100, book.Asks.Count);
            Assert.AreEqual(100, book.Bids.Count);

            Assert.AreEqual(9.65879994m, book.Asks[0].Quantity);
            Assert.AreEqual(781m, book.Asks[0].Price);
            Assert.AreEqual(9.45894400m, book.Bids[0].Quantity);
            Assert.AreEqual(776m, book.Bids[0].Price);
        }

        private JObject LoadTestData(string filename)
        {
            return TestUtils.LoadTestData<JObject>("Vault of Satoshi", filename);
        }
    }
}
