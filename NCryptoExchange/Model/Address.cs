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
    }
}
