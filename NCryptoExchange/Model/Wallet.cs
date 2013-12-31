using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.Model
{
    public class Wallet
    {
        private readonly string currencyCode;
        private readonly decimal balance;
        private readonly decimal heldBalance;

        public Wallet(string currencyCode, decimal setBalance, decimal setHeldBalance)
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

        public decimal Balance
        { get { return this.balance; } }

        public string CurrencyCode
        { get { return this.currencyCode; } }

        public decimal HeldBalance
        { get { return this.heldBalance; } }
    }
}
