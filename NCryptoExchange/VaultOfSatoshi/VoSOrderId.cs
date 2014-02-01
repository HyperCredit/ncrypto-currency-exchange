using Lostics.NCryptoExchange.Model;
using System.Collections.Generic;

namespace Lostics.NCryptoExchange.VaultOfSatoshi
{
    public sealed class VoSOrderId : AbstractIntBasedId, OrderId
    {
        public VoSOrderId(int value) : base (value)
        {
        }

        public KeyValuePair<string, string> KeyValuePair
        {
            get
            {
                return new KeyValuePair<string, string>("order_id", this.Value.ToString());
            }
        }
    }
}
