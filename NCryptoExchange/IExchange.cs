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
    public interface IExchange : IDisposable
    {
        Task<AccountInfo> GetAccountInfo();

        Task<List<Market>> GetMarkets();

        Task<Book> GetMarketDepth(MarketId marketId);

        Task<List<MyOrder>> GetMyActiveOrders(MarketId marketId, int? limit);

        string GetNextNonce();

        string Label { get; }
    }
}
