
namespace Lostics.NCryptoExchange.Model
{
    public abstract class AbstractIntBasedId
    {
        public int Value { get; private set; }

        public AbstractIntBasedId(int setValue)
        {
            this.Value = setValue;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            AbstractIntBasedId other = (AbstractIntBasedId)obj;

            return other.Value.Equals(this.Value);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
