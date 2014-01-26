using Lostics.NCryptoExchange.Bter;
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
    public class BterTest
    {
        [TestMethod]
        public void TestBuildBterPublicUrl()
        {
            string url = BterExchange.BuildPublicUrl(BterExchange.Method.pairs);

            Assert.AreEqual("http://bter.com/api/1/pairs", url);
        }

        [TestMethod]
        public void TestParseBterMarketPairs()
        {
            JArray pairsJson = LoadTestData<JArray>("pairs.json");
            List<BterMarketId> pairs = BterMarketId.ParsePairs(pairsJson);

            Assert.AreEqual(76, pairs.Count);
            
            Assert.AreEqual(pairs[0].ToString(), "btc_cny");
            Assert.AreEqual(pairs[1].ToString(), "ltc_cny");
            Assert.AreEqual(pairs[2].ToString(), "bqc_cny");
        }

        [TestMethod]
        public void TestParseBterMarketData()
        {
            JObject marketsJson = LoadTestData<JObject>("tickers.json");
            List<Market> markets = BterMarket.ParseMarkets(marketsJson);

            Assert.AreEqual(75, markets.Count);

            Assert.AreEqual(markets[0].Label, "btc_cny");
            Assert.AreEqual(markets[0].BaseCurrencyCode, "BTC");
            Assert.AreEqual(markets[0].Statistics.Volume24HBase, (decimal)1098.2957);
            Assert.AreEqual(markets[0].QuoteCurrencyCode, "CNY");
        }

        [TestMethod]
        public void TestParseBterMarketTrades()
        {
            JObject tradesJson = LoadTestData<JObject>("history_doge_btc.json");
            BterMarketId marketId = new BterMarketId("doge_btc");
            List<MarketTrade> trades = BterMarketTrade.Parse(marketId, tradesJson);

            Assert.AreEqual(40, trades.Count);
            Assert.AreEqual("3797993", trades[0].TradeId.ToString());
            Assert.AreEqual(new DateTime(2014, 1, 24, 22, 43, 46), trades[0].DateTime);
            Assert.AreEqual((decimal)5000, trades[0].Quantity);
            Assert.AreEqual((decimal)0.00000229, trades[0].Price);

            Assert.AreEqual("3797995", trades[1].TradeId.ToString());
            Assert.AreEqual(new DateTime(2014, 1, 24, 22, 43, 56), trades[1].DateTime);
            Assert.AreEqual((decimal)8215.336, trades[1].Quantity);
            Assert.AreEqual((decimal)0.00000229, trades[1].Price);
        }

        [TestMethod]
        public void TestParseBterOrderBook()
        {
            JObject orderBookJson = LoadTestData<JObject>("depth_doge_btc.json");
            Book orderBook = BterParsers.ParseOrderBook(orderBookJson);
            List<MarketDepth> asks = orderBook.Asks;
            List<MarketDepth> bids = orderBook.Bids;

            Assert.AreEqual(asks[0].Price, (decimal)0.00000238);
            Assert.AreEqual(asks[0].Quantity, (decimal)220397.66873897);
            Assert.AreEqual(asks[1].Price, (decimal)0.00000237);
            Assert.AreEqual(asks[1].Quantity, (decimal)2833885.131);

            Assert.AreEqual(bids[0].Price, (decimal)0.00000228);
            Assert.AreEqual(bids[0].Quantity, (decimal)69517.54691671);
            Assert.AreEqual(bids[1].Price, (decimal)0.00000227);
            Assert.AreEqual(bids[1].Quantity, (decimal)540711.166);
        }

        private T LoadTestData<T>(string filename)
            where T : JToken
        {
            return TestUtils.LoadTestData<T>("Bter", filename);
        }
    }
}
