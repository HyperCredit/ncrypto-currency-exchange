using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.Kraken
{
    public class KrakenFailureException : ExchangeException
    {
        public KrakenFailureException(string message)
            : base(message)
        {

        }

    }
}
