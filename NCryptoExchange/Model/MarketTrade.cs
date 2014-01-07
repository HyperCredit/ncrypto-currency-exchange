using System;
namespace Lostics.NCryptoExchange.Model
{
    public class MarketTrade<M> : Trade<M>
        where M : MarketId
    {
        public MarketTrade(TradeId tradeId, DateTime dateTime, decimal price,
            decimal quantity, M marketId) : base(tradeId, dateTime, price, quantity, marketId)
        {
        }
    }
}
