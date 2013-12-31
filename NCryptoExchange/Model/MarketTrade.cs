using System;
namespace Lostics.NCryptoExchange.Model
{
    public class MarketTrade<M, T> : Trade<M, T>
        where M : MarketId
        where T : TradeId
    {
        public MarketTrade(T tradeId, OrderType tradeType,
            DateTime dateTime, decimal price,
            decimal quantity, decimal fee,
            M marketId) : base(tradeId, tradeType, dateTime, price, quantity, fee, marketId)
        {
        }
    }
}
