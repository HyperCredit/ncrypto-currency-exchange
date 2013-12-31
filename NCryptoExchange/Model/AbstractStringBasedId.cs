
namespace Lostics.NCryptoExchange.Model
{
    public abstract class AbstractStringBasedId
    {
        public string Value { get; private set; }

        public AbstractStringBasedId(string setValue)
        {
            this.Value = setValue;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            AbstractStringBasedId other = (AbstractStringBasedId)obj;

            return other.Value.Equals(this.Value);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public override string ToString()
        {
            return this.Value;
        }
    }
}
