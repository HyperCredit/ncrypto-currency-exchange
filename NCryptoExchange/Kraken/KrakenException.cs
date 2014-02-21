using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.Kraken
{
    public abstract class KrakenException : Exception
    {
        public KrakenException(string message)
            : base(message)
        {

        }
        public KrakenException(string message, Exception cause)
            : base(message, cause)
        {

        }
    }
}
