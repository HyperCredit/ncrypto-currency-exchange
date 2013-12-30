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
        private string serverTimeZone;
        private int openOrderCount;

        public CryptsyAccountInfo(List<Wallet> setWallets, DateTime setSystemTime,
            string serverTimeZone, int openOrderCount) : base(setWallets, setSystemTime)
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
            string serverTimeZone = returnObj["servertimezone"].ToString();
            int openOrderCount = int.Parse(returnObj["openordercount"].ToString());
            List<Wallet> wallets = new List<Wallet>();

            foreach (JProperty balanceAvailable in balancesAvailable.Properties())
            {
                JProperty balanceHeld = balancesHold.Property(balanceAvailable.Name);

                wallets.Add(new Wallet(balanceAvailable.Name,
                    Price.Parse(balanceAvailable), Price.Parse(balanceHeld)));
            }

            return new CryptsyAccountInfo(wallets, DateTime.Parse(serverDateTimeStr),
                serverTimeZone, openOrderCount);
        }

        public int OpenOrderCount { get { return this.openOrderCount;  } }
        public string ServerTimeZone { get { return this.serverTimeZone; } }
    }
}
