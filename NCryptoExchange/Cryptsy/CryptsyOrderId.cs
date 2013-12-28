using Lostics.NCryptoExchange.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public class CryptsyOrderId : OrderId
    {
        private readonly int value;

        public int Value
        {
            get { return this.value; }
        }

        public CryptsyOrderId(int setValue)
        {
            this.value = setValue;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CryptsyOrderId))
            {
                return false;
            }

            CryptsyOrderId other = (CryptsyOrderId)obj;

            return other.value == this.value;
        }
    }
}
