using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Model
{
    public class Book
    {
        public Book(List<MarketOrder> sell, List<MarketOrder> buy)
        {
            this.Sell = sell;
            this.Buy = buy;
        }

        public List<MarketOrder> Buy { get; private set; }
        public List<MarketOrder> Sell { get; private set; }
    }
}
