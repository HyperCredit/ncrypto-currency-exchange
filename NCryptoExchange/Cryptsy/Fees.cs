using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json;
using System;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public class Fees
    {
        private Quantity fee;
        private Quantity net;

        public Fees(Quantity setFee, Quantity setNet)
        {
            this.fee = setFee;
            this.net = setNet;
        }

        public Fees(double setFee, double setNet)
        {
            this.fee = new Quantity(setFee);
            this.net = new Quantity(setNet);
        }

        public override string ToString()
        {
            return fee.ToString();
        }
    }
}
