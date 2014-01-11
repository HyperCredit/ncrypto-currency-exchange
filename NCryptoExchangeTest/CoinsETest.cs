using Lostics.NCryptoExchange.CoinsE;
using Lostics.NCryptoExchange.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lostics.NCryptoExchange
{
    [TestClass]
    public class CoinsETest
    {
        [TestMethod]
        public void TestParseAccountInfo()
        {
            JObject jsonObj = LoadTestData("get_wallets.json");
            AccountInfo accountInfo = CoinsEParsers.ParseAccountInfo(jsonObj);

            Assert.AreEqual(5, accountInfo.Wallets.Count);

            foreach (Wallet wallet in accountInfo.Wallets)
            {
                if (wallet.CurrencyCode.Equals("BTC"))
                {
                    Assert.AreEqual((decimal)3.05354081, wallet.Balance);
                }
            }
        }

        [TestMethod]
        public void TestParseCancelOrder()
        {
            JObject jsonObj = LoadTestData("cancel_order.json");
            CoinsEMyOrder order = CoinsEMyOrder.Parse(jsonObj.Value<JObject>("order"));

            Assert.AreEqual("B/0.00212300/5517761665040384", order.OrderId.ToString());
            Assert.AreEqual(CoinsEOrderStatus.cancel_requested, order.Status);
        }

        [TestMethod]
        public void TestParseCoins()
        {
            JObject jsonObj = LoadTestData("list_coins.json");
            List<CoinsECurrency> coins = jsonObj.Value<JArray>("coins").Select(
                coin => CoinsECurrency.Parse(coin as JObject)
            ).ToList();
            
            Assert.AreEqual(2, coins.Count);
            Assert.AreEqual("BTC", coins[0].CurrencyCode);
            Assert.AreEqual("bitcoin", coins[0].Label);
            Assert.AreEqual("DGC", coins[1].CurrencyCode);
            Assert.AreEqual("digitalcoin", coins[1].Label);
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
            List<CoinsEMarket> markets = marketsJson.Properties().Select(
                market => CoinsEMarket.Parse(currencyCodesToLabel, "RED_BTC",
                    market.Value as JObject)
            ).ToList();

            Assert.AreEqual(1, markets.Count);

            Assert.AreEqual(markets[0].Label, "RED_BTC");
            Assert.AreEqual(markets[0].BaseCurrencyCode, "RED");
            Assert.AreEqual(markets[0].BaseCurrencyName, "redcoin");
            Assert.AreEqual(markets[0].Statistics.Volume24HBase, (decimal)6396.70000000);
            Assert.AreEqual(markets[0].QuoteCurrencyCode, "BTC");
        }

        [TestMethod]
        public void TestParseListOrders()
        {
            JObject jsonObj = LoadTestData("list_orders.json");
            JArray ordersJson = jsonObj.Value<JArray>("orders");
            List<CoinsEMyOrder> orders = ordersJson.Select(
                market => CoinsEMyOrder.Parse(market as JObject)
            ).ToList();

            Assert.AreEqual(2, orders.Count);
            
            Assert.AreEqual(OrderType.Buy, orders[0].OrderType);
            Assert.AreEqual((decimal)1.00000000, orders[0].OriginalQuantity);
            Assert.AreEqual((decimal)0.00000000, orders[0].Quantity);
        }

        [TestMethod]
        public void TestParseNewOrder()
        {
            JObject jsonObj = LoadTestData("new_order.json");
            CoinsEMyOrder order = CoinsEMyOrder.Parse(jsonObj.Value<JObject>("order"));
            
            Assert.AreEqual("B/0.00212300/5517761665040384", order.OrderId.ToString());
            Assert.AreEqual((decimal)1.00000000, order.Quantity);
            Assert.AreEqual((decimal)0.00212300, order.Price);
            Assert.AreEqual(true, order.IsOpen);
            Assert.AreEqual(CoinsEOrderStatus.queued, order.Status);
        }

        [TestMethod]
        public void TestParseMarketDepth()
        {
            JObject jsonObj = LoadTestData("depth.json");
            Book marketOrders = CoinsEParsers.ParseMarketDepth(jsonObj.Value<JObject>("marketdepth"));

            Assert.AreEqual(3, marketOrders.Bids.Count);
            Assert.AreEqual(1, marketOrders.Asks.Count);

            CoinsEMarketDepth lowestSellOrder = (CoinsEMarketDepth)marketOrders.Asks[0];

            Assert.AreEqual((decimal)2.92858000, lowestSellOrder.Price);
            Assert.AreEqual((decimal)8.98400000, lowestSellOrder.Quantity);
            Assert.AreEqual((decimal)8.98400000, lowestSellOrder.CummulativeQuantity);

            CoinsEMarketDepth highestBuyOrder = (CoinsEMarketDepth)marketOrders.Bids[2];

            Assert.AreEqual((decimal)0.12230000, highestBuyOrder.Price);
            Assert.AreEqual((decimal)11.00000000, highestBuyOrder.Quantity);
            Assert.AreEqual((decimal)16.40500000, highestBuyOrder.CummulativeQuantity);
        }

        [TestMethod]
        public void TestParseRecentTrades()
        {
            JObject jsonObj = LoadTestData("recent_trades.json");
            List<CoinsEMarketTrade> trades = jsonObj.Value<JArray>("trades").Select(
                marketTrade => CoinsEMarketTrade.Parse(marketTrade as JObject)
            ).ToList();

            Assert.AreEqual(2, trades.Count);
            Assert.AreEqual("0.76124500/5909462682435584-1.52249000/6402043891679232", trades[0].TradeId.ToString());
            Assert.AreEqual("WDC_BTC", trades[0].MarketId.ToString());
            Assert.AreEqual((decimal)2.45600000, trades[0].Quantity);
            Assert.AreEqual((decimal)1.52249000, trades[0].Price);
            Assert.AreEqual("2.19643500/5205775240658944-2.28373500/4994669008125952", trades[1].TradeId.ToString());
        }

        private JObject LoadTestData(string filename)
        {
            string testDir = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string testDataDir = Path.Combine(Path.Combine(testDir, "Sample_Data"), "Coins-E");
            FileInfo fileName = new FileInfo(Path.Combine(testDataDir, filename));
            JObject jsonObj;

            using (StreamReader reader = new StreamReader(new FileStream(fileName.FullName, FileMode.Open)))
            {
                jsonObj = JObject.Parse(reader.ReadToEndAsync().Result);
            }

            return jsonObj;
        }
    }
}
