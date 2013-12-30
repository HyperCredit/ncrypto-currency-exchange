using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.Model
{
    public class Wallet
    {
        private readonly string currencyCode;
        private readonly Price balance;
        private readonly Price heldBalance;

        public Price Balance
        {
            get { return this.balance; }
        }

        public Price HeldBalance
        {
            get { return this.heldBalance; }
        }

        public Wallet(string currencyCode, Price setBalance, Price setHeldBalance)
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
