using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json;
using System;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public class Fees
    {
        public Fees(decimal setFee, decimal setNet)
        {
            this.Fee = setFee;
            this.Net = setNet;
        }

        public override string ToString()
        {
            return this.Fee.ToString();
        }

        public decimal Fee { get; private set; }

        public decimal Net { get; private set; }
    }
}
