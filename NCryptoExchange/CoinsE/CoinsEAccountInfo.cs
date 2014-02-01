using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;

namespace Lostics.NCryptoExchange.CoinsE
{
    public class CoinsEAccountInfo : AccountInfo
    {
        public CoinsEAccountInfo(List<Wallet> setWallets, DateTime setServerTime)
            : base(setWallets)
        {
            this.ServerTime = setServerTime;
        }

        public static AccountInfo Parse(JObject jsonObj)
        {
            DateTime systemTime = CoinsEParsers.ParseTime(jsonObj.Value<int>("systime"));
            List<Wallet> wallets = jsonObj.Value<JObject>("wallets").Properties().Select(
                 wallet => (Wallet)CoinsEWallet.Parse(wallet.Name, wallet.First.Value<JObject>())
             ).ToList();

            return new CoinsEAccountInfo(wallets, systemTime);
        }

        public DateTime ServerTime { get; private set; }
    }
}
