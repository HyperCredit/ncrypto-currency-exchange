using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.CoinEx
{
    public sealed class CoinExTradeId : AbstractIntBasedId, TradeId
    {
        public CoinExTradeId(int setValue) : base(setValue)
        {
        }
    }
}
