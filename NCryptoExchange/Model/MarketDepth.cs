using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Model
{
    public class MarketDepth
    {
        public MarketDepth(decimal price, decimal quantity)
        {
            this.Price = price;
            this.Quantity = quantity;
        }

        public decimal Quantity { get; private set; }
        public decimal Price { get; private set; }
    }
}
