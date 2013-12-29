using System;
namespace Lostics.NCryptoExchange.Model
{
    public class Address
    {
        private readonly string address;

        public string Value
        {
            get { return this.address; }
        }

        public Address(string setAddress)
        {
            this.address = setAddress;
        }

        public override string ToString()
        {
            return this.address;
        }

        public static Address Parse(Newtonsoft.Json.Linq.JToken jToken)
        {
            if (jToken.Type != Newtonsoft.Json.Linq.JTokenType.String)
            {
                throw new ArgumentException("Cannot parse non-string types as addresses");
            }

            return new Address(jToken.ToString());
        }
    }
}
