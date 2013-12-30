using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public enum CryptsyMethod
    {
        allmyorders,
        allmytrades,
        cancelorder,
        cancelallorders,
        cancelmarketorder,
        calculatefees,
        createorder,
        generatenewaddress,
        getinfo,
        getmarkets,
        marketorders,
        markettrades,
        myorders,
        mytrades,
        mytransactions
    }
}
