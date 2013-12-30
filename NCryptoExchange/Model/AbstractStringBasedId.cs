
namespace Lostics.NCryptoExchange.Model
{
    public abstract class AbstractStringBasedId
    {
        private readonly string value;

        public string Value
        {
            get { return this.value; }
        }

        public AbstractStringBasedId(string setValue)
        {
            this.value = setValue;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            AbstractStringBasedId other = (AbstractStringBasedId)obj;

            return other.value.Equals(this.value);
        }

        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }

        public override string ToString()
        {
            return this.value;
        }
    }
}
