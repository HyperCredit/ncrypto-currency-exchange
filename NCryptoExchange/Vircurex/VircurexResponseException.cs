using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.Vircurex
{
    public class VircurexResponseException : VircurexException
    {
        public VircurexResponseException(string message)
            : base(message)
        {

        }
        public VircurexResponseException(string message, Exception cause)
            : base(message, cause)
        {

        }
    }
}
