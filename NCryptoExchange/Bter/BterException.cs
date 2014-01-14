using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.Bter
{
    public abstract class BterException : Exception
    {
        public BterException(string message)
            : base(message)
        {

        }
        public BterException(string message, Exception cause)
            : base(message, cause)
        {

        }
    }
}
