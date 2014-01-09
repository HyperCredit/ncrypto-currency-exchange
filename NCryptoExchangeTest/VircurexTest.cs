using Lostics.NCryptoExchange.Vircurex;
using Lostics.NCryptoExchange.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lostics.NCryptoExchange
{
    [TestClass]
    public class VircurexTest
    {
        [TestMethod]
        public void TestParseCoins()
        {
            JObject jsonObj = LoadTestData("get_currency_info.json");
            List<VircurexCurrency> coins = VircurexCurrency.Parse(jsonObj);
            
            Assert.AreEqual(19, coins.Count);

            Assert.AreEqual("BTC", coins[0].CurrencyCode);
            Assert.AreEqual("Bitcoin", coins[0].Label);
            Assert.AreEqual(4, coins[0].Confirmations);

            Assert.AreEqual("NMC", coins[1].CurrencyCode);
            Assert.AreEqual("Namecoin", coins[1].Label);
        }

        [TestMethod]
        public void TestParseMarketData()
        {
            Dictionary<string, string> currencyCodesToLabel = new Dictionary<string, string>()
            {
                {"BTC", "bitcoin"},
                {"RED", "redcoin"}
            };

            JObject jsonObj = LoadTestData("market_data.json");
            JObject marketsJson = jsonObj.Value<JObject>("markets");
            List<VircurexMarket> markets = marketsJson.Properties().Select(
                market => VircurexMarket.Parse(currencyCodesToLabel, "RED_BTC",
                    market.Value as JProperty)
            ).ToList();

            Assert.AreEqual(1, markets.Count);

            Assert.AreEqual(markets[0].Label, "RED_BTC");
            Assert.AreEqual(markets[0].BaseCurrencyCode, "RED");
            Assert.AreEqual(markets[0].BaseCurrencyName, "redcoin");
            Assert.AreEqual(markets[0].Statistics.Volume24HBase, (decimal)6396.70000000);
            Assert.AreEqual(markets[0].QuoteCurrencyCode, "BTC");
        }

        private JObject LoadTestData(string filename)
        {
            string testDir = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string testDataDir = Path.Combine(Path.Combine(testDir, "Sample_Data"), "Vircurex");
            FileInfo fileName = new FileInfo(Path.Combine(testDataDir, filename));

            using (StreamReader reader = new StreamReader(new FileStream(fileName.FullName, FileMode.Open)))
            {
                return JObject.Parse(reader.ReadToEndAsync().Result);
            }
        }
    }
}
