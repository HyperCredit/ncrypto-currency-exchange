using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchangeTest
{
    [TestClass]
    public class PriceTest
    {
        [TestMethod]
        public void TestValueMatchesConstructor()
        {
            Price price = new Price(1000, 0.1); // 100.0
            double expected = 100.0;
            double actual = price.Value;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAddition()
        {
            Price priceA = new Price(1000, 0.1); // 100.0
            Price priceB = new Price(2000, 0.1); // 200.0
            double expected = 300.0;
            double actual = (priceA + priceB).Value;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAdditionDifferentPrecision()
        {
            Price priceA = new Price(10000, 0.01); // 100.0
            Price priceB = new Price(2000, 0.1); // 200.0
            double expected = 300.0;
            double actual = (priceA + priceB).Value;

            Assert.AreEqual(expected, actual, 0.1);
        }

        [TestMethod]
        public void TestSubtraction()
        {
            Price priceA = new Price(2000, 0.1); // 200.0
            Price priceB = new Price(1000, 0.1); // 100.0
            double expected = 100.0;
            double actual = (priceA - priceB).Value;

            Assert.AreEqual(expected, actual, 0.1);
        }

        [TestMethod]
        public void TestMultiplication()
        {
            Price priceA = new Price(20, 0.1); // 2.0
            Price priceB = new Price(30, 0.1); // 3.0
            double expected = 6;
            double actual = (priceA * priceB).Value;

            Assert.AreEqual(expected, actual, 0.1);
        }

        [TestMethod]
        public void TestDivision()
        {
            Price priceA = new Price(6000, 0.1); // 600.0
            Price priceB = new Price(2000, 0.1); // 200.0
            double expected = 3.0;
            double actual = (priceA / priceB).Value;

            Assert.AreEqual(expected, actual, 0.1);
        }
    }
}
