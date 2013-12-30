using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string msg)
            : base(msg)
        {

        }
    }
}
