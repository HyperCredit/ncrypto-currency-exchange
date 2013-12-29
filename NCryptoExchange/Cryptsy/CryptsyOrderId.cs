using Lostics.NCryptoExchange.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public sealed class CryptsyOrderId : OrderId
    {
        private readonly string value;

        public string Value
        {
            get { return this.value; }
        }

        public CryptsyOrderId(string setValue)
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

            return other.value.Equals(this.value);
        }

        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }

        public override string ToString()
        {
            return this.value;
        }
    }
}
