using System.Collections;
using System.Collections.Generic;

using Lostics.NCryptoExchange.Model;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange
{
    public interface IExchange<M, O, W> where M: MarketId where O: OrderId where W: Wallet
    {
        Task<AccountInfo<W>> GetAccountInfo();

        Task<List<Market<M>>> GetMarkets();

        Task<List<Transaction>> GetMyTransactons();

        Task<List<MarketTrade>> GetMarketTrades(M marketId);

        Task<List<MyTrade>> GetMyTrades(M marketId, int? limit);

        Task<List<MyTrade>> GetAllMyTrades(int? limit);

        Task<List<MyOrder>> GetMyOrders(M marketId, int? limit);

        Task<List<MyOrder>> GetAllMyOrders(int? limit);

        Task<List<MarketDepth>> GetMarketDepth(M marketId);

        Task CancelOrder(O orderId);

        Task CancelAllOrders();

        Task CancelMarketOrders(M marketId);

        Task<O> CreateOrder(M marketId,
                OrderType orderType, Quantity quantity,
                Quantity price);

        string GetNextNonce();
    }
}
