using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.Model
{
    public class Wallet
    {
        public Wallet(string currencyCode, decimal setBalance, decimal setHeldBalance)
        {
            this.CurrencyCode = currencyCode;
            this.Balance = setBalance;
            this.HeldBalance = setHeldBalance;
        }

        public Wallet(string currencyCode, decimal setBalance)
        {
            this.CurrencyCode = currencyCode;
            this.Balance = setBalance;
            this.HeldBalance = null;
        }

        public override string ToString()
        {
            return Balance + " "
                + CurrencyCode;
        }


        public string CurrencyCode { get; private set; }
        public decimal Balance { get; private set; }
        public decimal? HeldBalance { get; private set; }
    }
}
