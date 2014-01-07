
namespace Lostics.NCryptoExchange.Model
{
    public abstract class AbstractLongBasedId
    {
        public long Value { get; private set; }

        public AbstractLongBasedId(long setValue)
        {
            this.Value = setValue;
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            AbstractLongBasedId other = (AbstractLongBasedId)obj;

            return other.Value.Equals(this.Value);
        }

        public override int GetHashCode()
        {
            return (int)this.Value;
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
