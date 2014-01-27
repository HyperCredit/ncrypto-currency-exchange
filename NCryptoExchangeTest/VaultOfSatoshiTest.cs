using Lostics.NCryptoExchange.VaultOfSatoshi;
using Lostics.NCryptoExchange.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lostics.NCryptoExchange
{
    [TestClass]
    public class VaultOfSatoshiTest
    {
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
