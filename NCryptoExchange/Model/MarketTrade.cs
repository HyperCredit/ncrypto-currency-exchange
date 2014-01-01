using System;
namespace Lostics.NCryptoExchange.Model
{
    public class MarketTrade<M> : Trade<M>
        where M : MarketId
    {
        public MarketTrade(TradeId tradeId, OrderType tradeType,
            DateTime dateTime, decimal price,
            decimal quantity, decimal fee,
            M marketId) : base(tradeId, tradeType, dateTime, price, quantity, fee, marketId)
        {
        }
    }
}
