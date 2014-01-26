using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Model
{
    public class Book
    {
        // Considered storing this data as a pair of dictionaries, but for now
        // am sticking how how the markets provide the raw data...
        public Book(List<MarketDepth> asks, List<MarketDepth> bids)
        {
            this.Asks = asks;
            this.Bids = bids;
        }

        public List<MarketDepth> Bids { get; private set; }
        public List<MarketDepth> Asks { get; private set; }
    }
}
