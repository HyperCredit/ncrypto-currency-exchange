using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json;
using System;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public class Fees
    {
        private Price fee;
        private Price net;

        public Fees(Price setFee, Price setNet)
        {
            this.fee = setFee;
            this.net = setNet;
        }

        public Fees(double setFee, double setNet)
        {
            this.fee = new Price(setFee);
            this.net = new Price(setNet);
        }

        public override string ToString()
        {
            return fee.ToString();
        }
    }
}
