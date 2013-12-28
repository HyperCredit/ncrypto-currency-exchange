using System.Collections;
using System.Collections.Generic;

using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange
{
    public interface IExchange<M, O, W> where M: MarketId where O: OrderId where W: Wallet
    {
        public AccountInfo<W> getAccountInfo();

        public List<Market<M>> getMarkets();

        public List<Transaction> getMyTransactons();

        public List<MarketTrade> getMarketTrades(M marketId);

        public List<MyTrade> getMyTrades(M marketId, int? limit);

        public List<MyTrade> getAllMyTrades(int? limit);

        public List<MyOrder> getMyOrders(M marketId, int? limit);

        public List<MyOrder> getAllMyOrders(int? limit);

        public List<MarketDepth> getMarketDepth(M marketId);

        public void cancelOrder(O orderId);

        public void cancelAllOrders();

        public void cancelMarketOrders(M marketId);

        public O createOrder(M marketId,
                OrderType orderType, Quantity quantity,
                Quantity price);

        public int GetNextNonce();
    }
}
