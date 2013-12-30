using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.CoinsE
{
    public abstract class CoinsEException : Exception
    {
        public CoinsEException(string message)
            : base(message)
        {

        }
        public CoinsEException(string message, Exception cause)
            : base(message, cause)
        {

        }
    }
}
