using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public class CryptsyFailureException : ExchangeException
    {
        public CryptsyFailureException(string message)
            : base(message)
        {

        }

    }
}
