using System;

namespace Lostics.NCryptoExchange.Model
{
    public class Transaction
    {
        public Transaction(string currencyCode, DateTime transactionPosted, TransactionType transactionType,
            Address address, decimal amountExFees, decimal fee)
        {
            this.CurrencyCode = currencyCode;
            this.TransactionPosted = transactionPosted;
            this.TransactionType = transactionType;
            this.Address = address;
            this.AmountExFees = amountExFees;
            this.Fee = fee;
        }

        public override string ToString()
        {
            return this.TransactionType.ToString() + " "
                + this.AmountExFees + " "
                + this.CurrencyCode + " to "
                + this.Address + " at "
                + this.TransactionPosted;
        }

        public string CurrencyCode { get; private set; }
        public DateTime TransactionPosted { get; private set; }
        public TransactionType TransactionType { get; private set; }
        public Address Address { get; private set; }
        public decimal AmountExFees { get; private set; }
        public decimal Fee { get; private set; }
    }
}
