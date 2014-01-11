using Lostics.NCryptoExchange.Vircurex;
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
    public class VircurexTest
    {
        private Dictionary<string, string> GetCurrencyShortCodesToLabelMapping()
        {
            JObject currenciesJson = LoadTestDataObject("get_currency_info.json");
            List<VircurexCurrency> currencies = VircurexCurrency.Parse(currenciesJson);
            Dictionary<string, string> currencyShortCodeToLabel = new Dictionary<string, string>();

            foreach (VircurexCurrency currency in currencies)
            {
                currencyShortCodeToLabel.Add(currency.CurrencyCode, currency.Label);
            }
            return currencyShortCodeToLabel;
        }

        [TestMethod]
        public void TestBuildPublicUrl()
        {
            string url = VircurexExchange.BuildPublicUrl(VircurexExchange.Method.orderbook,
                VircurexExchange.Format.Json);

            Assert.AreEqual("https://vircurex.com/api/orderbook.json", url);
        }

        [TestMethod]
        public void TestParseCoins()
        {
            JObject jsonObj = LoadTestDataObject("get_currency_info.json");
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
            Dictionary<string, string> currencyShortCodeToLabel = GetCurrencyShortCodesToLabelMapping();

            JObject marketsJson = LoadTestDataObject("get_info_for_currency.json");
            List<Market> markets = new List<Market>();

            foreach (JProperty baseProperty in marketsJson.Properties())
            {
                string baseCurrency = baseProperty.Name;
                JObject properties = (JObject)baseProperty.Value;

                foreach (JProperty quoteProperty in properties.Properties())
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

        [TestMethod]
        public void TestParseMarketTrades()
        {
            JArray tradesJson = LoadTestDataArray("trades.json");
            VircurexMarketId marketId = new VircurexMarketId("DOGE", "BTC");
            List<MarketTrade> trades = VircurexParsers.ParseMarketTrades(marketId, tradesJson);

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
            Book orderBook = VircurexParsers.ParseMarketOrders(orderBookJson);
            List<MarketDepth> asks = orderBook.Asks;
            List<MarketDepth> bids = orderBook.Bids;

            Assert.AreEqual(asks[0].Price, (decimal)0.0000003);
            Assert.AreEqual(asks[0].Quantity, (decimal)3742.0);
        }

        [TestMethod]
        public void TestParseOrderBookAlt()
        {
            JObject orderBookJson = LoadTestDataObject("orderbook_alt.json");
            Dictionary<MarketId, Book> orderBooks = VircurexParsers.ParseMarketOrdersAlt("BTC",
                orderBookJson);
            Book orderBook = orderBooks[new VircurexMarketId("DOGE", "BTC")];
            List<MarketDepth> asks = orderBook.Asks;
            List<MarketDepth> bids = orderBook.Bids;

            Assert.AreEqual(asks[0].Price, (decimal)0.0000003);
            Assert.AreEqual(asks[0].Quantity, (decimal)120793.0143665);
        }

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
            string testDataDir = Path.Combine(Path.Combine(testDir, "Sample_Data"), "Vircurex");
            FileInfo fileName = new FileInfo(Path.Combine(testDataDir, filename));

            using (StreamReader reader = new StreamReader(new FileStream(fileName.FullName, FileMode.Open)))
            {
                return reader.ReadToEndAsync().Result;
            }
        }
    }
}
