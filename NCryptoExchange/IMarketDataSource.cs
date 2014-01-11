using Lostics.NCryptoExchange.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange
{
    /// <summary>
    /// Interface for API to fetch data about a markets an exchange offers,.
    /// </summary>
    public interface IMarketDataSource
    {
        Task<List<Market>> GetMarkets();

        Task<List<MarketTrade>> GetMarketTrades(MarketId marketId);

        Task<Book> GetMarketDepth(MarketId marketId);
    }
}
