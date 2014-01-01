using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Model
{
    public class MarketOrders<O>
        where O : MarketOrder
    {
        public  MarketOrders(List<O> sell, List<O> buy)
        {
            this.Sell = sell;
            this.Buy = buy;
        }

        public List<O> Buy { get; private set; }
        public List<O> Sell { get; private set; }
    }
}
