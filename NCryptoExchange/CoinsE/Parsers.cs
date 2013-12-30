using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.CoinsE
{
    public static class Parsers
    {
        internal static Task<List<Model.Market<CoinsEMarketId>>> ParseMarkets(Newtonsoft.Json.Linq.JObject marketsJson)
        {
            throw new NotImplementedException();
        }
    }
}
