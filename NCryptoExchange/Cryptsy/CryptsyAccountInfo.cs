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
        private string servertimezone;
        private DateTime serverdatetime;
        private int openordercount;

        public CryptsyAccountInfo()
            : base(new List<Wallet>(), DateTime.Now)
        {
        }

        public CryptsyAccountInfo(List<Wallet> setWallets, DateTime setSystemTime) : base(setWallets, setSystemTime)
        {

        }

        internal static AccountInfo<Wallet> Parse(Newtonsoft.Json.Linq.JObject returnObj)
        {
            JObject balancesAvailable = (JObject)returnObj["balances_available"];
            JObject balancesHold = (JObject)returnObj["balances_hold"];
            string serverDateTime = returnObj["serverdatetime"].ToString();
            List<Wallet> wallets = new List<Wallet>();

            foreach (JProperty balanceAvailable in balancesAvailable.Properties())
            {
                JProperty balanceHeld = balancesHold.Property(balanceAvailable.Name);

                wallets.Add(new Wallet(balanceAvailable.Name,
                    Price.Parse(balanceAvailable), Price.Parse(balanceHeld)));
            }

            return new CryptsyAccountInfo(wallets, DateTime.Parse(serverDateTime));
        }
    }
}
