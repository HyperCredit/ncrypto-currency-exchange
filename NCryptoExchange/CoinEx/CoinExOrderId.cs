using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.CoinEx
{
    public sealed class CoinExOrderId : AbstractIntBasedId, OrderId
    {
        public CoinExOrderId(int value) : base (value)
        {
        }
    }
}
