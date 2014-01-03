using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.CoinsE
{
    public static class CoinsEParsers
    {
        public static Book ParseMarketOrders(JObject jObject)
        {
            throw new NotImplementedException();
        }

        public static MyTrade<CoinsEMarketId, CoinsEOrderId> ParseMyTrade(JObject jObject, CoinsEMarketId marketId, TimeZoneInfo defaultTimeZone)
        {
            throw new NotImplementedException();
        }

        public static AccountInfo ParseAccountInfo(JObject jsonObj)
        {
            throw new NotImplementedException();
        }
    }
}
