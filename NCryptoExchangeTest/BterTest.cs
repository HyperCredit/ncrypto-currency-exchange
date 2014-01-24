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

            Assert.AreEqual("http://data.bter.com/api/1/pairs", url);
        }

        [TestMethod]
        public void TestParseMarketPairs()
        {
            JArray pairsJson = LoadTestDataArray("pairs.json");
            List<BterMarketId> pairs = BterMarketId.ParsePairs(pairsJson);

            Assert.AreEqual(76, pairs.Count);
            
            Assert.AreEqual(pairs[0].ToString(), "btc_cny");
            Assert.AreEqual(pairs[1].ToString(), "ltc_cny");
            Assert.AreEqual(pairs[2].ToString(), "bqc_cny");
        }

        [TestMethod]
        public void TestParseMarketData()
        {
            JObject marketsJson = LoadTestDataObject("tickers.json");
            List<Market> markets = BterMarket.ParseMarkets(marketsJson);

            Assert.AreEqual(76, markets.Count);

            Assert.AreEqual(markets[0].Label, "btc_cny");
            Assert.AreEqual(markets[0].BaseCurrencyCode, "BTC");
            Assert.AreEqual(markets[0].Statistics.Volume24HBase, (decimal)1098.2957);
            Assert.AreEqual(markets[0].QuoteCurrencyCode, "CNY");
        }

        /* [TestMethod]
        public void TestParseMarketTrades()
        {
            JArray tradesJson = LoadTestDataArray("trades.json");
            BterMarketId marketId = new BterMarketId("DOGE", "BTC");
            List<MarketTrade> trades = BterParsers.ParseMarketTrades(marketId, tradesJson);

            Assert.AreEqual(1000, trades.Count);
            Assert.AreEqual("1110350", trades[0].TradeId.ToString());
            Assert.AreEqual(new DateTime(2014, 1, 3, 11, 14, 42), trades[0].DateTime);
            Assert.AreEqual((decimal)1208.12173, trades[0].Quantity);
            Assert.AreEqual((decimal)0.00000043, trades[0].Price);

            Assert.AreEqual("1110352", trades[1].TradeId.ToString());
        }

        [TestMethod]
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

        private JArray LoadTestDataArray(string filename)
        {
            return JArray.Parse(LoadTestDataRaw(filename));
        }

        private JObject LoadTestDataObject(string filename)
        {
            return JObject.Parse(LoadTestDataRaw(filename));
        }

        private string LoadTestDataRaw(string filename)
        {
            string testDir = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string testDataDir = Path.Combine(Path.Combine(testDir, "Sample_Data"), "Bter");
            FileInfo fileName = new FileInfo(Path.Combine(testDataDir, filename));

            using (StreamReader reader = new StreamReader(new FileStream(fileName.FullName, FileMode.Open)))
            {
                return reader.ReadToEndAsync().Result;
            }
        }
    }
}
