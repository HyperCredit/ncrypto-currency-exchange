using Lostics.NCryptoExchange.Prelude;
using Lostics.NCryptoExchange.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace Lostics.NCryptoExchange
{
    [TestClass]
    public class PreludeTest
    {
        [TestMethod]
        public void TestBuildPreludePublicUrl()
        {
            PreludeMarketId marketId = new PreludeMarketId("DOGE", PreludeQuoteCurrency.USD);
            string url = PreludeExchange.BuildPublicUrl(PreludeExchange.Method.combined, marketId.QuoteCurrencyCode);

            Assert.AreEqual("https://api.prelude.io/combined-usd/", url);
        }

        [TestMethod]
        public void TestBuildPreludePairingsUrl()
        {
            PreludeMarketId marketId = new PreludeMarketId("DOGE", PreludeQuoteCurrency.USD);
            string url = PreludeExchange.BuildPublicUrl(PreludeExchange.Method.pairings, marketId.QuoteCurrencyCode);

            Assert.AreEqual("https://api.prelude.io/pairings/usd", url);
        }

        [TestMethod]
        public void TestParsePreludeMarketPairs()
        {
            JObject marketsJson = LoadTestData<JObject>("pairings.json");
            List<MarketId> markets = PreludeMarketId.ParsePairs(marketsJson, PreludeQuoteCurrency.BTC);

            Assert.AreEqual(15, markets.Count);

            Assert.AreEqual("AUR_BTC", markets[0].ToString());
            Assert.AreEqual("DRK_BTC", markets[1].ToString());
            Assert.AreEqual("DGB_BTC", markets[2].ToString());
            Assert.AreEqual("DGC_BTC", markets[3].ToString());
            Assert.AreEqual("DOGE_BTC", markets[4].ToString());
        }

        [TestMethod]
        public void TestParsePreludeMarketTrades()
        {
            JObject tradesJson = LoadTestData<JObject>("last.json");
            PreludeMarketId marketId = new PreludeMarketId("DOGE", PreludeQuoteCurrency.BTC);
            List<MarketTrade> trades = PreludeMarketTrade.Parse(marketId, tradesJson);

            Assert.AreEqual(25, trades.Count);
            Assert.AreEqual(new DateTime(2014, 4, 4, 19, 48, 34), trades[0].DateTime);
            Assert.AreEqual(1000m, trades[0].Quantity);
            Assert.AreEqual(0.00000103m, trades[0].Price);

            Assert.AreEqual(new DateTime(2014, 4, 4, 19, 21, 29), trades[1].DateTime);
            Assert.AreEqual(42326.92307692m, trades[1].Quantity);
            Assert.AreEqual(0.00000104m, trades[1].Price);
        }

        [TestMethod]
        public void TestParsePreludeOrderBook()
        {
            JObject orderBookJson = LoadTestData<JObject>("combined.json");
            Book orderBook = PreludeParsers.ParseOrderBook(orderBookJson);
            List<MarketDepth> asks = orderBook.Asks;
            List<MarketDepth> bids = orderBook.Bids;

            Assert.AreEqual(0.00000104m, asks[0].Price);
            Assert.AreEqual(109706.61183787m, asks[0].Quantity);
            Assert.AreEqual(0.00000105m, asks[1].Price);
            Assert.AreEqual(4403622.93269661m, asks[1].Quantity);

            Assert.AreEqual(0.00000103m, bids[0].Price);
            Assert.AreEqual(1250m, bids[0].Quantity);
            Assert.AreEqual(0.00000102m, bids[1].Price);
            Assert.AreEqual(2881582.99019606m, bids[1].Quantity);
        }

        private T LoadTestData<T>(string filename)
            where T : JToken
        {
            return TestUtils.LoadTestData<T>("Prelude", filename);
        }
    }
}
