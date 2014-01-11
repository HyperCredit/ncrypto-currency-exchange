using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange
{
    /// <summary>
    /// Generic interface for an exchange
    /// </summary>
    public interface IExchange : IDisposable, IMarketDataSource
    {
        Task CancelOrder(OrderId orderId);

        Task CancelMarketOrders(MarketId marketId);

        Task<OrderId> CreateOrder(MarketId marketId, OrderType orderType, decimal quantity, decimal price);

        void Dispose();

        Task<AccountInfo> GetAccountInfo();

        Task<List<MyTrade>> GetMyTrades(MarketId marketId, int? limit);

        Task<List<MyTrade>> GetAllMyTrades(int? limit);

        Task<List<Market>> GetMarkets();

        Task<Book> GetMarketOrders(MarketId marketId);

        Task<List<MarketTrade>> GetMarketTrades(MarketId marketId);

        Task<Book> GetMarketDepth(MarketId marketId);

        Task<List<MyOrder>> GetMyActiveOrders(MarketId marketId, int? limit);

        string GetNextNonce();

        string Label { get; }
    }
}
