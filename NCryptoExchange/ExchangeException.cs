using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange
{
    public class ExchangeException : Exception
    {
        public ExchangeException(string message)
            : base(message)
        {

        }
    }
}
