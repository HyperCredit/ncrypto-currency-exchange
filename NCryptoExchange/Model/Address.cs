using System;
namespace Lostics.NCryptoExchange.Model
{
    public class Address
    {
        public Address(string setAddress)
        {
            this.Value = setAddress;
        }

        public override string ToString()
        {
            return this.Value;
        }

        public static Address Parse(Newtonsoft.Json.Linq.JToken jToken)
        {
            if (jToken.Type != Newtonsoft.Json.Linq.JTokenType.String)
            {
                throw new ArgumentException("Cannot parse non-string types as addresses");
            }

            return new Address(jToken.ToString());
        }

        public string Value
        {
            get;
            private set;
        }
    }
}
