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
        public static AccountInfo ParseAccountInfo(JObject jsonObj)
        {
            DateTime systemTime = ParseTime(jsonObj.Value<int>("systime"));
            List<Wallet> wallets = jsonObj.Value<JObject>("wallets").Properties().Select(
                 wallet => (Wallet)CoinsEWallet.Parse(wallet.Name, wallet.First.Value<JObject>())
             ).ToList();

            return new AccountInfo(wallets, systemTime);
        }

        public static Book ParseMarketOrders(JObject bookJson)
        {
            JArray bidsArray = bookJson.Value<JArray>("bids");
            JArray asksArray = bookJson.Value<JArray>("asks");

            List<MarketOrder> bids = bidsArray.Select(
                depth => (MarketOrder)CoinsEMarketOrder.ParseMarketDepth(depth as JObject, OrderType.Buy)
            ).ToList();
            List<MarketOrder> asks = asksArray.Select(
                depth => (MarketOrder)CoinsEMarketOrder.ParseMarketDepth(depth as JObject, OrderType.Sell)
            ).ToList();

            return new Book(asks, bids);
        }

        public static MyTrade<CoinsEMarketId, CoinsEOrderId> ParseMyTrade(JObject jObject, CoinsEMarketId marketId, TimeZoneInfo defaultTimeZone)
        {
            throw new NotImplementedException();
        }

        public static DateTime ParseTime(int secondsSinceEpoch)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(secondsSinceEpoch);
        }
    }
}
