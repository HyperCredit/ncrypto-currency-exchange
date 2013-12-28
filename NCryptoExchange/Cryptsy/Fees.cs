using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public class Fees
    {
        private readonly Quantity fee;
        private readonly Quantity net;

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

        public static Fees ParseJson(string json)
        {
            RawFees rawFees = JsonConvert.DeserializeObject<RawFees>(json);

            return new Fees(rawFees.fee, rawFees.net);
        }
    }

    protected class RawFees
    {
        public double fee;
        public double net;
    }
}
