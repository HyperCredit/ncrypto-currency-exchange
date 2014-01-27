using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.VaultOfSatoshi
{
    public abstract class VoSException : Exception
    {
        public VoSException(string message)
            : base(message)
        {

        }
        public VoSException(string message, Exception cause)
            : base(message, cause)
        {

        }
    }
}
