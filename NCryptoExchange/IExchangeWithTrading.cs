using Lostics.NCryptoExchange.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange
{
    /// <summary>
    /// Interface for exchange trading; this is a separate interface so that
    /// exchanges where trading is not yet supported can be implemented sensibly.
    /// </summary>
    public interface IExchangeWithTrading : IExchange
    {
        Task CancelOrder(OrderId orderId);

        Task<OrderId> CreateOrder(MarketId marketId, OrderType orderType, decimal quantity, decimal price);
    }
}
