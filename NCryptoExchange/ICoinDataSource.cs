using Lostics.NCryptoExchange.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange
{
    public interface ICoinDataSource<C>
        where C : Currency
    {
        Task<List<C>> GetCoins();
    }
}
