using System;
namespace Lostics.NCryptoExchange.Model
{
    public class MarketTrade : Trade
    {
        public MarketTrade(TradeId tradeId, DateTime dateTime, decimal price,
            decimal quantity, MarketId marketId) : base(tradeId, dateTime, price, quantity, marketId)
        {
        }
    }
}
