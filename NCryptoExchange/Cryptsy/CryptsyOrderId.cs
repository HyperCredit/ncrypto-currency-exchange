using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public sealed class CryptsyOrderId : AbstractStringBasedId, OrderId
    {
        public CryptsyOrderId(string setValue) : base(setValue)
        {
        }

        internal static CryptsyOrderId Parse(Newtonsoft.Json.Linq.JToken orderIdToken)
        {
            if (orderIdToken.Type != Newtonsoft.Json.Linq.JTokenType.String)
            {
                throw new CryptsyResponseException("Expected order ID as a string but encountered token type \""
                    + orderIdToken.Type + "\".");
            }

            return new CryptsyOrderId(orderIdToken.ToString());
        }
    }
}
