using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.VaultOfSatoshi
{
    public class VoSResponseException : VoSException
    {
        public VoSResponseException(string message)
            : base(message)
        {

        }
        public VoSResponseException(string message, Exception cause)
            : base(message, cause)
        {

        }
    }
}
