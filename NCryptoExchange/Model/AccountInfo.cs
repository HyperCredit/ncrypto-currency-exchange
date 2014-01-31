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

        public      AccountInfo(List<Wallet> setWallets) {
            this.Wallets = setWallets;
        }
    }
}
