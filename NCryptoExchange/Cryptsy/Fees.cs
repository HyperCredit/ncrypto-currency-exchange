using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json;
using System;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public class Fees
    {
        private decimal fee;
        private decimal net;

        public Fees(decimal setFee, decimal setNet)
        {
            this.fee = setFee;
            this.net = setNet;
        }

        public override string ToString()
        {
            return fee.ToString();
        }
    }
}
