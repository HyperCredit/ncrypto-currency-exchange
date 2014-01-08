using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Model
{
    public abstract class Currency
    {
        public Currency(string currencyCode, string label)
        {
            this.CurrencyCode = currencyCode;
            this.Label = label;
        }

        public string CurrencyCode { get; private set; }
        public String Label { get; private set; }
    }
}
