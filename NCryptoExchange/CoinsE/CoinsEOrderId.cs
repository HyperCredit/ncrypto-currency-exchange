using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.CoinsE
{
    public sealed class CoinsEOrderId : AbstractStringBasedId, OrderId
    {
        public CoinsEOrderId(string setValue) : base(setValue)
        {
        }

        internal static CoinsEOrderId Parse(Newtonsoft.Json.Linq.JToken orderIdToken)
        {
            if (orderIdToken.Type != Newtonsoft.Json.Linq.JTokenType.String)
            {
                throw new CoinsEResponseException("Expected order ID as a string but encountered token type \""
                    + orderIdToken.Type + "\".");
            }

            return new CoinsEOrderId(orderIdToken.ToString());
        }
    }
}
