using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.Prelude
{
    public abstract class PreludeException : Exception
    {
        public PreludeException(string message)
            : base(message)
        {

        }
        public PreludeException(string message, Exception cause)
            : base(message, cause)
        {

        }
    }
}
