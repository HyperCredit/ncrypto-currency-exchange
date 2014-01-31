using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;

namespace Lostics.NCryptoExchange.VaultOfSatoshi
{
    public class VoSAccountInfo : AccountInfo
    {
        public VoSAccountInfo(List<Wallet> setWallets)
            : base(setWallets)
        {
        }

        public static VoSAccountInfo Parse(JObject accountInfoJson)
        {
            DateTime created = VoSParsers.ParseTime(accountInfoJson.Value<int>("created"));
            Dictionary<string, decimal> tradeFees = VoSParsers.ParseCurrencyValueObj(accountInfoJson.Value<JObject>("trade_fee"));
            Dictionary<string, decimal> monthlyVolume = VoSParsers.ParseCurrencyValueObj(accountInfoJson.Value<JObject>("monthly_volume"));
            List<Wallet> wallets = ParseWallets(accountInfoJson.Value<JObject>("wallets"));

            return new VoSAccountInfo(wallets)
            {
                AccountId = accountInfoJson.Value<string>("account_id"),
                Created = created,
                TradeFees = tradeFees,
                MonthlyVolume = monthlyVolume
            };
        }

        internal static List<Wallet> ParseWallets(JObject walletsJson)
        {
            List<Wallet> wallets = new List<Wallet>();
            foreach (JProperty wallet in walletsJson.Properties())
            {
                string currencyCode = wallet.Name;

                wallets.Add(new Wallet(currencyCode, VoSParsers.ParseCurrencyObject(wallet.Value<JObject>("balance"))));
            }
            return wallets;
        }

        public override string ToString()
        {
            return this.AccountId;
        }

        public DateTime Created { get; private set; }
        public string AccountId { get; private set; }

        public Dictionary<string, decimal> TradeFees { get; private set; }

        public Dictionary<string, decimal> MonthlyVolume { get; private set; }
    }
}
