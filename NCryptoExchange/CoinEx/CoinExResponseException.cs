using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.CoinEx
{
    public class CoinExResponseException : CoinExException
    {
        public CoinExResponseException(string message)
            : base(message)
        {

        }
        public CoinExResponseException(string message, Exception cause)
            : base(message, cause)
        {

        }
    }
}
