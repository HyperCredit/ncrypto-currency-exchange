using System;
using System.Collections.Generic;

namespace Lostics.NCryptoExchange.Model
{
    public abstract class AccountInfo<W> where W: Wallet
    {
        public List<W> Wallets
        {
            get;
            private set;
        }

        public DateTime SystemTime
        {
            get;
            private set;
        }

        protected   AccountInfo(List<W> setWallets, DateTime setSystemTime) {
            this.Wallets = setWallets;
            this.SystemTime = setSystemTime;
        }
    }
}
