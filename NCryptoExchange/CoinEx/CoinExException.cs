using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.CoinEx
{
    public abstract class CoinExException : Exception
    {
        public CoinExException(string message)
            : base(message)
        {

        }
        public CoinExException(string message, Exception cause)
            : base(message, cause)
        {

        }
    }
}
