using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.CoinsE
{
    public enum CoinsEOrderStatus
    {
        queued,
        accepted,
        executed,
        partially_executed,
        cancel_requested,
        cancel_initiated,
        cancelled
    }
}
