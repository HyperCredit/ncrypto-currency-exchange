using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Model
{
    public class Book
    {
        private List<MarketDepth> sell;
        private List<MarketDepth> buy;

        public Book(List<MarketDepth> sell, List<MarketDepth> buy)
        {
            this.sell = sell;
            this.buy = buy;
        }

        public List<MarketDepth> Buy { get; set; }
        public List<MarketDepth> Sell { get; set; }
    }
}
