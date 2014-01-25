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
        public void TestBuildPublicUrl()
        {
            string url = BterExchange.BuildPublicUrl(BterExchange.Method.pairs);

            Assert.AreEqual("http://bter.com/api/1/pairs", url);
        }

        [TestMethod]
        public void TestParseMarketPairs()
        {
            JArray pairsJson = LoadTestData<JArray>("pairs.json");
            List<BterMarketId> pairs = BterMarketId.ParsePairs(pairsJson);

            Assert.AreEqual(76, pairs.Count);
            
            Assert.AreEqual(pairs[0].ToString(), "btc_cny");
            Assert.AreEqual(pairs[1].ToString(), "ltc_cny");
            Assert.AreEqual(pairs[2].ToString(), "bqc_cny");
        }

        [TestMethod]
        public void TestParseMarketData()
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
        public void TestParseMarketTrades()
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

        /* [TestMethod]
        public void TestParseOrderBook()
        {
            JObject orderBookJson = LoadTestDataObject("orderbook.json");
            Book orderBook = BterParsers.ParseMarketOrders(orderBookJson);
            List<MarketDepth> asks = orderBook.Asks;
            List<MarketDepth> bids = orderBook.Bids;

            Assert.AreEqual(asks[0].Price, (decimal)0.00000038);
            Assert.AreEqual(asks[0].Quantity, (decimal)156510.61595001);

            Assert.AreEqual(bids[0].Price, (decimal)0.00000037);
            Assert.AreEqual(bids[0].Quantity, (decimal)2295316.39314516);
        } */

        private T LoadTestData<T>(string filename)
            where T : JToken
        {
            return TestUtils.LoadTestData<T>("Bter", filename);
        }
    }
}
