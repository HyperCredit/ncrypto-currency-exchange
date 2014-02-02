using Lostics.NCryptoExchange.Cryptsy;
using Lostics.NCryptoExchange.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace Lostics.NCryptoExchange
{
    [TestClass]
    public class CryptsyTest
    {
        [TestMethod]
        public void TestParseCryptsyAccountInfo()
        {
            JObject jsonObj = LoadTestData("accountinfo.json");
            CryptsyAccountInfo accountInfo = CryptsyAccountInfo.Parse(jsonObj.Value<JObject>("return"));

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
        public void TestParseCryptsyMarkets()
        {
            JObject jsonObj = LoadTestData("getmarkets.json");
            TimeZoneInfo defaultTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            JArray marketsJson = jsonObj.Value<JArray>("return");
            List<CryptsyMarket> markets = marketsJson.Select(
                market => CryptsyMarket.Parse(market as JObject, defaultTimeZone)
            ).ToList();

            Assert.AreEqual(114, markets.Count);

            foreach (Market market in markets)
            {
                // DOGE/BTC
                if (market.MarketId.ToString().Equals("132"))
                {
                    Assert.AreEqual(market.BaseCurrencyCode, "DOGE");
                    Assert.AreEqual(market.QuoteCurrencyCode, "BTC");
                    Assert.AreEqual(market.Statistics.Volume24HBase, (decimal)716008746.70171800);
                }
            }
        }

        [TestMethod]
        public void TestParseCryptsyMarketOrders()
        {
            JObject jsonObj = LoadTestData("getmarketorders.json");
            Book marketOrders = CryptsyParsers.ParseMarketOrders(jsonObj.Value<JObject>("return"));

            MarketDepth lowestSellOrder = marketOrders.Asks[0];

            Assert.AreEqual((decimal)0.00001118, lowestSellOrder.Price);
            Assert.AreEqual((decimal)119.40714285, lowestSellOrder.Quantity);
        }

        [TestMethod]
        public void TestParseCryptsyMarketTrades()
        {
            JObject jsonObj = LoadTestData("getmarkettrades.json");
            CryptsyMarketId marketId = new CryptsyMarketId("1");
            JArray marketTradesJson = jsonObj.Value<JArray>("return");
            TimeZoneInfo defaultTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            List<CryptsyMarketTrade> marketTrades = marketTradesJson.Select(
                marketTrade => CryptsyMarketTrade.Parse(marketTrade as JObject, marketId, defaultTimeZone)
            ).ToList();

            CryptsyMarketTrade mostRecentTrade = marketTrades[0];

            Assert.AreEqual("10958207", mostRecentTrade.TradeId.ToString());
            Assert.AreEqual((decimal)16433.01498728, mostRecentTrade.Quantity);
            Assert.AreEqual(OrderType.Sell, mostRecentTrade.TradeType);
        }

        [TestMethod]
        public void TestParseCryptsyMyTrades()
        {
            JObject jsonObj = LoadTestData("getmytrades.json");
            CryptsyMarketId marketId = new CryptsyMarketId("132");
            TimeZoneInfo defaultTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            List<MyTrade> trades = jsonObj.Value<JArray>("return").Select(
                marketTrade => CryptsyParsers.ParseMyTrade(marketTrade as JObject, marketId, defaultTimeZone)
            ).ToList();

            Assert.AreEqual(2, trades.Count);
            Assert.AreEqual("9373209", trades[0].TradeId.ToString());
            Assert.AreEqual((decimal)0.00000059, trades[0].Price);
            Assert.AreEqual((decimal)1500.00000000, trades[0].Quantity);
            Assert.AreEqual((decimal)0.000000069, trades[0].Fee);

            Assert.AreEqual("9164163", trades[1].TradeId.ToString());
        }

        private JObject LoadTestData(string filename)
        {
            return TestUtils.LoadTestData<JObject>("Cryptsy", filename);
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

            using (CryptsyExchange cryptsy = new CryptsyExchange()
                {
                    PublicKey = "64d00dc4ee1c2b9551eabbdc831972d4ce2bcac5",
                    PrivateKey = "topsecret"
                }
            )
            {
                actual = cryptsy.GenerateSHA512Signature(request);
            }

            Assert.AreEqual(expected, actual);
        }
    }
}
