using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.Vircurex
{
    public abstract class VircurexException : Exception
    {
        public VircurexException(string message)
            : base(message)
        {

        }
        public VircurexException(string message, Exception cause)
            : base(message, cause)
        {

        }
    }
}
