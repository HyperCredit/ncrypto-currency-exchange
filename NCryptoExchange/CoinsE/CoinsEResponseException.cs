using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.CoinsE
{
    public class CoinsEResponseException : CoinsEException
    {
        public CoinsEResponseException(string message)
            : base(message)
        {

        }
        public CoinsEResponseException(string message, Exception cause)
            : base(message, cause)
        {

        }
    }
}
