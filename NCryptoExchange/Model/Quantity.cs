using Newtonsoft.Json.Linq;
using System;

namespace Lostics.NCryptoExchange.Model
{
    public class Quantity : IComparable<Quantity>
    {
        private readonly long count;
        private readonly double tickSize;

        public long Count { get { return this.count;  } }
        public double TickSize { get { return this.tickSize; } }
        public double Value { get { return this.count * this.tickSize; } }

        public Quantity(long setCount, double setTickSize)
        {
            this.count = setCount;
            this.tickSize = setTickSize;
        }

        public Quantity(double setValue)
        {
            this.count = (long)Math.Round(setValue / Constants.DEFAULT_TICK_SIZE);
            this.tickSize = Constants.DEFAULT_TICK_SIZE;
        }

        public int CompareTo(Quantity other)
        {
            double otherValue = other.Value;
            double diff = this.Value - other.Value;
            double smallestTick = Math.Min(this.tickSize, other.tickSize);

            if (diff > smallestTick)
            {
                return 1;
            }
            else if (diff < -smallestTick)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Quantity))
            {
                return false;
            }

            Quantity other = (Quantity)obj;

            return Math.Abs(this.Value - other.Value) < Math.Min(this.tickSize, other.tickSize);
        }

        public override int GetHashCode()
        {
            return (int)this.count;
        }

        /// <summary>
        /// Parse a quantity from a string representation
        /// </summary>
        /// <param name="valueAsStr"></param>
        /// <returns></returns>
        /// <exception cref="System.FormatException">valueAsStr does not represent a number in a valid format.</exception>
        public static Quantity Parse(string valueAsStr)
        {
            double value = Double.Parse(valueAsStr);

            // Should derive tick size from formatted string

            return new Quantity(value);
        }

        public static Quantity Parse(JToken valueAsJson)
        {
            switch (valueAsJson.Type) {
                case JTokenType.Property:
                    JProperty property = (JProperty)valueAsJson;
                    return Parse(property.Value);
                case JTokenType.String:
                    string value = valueAsJson.ToString();
                    return Parse(value);
                default:
                    throw new ArgumentException("Expected property or string, found JSON token type \""
                        + valueAsJson.Type + "\".");
            }
        }

        public override string ToString()
        {
            return this.Value.ToString("F8");
        }
    }
}
