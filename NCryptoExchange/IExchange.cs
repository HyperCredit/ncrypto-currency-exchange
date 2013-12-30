using System.Collections.Generic;
using System.Threading.Tasks;
using System;

using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange
{
    public interface IExchange<M, O, T, W> : IDisposable
        where M : MarketId
        where O : OrderId
        where T : TradeId
        where W : Wallet
    {
        Task<AccountInfo<W>> GetAccountInfo();

        Task<List<Market<M>>> GetMarkets();

        Task<List<Transaction>> GetMyTransactions();

        Task<List<MarketTrade<M, T>>> GetMarketTrades(M marketId);

        Task<List<MyTrade<M, O, T>>> GetMyTrades(M marketId, int? limit);

        Task<List<MyTrade<M, O, T>>> GetAllMyTrades(int? limit);

        Task<List<MyOrder<M, O>>> GetMyOrders(M marketId, int? limit);

        Task<List<MyOrder<M, O>>> GetAllMyOrders(int? limit);

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
