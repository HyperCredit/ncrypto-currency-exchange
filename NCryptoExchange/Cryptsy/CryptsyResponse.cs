using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public abstract class CryptsyResponse<T> where T : class
    {
        private int success;
        private T returnValue;

        public int Success { get; set; }
        public T ReturnValue { get; set; }
    }
}
