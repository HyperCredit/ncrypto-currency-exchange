using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.Model
{
    public class Wallet
    {
        private readonly Quantity balance;
        private readonly Quantity heldBalance;

        public Quantity Balance
        {
            get { return this.balance; }
        }

        public Quantity HeldBalance
        {
            get { return this.heldBalance; }
        }

        public Wallet(Quantity setBalance, Quantity setHeldBalance)
        {
            this.balance = setBalance;
            this.heldBalance = setHeldBalance;
        }
    }
}
