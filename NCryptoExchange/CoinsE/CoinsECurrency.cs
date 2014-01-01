using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Lostics.NCryptoExchange.CoinsE
{
    public class CoinsECurrency : Currency
    {
        public static CoinsECurrency Parse(JObject coinJson)
        {
                /* "confirmations": 4,
        "trade_fee_percent": "0.30",
        "trade_fee": "0.003",
        "status": "maintanance",
        "tier": 1,
        "name": "bitcoin",
        "block_time": 600,
        "withdrawal_fee": "0.00050000",
        "coin": "BTC",
        "confirmation_time": 2400,
        "folder_name": "bitcoin" */

            return null;
        }

        public int ConfirmationsRequired { get; private set; }
        public decimal TradeFeePercent { get; private set; }
        public decimal WithdrawalFeeAbsolute { get; private set; }
    }
}
