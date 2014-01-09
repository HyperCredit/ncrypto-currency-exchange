using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Model
{
    public class Book
    {
        public Book(List<MarketOrder> asks, List<MarketOrder> bids)
        {
            this.Asks = asks;
            this.Bids = bids;
        }

        public List<MarketOrder> Bids { get; private set; }
        public List<MarketOrder> Asks { get; private set; }
    }
}
