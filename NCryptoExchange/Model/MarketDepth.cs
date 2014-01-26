using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Model
{
    public class MarketDepth : IComparable<MarketDepth>
    {
        public MarketDepth(decimal price, decimal quantity)
        {
            this.Price = price;
            this.Quantity = quantity;
        }

        public int CompareTo(MarketDepth other)
        {
            decimal priceDifference = this.Price - other.Price;

            if (0 == priceDifference)
            {
                decimal quantityDifference = this.Quantity - other.Quantity;

                return quantityDifference < 0
                    ? -1
                    : quantityDifference > 0
                        ? 1
                        : 0;
            }
            else
            {
                return priceDifference < 0
                    ? 1
                    : -1;
            }
        }

        /// <summary>
        /// Converts a dictionary of prices to quantity, into a list of market
        /// depth entries.
        /// </summary>
        /// <param name="pricesToQuantity">A dictionary mapping prices to
        /// quantity.</param>
        /// <returns>An ordered list of market depth data</returns>
        public static List<MarketDepth> DictionaryToList(Dictionary<decimal, decimal> pricesToQuantity)
        {
            List<MarketDepth> side = new List<MarketDepth>(pricesToQuantity.Count);

            foreach (decimal price in pricesToQuantity.Keys)
            {
                side.Add(new MarketDepth(price, pricesToQuantity[price]));
            }

            side.Sort();

            return side;
        }

        public decimal Quantity { get; private set; }
        public decimal Price { get; private set; }
    }
}
