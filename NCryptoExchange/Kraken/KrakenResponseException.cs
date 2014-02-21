using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.Kraken
{
    public class KrakenResponseException : KrakenException
    {
        public KrakenResponseException(string message)
            : base(message)
        {

        }
        public KrakenResponseException(string message, Exception cause)
            : base(message, cause)
        {

        }
    }
}
