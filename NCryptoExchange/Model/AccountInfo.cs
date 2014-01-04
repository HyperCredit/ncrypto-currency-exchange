using System;
using System.Collections.Generic;

namespace Lostics.NCryptoExchange.Model
{
    public class AccountInfo
    {
        public List<Wallet> Wallets
        {
            get;
            private set;
        }

        public DateTime SystemTime
        {
            get;
            private set;
        }

        public      AccountInfo(List<Wallet> setWallets, DateTime setSystemTime) {
            this.Wallets = setWallets;
            this.SystemTime = setSystemTime;
        }
    }
}
