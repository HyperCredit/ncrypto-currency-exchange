using Lostics.NCryptoExchange.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.CoinsE
{
    /// <summary>
    /// An order number; this is a sub-component of an order ID, as Coins-E considers them.
    /// </summary>
    public class CoinsEOrderNumber : AbstractLongBasedId
    {
        public CoinsEOrderNumber(long setValue) : base(setValue)
        {
        }
    }
}
