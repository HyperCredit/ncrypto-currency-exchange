using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.Model
{
    public class Wallet
    {
        private readonly string currencyCode;
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

        public Wallet(string currencyCode, Quantity setBalance, Quantity setHeldBalance)
        {
            this.currencyCode = currencyCode;
            this.balance = setBalance;
            this.heldBalance = setHeldBalance;
        }

        public override string ToString()
        {
            return balance + " "
                + currencyCode;
        }
    }
}
