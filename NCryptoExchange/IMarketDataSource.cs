using Lostics.NCryptoExchange.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange
{
    public interface IMarketDataSource
    {
        Task<List<Market>> GetMarkets();

        Task<Book> GetMarketOrders(MarketId marketId);

        Task<List<MarketTrade>> GetMarketTrades(MarketId marketId);

        Task<Book> GetMarketDepth(MarketId marketId);
    }
}
