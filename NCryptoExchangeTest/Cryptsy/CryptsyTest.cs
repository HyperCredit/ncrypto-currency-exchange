using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lostics.NCryptoExchange.Cryptsy;
using System.Threading.Tasks;
using System.Net.Http;
using Lostics.NCryptoExchange.Model;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Lostics.NCryptoExchangeTest.Cryptsy
{
    [TestClass]
    public class CryptsyTest
    {
        [TestMethod]
        public void TestParseMarkets()
        {
            JObject jsonObj = LoadTestData("getmarkets.json");
            List<Market<CryptsyMarketId>> markets = CryptsyParsers.ParseMarkets((JArray)jsonObj["return"],
                TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

            Assert.AreEqual(114, markets.Count);

            foreach (Market<CryptsyMarketId> market in markets) {
                // DOGE/BTC
                if (market.MarketId.ToString().Equals("132"))
                {
                    Assert.AreEqual(market.BaseCurrencyCode, "DOGE");
                    Assert.AreEqual(market.QuoteCurrencyCode, "BTC");
                }
            }
        }

        private JObject LoadTestData(string filename)
        {
            string testDir = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string testDataDir = Path.Combine(Path.Combine(testDir, "Sample_Data"), "Cryptsy");
            FileInfo fileName = new FileInfo(Path.Combine(testDataDir, filename));
            JObject jsonObj;

            using (StreamReader reader = new StreamReader(new FileStream(fileName.FullName, FileMode.Open)))
            {
                jsonObj = JObject.Parse(reader.ReadToEndAsync().Result);
            }

            return jsonObj;
        }

        [TestMethod]
        public void TestSignCryptsyRequest()
        {
            string privateKey = "topsecret";
            byte[] privateKeyBytes = System.Text.Encoding.ASCII.GetBytes(privateKey);
            string actual;
            string expected = "6dd05bfe3104a70768cf76a30474176db356818d3556e536c31d158fc2c3adafa096df144b46b2ccb1ff6128d6a0a07746695eca061547b25fd676c9614e6718";
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>(CryptsyExchange.PARAM_METHOD, Enum.GetName(typeof(CryptsyMethod), CryptsyMethod.getinfo)),
                    new KeyValuePair<string, string>(CryptsyExchange.PARAM_NONCE, "1388246959")
                });

            using (CryptsyExchange cryptsy = new CryptsyExchange("64d00dc4ee1c2b9551eabbdc831972d4ce2bcac5",
                "topsecret"))
            {
                actual = CryptsyExchange.GenerateSHA512Signature(request, privateKeyBytes).Result;
            }

            Assert.AreEqual(expected, actual);
        }
    }
}
