using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public class CryptsyResponseException : ExchangeException
    {
        public CryptsyResponseException(string message)
            : base(message)
        {

        }
        public CryptsyResponseException(string message, Exception cause)
            : base(message, cause)
        {

        }
    }
}
