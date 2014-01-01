using Lostics.NCryptoExchange.Cryptsy;
using Lostics.NCryptoExchange.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace Lostics.NCryptoExchangeTest.Cryptsy
{
    [TestClass]
    public class CryptsyTest
    {
        [TestMethod]
        public void TestParseAccountInfo()
        {
            JObject jsonObj = LoadTestData("accountinfo.json");
            CryptsyAccountInfo accountInfo = CryptsyParsers.ParseAccountInfo(jsonObj.Value<JObject>("return"));

            Assert.AreEqual(93, accountInfo.Wallets.Count);
            Assert.AreEqual(TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), accountInfo.ServerTimeZone);

            foreach (Wallet wallet in accountInfo.Wallets)
            {
                if (wallet.CurrencyCode.Equals("BTC"))
                {
                    Assert.AreEqual((decimal)0.00001458, wallet.Balance);
                }
            }
        }

        [TestMethod]
        public void TestParseMarkets()
        {
            JObject jsonObj = LoadTestData("getmarkets.json");
            List<Market<CryptsyMarketId>> markets = CryptsyParsers.ParseMarkets(jsonObj.Value<JArray>("return"),
                TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

            Assert.AreEqual(114, markets.Count);

            foreach (Market<CryptsyMarketId> market in markets)
            {
                // DOGE/BTC
                if (market.MarketId.ToString().Equals("132"))
                {
                    Assert.AreEqual(market.BaseCurrencyCode, "DOGE");
                    Assert.AreEqual(market.QuoteCurrencyCode, "BTC");
                }
            }
        }

        [TestMethod]
        public void TestParseMarketOrders()
        {
            JObject jsonObj = LoadTestData("getmarketorders.json");
            MarketOrders<CryptsyMarketOrder> marketOrders = CryptsyParsers.ParseMarketOrders(jsonObj.Value<JObject>("return"));

            MarketOrder lowestSellOrder = marketOrders.Sell[0];

            Assert.AreEqual((decimal)0.00001118, lowestSellOrder.Price);
            Assert.AreEqual((decimal)119.40714285, lowestSellOrder.Quantity);
        }

        [TestMethod]
        public void TestParseMarketTrades()
        {
            JObject jsonObj = LoadTestData("getmarkettrades.json");
            CryptsyMarketId marketId = new CryptsyMarketId("1");
            JArray marketTradesJson = jsonObj.Value<JArray>("return");
            TimeZoneInfo defaultTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            List<CryptsyMarketTrade> marketTrades = marketTradesJson.Select(
                marketTrade => CryptsyMarketTrade.Parse(marketTrade as JObject, marketId, defaultTimeZone)
            ).ToList();

            MarketTrade<CryptsyMarketId> mostRecentTrade = marketTrades[0];

            Assert.AreEqual("10958207", mostRecentTrade.TradeId.ToString());
            Assert.AreEqual((decimal)16433.01498728, mostRecentTrade.Quantity);
            Assert.AreEqual(OrderType.Sell, mostRecentTrade.TradeType);
        }

        [TestMethod]
        public void TestParseMyTrades()
        {
            JObject jsonObj = LoadTestData("getmytrades.json");
            List<MyTrade<CryptsyMarketId, CryptsyOrderId>> trades = CryptsyParsers.ParseMyTrades(jsonObj.Value<JArray>("return"),
                new CryptsyMarketId("132"),
                TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

            Assert.AreEqual(2, trades.Count);
            Assert.AreEqual("9373209", trades[0].TradeId.ToString());
            Assert.AreEqual("9164163", trades[1].TradeId.ToString());
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
