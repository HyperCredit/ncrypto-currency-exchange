using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;

namespace Lostics.NCryptoExchange.CoinsE
{
    public class CoinsEWallet : Wallet
    {
        public      CoinsEWallet(string currencyCode, decimal setBalance, decimal setHeldBalance,
            decimal setUnconfirmedBalance) : base(currencyCode, setBalance, setHeldBalance)
        {
            this.UnconfirmedBalance = setUnconfirmedBalance;
        }

        public static CoinsEWallet Parse(string currencyCode, JObject walletJson)
        {
            return new CoinsEWallet(currencyCode, walletJson.Value<decimal>("a"),
                walletJson.Value<decimal>("h"), walletJson.Value<decimal>("u"));
        }

        public decimal UnconfirmedBalance { get; private set; }
    }
}
