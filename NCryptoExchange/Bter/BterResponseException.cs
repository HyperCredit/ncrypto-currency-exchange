using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.Bter
{
    public class BterResponseException : BterException
    {
        public BterResponseException(string message)
            : base(message)
        {

        }
        public BterResponseException(string message, Exception cause)
            : base(message, cause)
        {

        }
    }
}
