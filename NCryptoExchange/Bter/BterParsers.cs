using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Bter
{
    public static class BterParsers
    {
        public static DateTime ParseTime(int secondsSinceEpoch)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(secondsSinceEpoch);
        }

        internal static DateTime ParseDateTime(int secondsSinceEpoch)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            return dateTime.AddSeconds(secondsSinceEpoch);
        }

        public static Book ParseOrderBook(JObject bookJson)
        {
            JArray asksArray = bookJson.Value<JArray>("asks");
            JArray bidsArray = bookJson.Value<JArray>("bids");

            List<MarketDepth> asks = asksArray.Select(
                depth => (MarketDepth)ParseMarketDepth((JArray)depth, OrderType.Sell)
            ).ToList();
            List<MarketDepth> bids = bidsArray.Select(
                depth => (MarketDepth)ParseMarketDepth((JArray)depth, OrderType.Buy)
            ).ToList();

            return new Book(asks, bids);
        }

        internal static MarketOrder ParseMarketDepth(JArray depthArray, OrderType orderType)
        {
            return new MarketOrder(orderType,
                depthArray.Value<decimal>(0), depthArray.Value<decimal>(1));
        }

        public static List<Wallet> ParseWallets(JObject fundsJson)
        {
            JObject availableFundsJson = fundsJson.Value<JObject>("available_funds");
            JObject lockedFundsJson = fundsJson.Value<JObject>("locked_funds");
            List<Wallet> wallets = new List<Wallet>();

            foreach (JProperty fund in availableFundsJson.Properties())
            {
                decimal availableFunds = availableFundsJson.Value<decimal>(fund.Name);
                JToken lockedFundJson;
                decimal lockedFunds;

                if (null != lockedFundsJson
                    && lockedFundsJson.TryGetValue(fund.Name, out lockedFundJson))
                {
                    lockedFunds = decimal.Parse(lockedFundJson.ToString());
                }
                else
                {
                    lockedFunds = 0.0m;
                }

                wallets.Add(new Wallet(fund.Name, availableFunds + lockedFunds, lockedFunds));
            }

            return wallets;
        }
    }
}
