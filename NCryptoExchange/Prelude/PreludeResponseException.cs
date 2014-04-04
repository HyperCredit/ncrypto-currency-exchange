using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.Prelude
{
    public class PreludeResponseException : PreludeException
    {
        public PreludeResponseException(string message)
            : base(message)
        {

        }
        public PreludeResponseException(string message, Exception cause)
            : base(message, cause)
        {

        }
    }
}
