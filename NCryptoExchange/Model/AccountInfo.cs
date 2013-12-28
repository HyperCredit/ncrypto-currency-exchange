using System;
using System.Collections.Generic;

namespace Lostics.NCryptoExchange.Model
{
    public abstract class AccountInfo<W> where W: Wallet
    {
        private readonly List<W> wallets;
        private readonly DateTime systemTime;

        public List<W> Wallets
        {
            get { return this.wallets; }
        }

        public DateTime SystemTime
        {
            get { return this.systemTime; }
        }

        protected   AccountInfo(List<W> setWallets, DateTime setSystemTime) {
            this.wallets = setWallets;
            this.systemTime = setSystemTime;
        }
    }
}
