using System;

namespace Lostics.NCryptoExchange.Model
{
    public class Transaction
    {
        private readonly string currencyCode;
        private readonly DateTime transactionPosted;
        private readonly TransactionType transactionType;
        private readonly Address address;
        private readonly Price amountExFees;
        private readonly Price fee;

        public Transaction(string currencyCode, DateTime transactionPosted, TransactionType transactionType,
            Address address, Price amountExFees, Price fee)
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
        public Price AmountExFees { get { return this.amountExFees; } }
        public Price Fee { get { return this.fee;  } }
    }
}
