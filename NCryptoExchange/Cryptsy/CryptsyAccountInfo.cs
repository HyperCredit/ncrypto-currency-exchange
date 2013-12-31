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
        public CryptsyAccountInfo(List<Wallet> setWallets, DateTime setSystemTime,
            TimeZoneInfo serverTimeZone, int openOrderCount)
            : base(setWallets, setSystemTime)
        {
            this.ServerTimeZone = serverTimeZone;
            this.OpenOrderCount = openOrderCount;
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
