using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lostics.NCryptoExchange.Cryptsy;
using System.Threading.Tasks;
using System.Net.Http;
using Lostics.NCryptoExchange.Model;
using System.Collections.Generic;

namespace Lostics.NCryptoExchangeTest.Cryptsy
{
    [TestClass]
    public class CryptsyTest
    {
        [TestMethod]
        public void SignRequestTest()
        {
            string actual;
            string expected = "6dd05bfe3104a70768cf76a30474176db356818d3556e536c31d158fc2c3adafa096df144b46b2ccb1ff6128d6a0a07746695eca061547b25fd676c9614e6718";
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>(CryptsyExchange.PARAM_METHOD, Enum.GetName(typeof(CryptsyMethod), CryptsyMethod.getinfo)),
                    new KeyValuePair<string, string>(CryptsyExchange.PARAM_NONCE, "1388246959")
                });

            using (CryptsyExchange cryptsy = new CryptsyExchange("64d00dc4ee1c2b9551eabbdc831972d4ce2bcac5",
                "topsecret"))
            {
                actual = CryptsyExchange.GenerateSHA512Signature(request, cryptsy.PrivateKey).Result;
            }

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CalculateFeesTest()
        {
            Task<Fees> response;

            using (CryptsyExchange cryptsy = GetExchange())
            {
                response = cryptsy.CalculateFees(OrderType.Buy, new Price(0.05), new Price(1.0));
                response.Wait();
            }

            System.Console.Write(response.Result);
        }

        [TestMethod]
        public void GetMarketOrdersTest()
        {
            CryptsyMarketId marketId = new CryptsyMarketId("132");
            Task<List<MarketOrder>> response;

            using (CryptsyExchange cryptsy = GetExchange())
            {
                response = cryptsy.GetMarketOrders(marketId);
                response.Wait();
            }

            foreach (MarketOrder order in response.Result)
            {
                Console.WriteLine(order.ToString());
            }
        }

        [TestMethod]
        public void GetMyOrdersTest()
        {
            CryptsyMarketId marketId = new CryptsyMarketId("132");
            Task<List<MyOrder<CryptsyMarketId, CryptsyOrderId>>> response;

            using (CryptsyExchange cryptsy = GetExchange())
            {
                response = cryptsy.GetMyOrders(marketId, null);
                response.Wait();
            }

            foreach (MyOrder<CryptsyMarketId, CryptsyOrderId> order in response.Result)
            {
                Console.WriteLine(order.ToString());
            }
        }

        [TestMethod]
        public void GetMyTradesTest()
        {
            CryptsyMarketId marketId = new CryptsyMarketId("132");
            Task<List<MyTrade<CryptsyMarketId, CryptsyOrderId, CryptsyTradeId>>> response;

            using (CryptsyExchange cryptsy = GetExchange())
            {
                response = cryptsy.GetMyTrades(marketId, null);
                response.Wait();
            }

            foreach (MyTrade<CryptsyMarketId, CryptsyOrderId, CryptsyTradeId> trade in response.Result)
            {
                Console.WriteLine(trade.ToString());
            }
        }

        [TestMethod]
        public void GetTransactionsTest()
        {
            Task<List<Transaction>> response;

            using (CryptsyExchange cryptsy = GetExchange())
            {
                response = cryptsy.GetMyTransactions();
                response.Wait();
            }


            foreach (Transaction transaction in response.Result)
            {
                Console.WriteLine(transaction.ToString());
            }
        }

        private static CryptsyExchange GetExchange()
        {
            return new CryptsyExchange("64d00dc4ee1c2b9551eabbdc831972d4ce2bcac5",
                "topsecret");
        }
    }
}
