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
    public class CryptsyAccountInfo : AccountInfo<Wallet>
    {
        private TimeZoneInfo serverTimeZone;
        private int openOrderCount;

        public CryptsyAccountInfo(List<Wallet> setWallets, DateTime setSystemTime,
            TimeZoneInfo serverTimeZone, int openOrderCount)
            : base(setWallets, setSystemTime)
        {
            this.serverTimeZone = serverTimeZone;
            this.openOrderCount = openOrderCount;
        }

        public override string ToString()
        {
            return this.SystemTime.ToString() + ": "
                + this.OpenOrderCount + " open orders.";
        }

        internal static AccountInfo<Wallet> Parse(Newtonsoft.Json.Linq.JObject returnObj)
        {
            JObject balancesAvailable = (JObject)returnObj["balances_available"];
            JObject balancesHold = (JObject)returnObj["balances_hold"];
            string serverDateTimeStr = returnObj["serverdatetime"].ToString();
            int openOrderCount = int.Parse(returnObj["openordercount"].ToString());
            TimeZoneInfo serverTimeZone = TimeZoneResolver.GetByShortCode(returnObj["servertimezone"].ToString());
            DateTime serverDateTime = DateTime.Parse(serverDateTimeStr);

            serverDateTime = TimeZoneInfo.ConvertTimeToUtc(serverDateTime, serverTimeZone);

            List<Wallet> wallets = ParseWallets(balancesAvailable, balancesHold);

            return new CryptsyAccountInfo(wallets, serverDateTime,
                serverTimeZone, openOrderCount);
        }

        private static List<Wallet> ParseWallets(JObject balancesAvailable, JObject balancesHold)
        {
            List<Wallet> wallets = new List<Wallet>();
            foreach (JProperty balanceAvailable in balancesAvailable.Properties())
            {
                JProperty balanceHeld = balancesHold.Property(balanceAvailable.Name);
                Wallet wallet = new Wallet(balanceAvailable.Name,
                    decimal.Parse(balanceAvailable.Value.ToString()), decimal.Parse(balanceHeld.Value.ToString()));
                wallets.Add(wallet);
            }
            return wallets;
        }

        public int OpenOrderCount { get { return this.openOrderCount;  } }
        public TimeZoneInfo ServerTimeZone { get { return this.serverTimeZone; } }
    }
}
