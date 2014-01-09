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
            List<VircurexCurrency> currencies = VircurexCurrency.Parse(jsonObj);
            
            Assert.AreEqual(19, currencies.Count);

            Assert.AreEqual("BTC", currencies[0].CurrencyCode);
            Assert.AreEqual("Bitcoin", currencies[0].Label);
            Assert.AreEqual(4, currencies[0].Confirmations);

            Assert.AreEqual("NMC", currencies[1].CurrencyCode);
            Assert.AreEqual("Namecoin", currencies[1].Label);
        }

        [TestMethod]
        public void TestParseMarketData()
        {
            JObject currenciesJson = LoadTestData("get_currency_info.json");
            List<VircurexCurrency> currencies = VircurexCurrency.Parse(currenciesJson);
            Dictionary<string, string> currencyShortCodeToLabel = new Dictionary<string, string>();

            foreach (VircurexCurrency currency in currencies) {
                currencyShortCodeToLabel.Add(currency.CurrencyCode, currency.Label);
            }

            JObject marketsJson = LoadTestData("get_info_for_currency.json");
            List<Market> markets = new List<Market>();

            foreach (JProperty baseProperty in marketsJson.Properties())
            {
                string baseCurrency = baseProperty.Name;

                foreach (JProperty quoteProperty in (baseProperty.Value as JObject).Properties())
                {
                    markets.Add(VircurexMarket.Parse(currencyShortCodeToLabel, baseCurrency, quoteProperty));
                }
            }

            Assert.AreEqual(342, markets.Count);

            Assert.AreEqual(markets[0].Label, "ANC/BTC");
            Assert.AreEqual(markets[0].BaseCurrencyCode, "ANC");
            Assert.AreEqual(markets[0].BaseCurrencyName, "Anoncoin");
            Assert.AreEqual(markets[0].Statistics.Volume24HBase, (decimal)2089.56097032);
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
