﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Model
{
    public class Book
    {
        public Book(List<MarketDepth> sell, List<MarketDepth> buy)
        {
            this.Sell = sell;
            this.Buy = buy;
        }

        public List<MarketDepth> Buy { get; private set; }
        public List<MarketDepth> Sell { get; private set; }
    }
}
