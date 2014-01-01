using System;
using System.Collections.Generic;

namespace Lostics.NCryptoExchange.Model
{
    public abstract class AccountInfo
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

        protected   AccountInfo(List<Wallet> setWallets, DateTime setSystemTime) {
            this.Wallets = setWallets;
            this.SystemTime = setSystemTime;
        }
    }
}
