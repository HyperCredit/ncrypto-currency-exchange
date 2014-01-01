using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public class CryptsyAccountInfo : AccountInfo
    {
        public CryptsyAccountInfo(List<Wallet> setWallets, DateTime setSystemTime,
            TimeZoneInfo serverTimeZone, int openOrderCount)
            : base(setWallets, setSystemTime)
        {
            this.ServerTimeZone = serverTimeZone;
            this.OpenOrderCount = openOrderCount;
        }

        public static CryptsyAccountInfo Parse(JObject accountInfoJson)
        {
            TimeZoneInfo serverTimeZone = TimeZoneResolver.GetByShortCode(accountInfoJson.Value<string>("servertimezone"));
            DateTime serverDateTime = DateTime.Parse(accountInfoJson.Value<string>("serverdatetime"));

            serverDateTime = TimeZoneInfo.ConvertTimeToUtc(serverDateTime, serverTimeZone);

            return new CryptsyAccountInfo(
                ParseWallets(accountInfoJson.Value<JObject>("balances_available"), accountInfoJson.Value<JObject>("balances_hold")),
                serverDateTime, serverTimeZone,
                accountInfoJson.Value<int>("openordercount")
            );
        }

        internal static List<Wallet> ParseWallets(JObject balancesAvailable, JObject balancesHold)
        {
            List<Wallet> wallets = new List<Wallet>();
            foreach (JProperty balanceAvailable in balancesAvailable.Properties())
            {
                wallets.Add(new Wallet(balanceAvailable.Name,
                    balancesAvailable.Value<decimal>(balanceAvailable.Name),
                    balancesHold.Value<decimal>(balanceAvailable.Name)));
            }
            return wallets;
        }

        public override string ToString()
        {
            return this.SystemTime.ToString() + ": "
                + this.OpenOrderCount + " open orders.";
        }

        public int OpenOrderCount { get; private set; }
        public TimeZoneInfo ServerTimeZone { get; private set; }
    }
}
