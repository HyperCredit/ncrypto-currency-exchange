using Lostics.NCryptoExchange.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange
{
    /// <summary>
    /// Interface for exchange trading
    /// </summary>
    public interface ITrading
    {
        Task CancelOrder(OrderId orderId);

        Task<OrderId> CreateOrder(MarketId marketId, OrderType orderType, decimal quantity, decimal price);
    }
}
