using System;

namespace Lostics.NCryptoExchange.Model
{
    public class Transaction
    {
        private readonly string currencyCode;
        private readonly DateTime transactionPosted;
        private readonly TransactionType transactionType;
        private readonly Address address;
        private readonly decimal amountExFees;
        private readonly decimal fee;

        public Transaction(string currencyCode, DateTime transactionPosted, TransactionType transactionType,
            Address address, decimal amountExFees, decimal fee)
        {
            this.currencyCode = currencyCode;
            this.transactionPosted = transactionPosted;
            this.transactionType = transactionType;
            this.address = address;
            this.amountExFees = amountExFees;
            this.fee = fee;
        }

        public override string ToString()
        {
            return this.transactionType.ToString() + " "
                + this.amountExFees + " "
                + this.currencyCode + " to "
                + this.address + " at "
                + this.transactionPosted;
        }

        public string CurrencyCode { get { return this.currencyCode; } }
        public DateTime TransactionPosted { get { return this.transactionPosted; } }
        public TransactionType TransactionType { get { return this.transactionType; } }
        public Address Address { get { return this.address;  } }
        public decimal AmountExFees { get { return this.amountExFees; } }
        public decimal Fee { get { return this.fee;  } }
    }
}
