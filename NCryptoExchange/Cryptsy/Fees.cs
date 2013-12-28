using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json;
using System;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public class Fees
    {
        internal Quantity fee;
        internal Quantity net;

        internal Fees()
        {
            this.fee = null;
            this.net = null;
        }

        public Fees(Quantity setFee, Quantity setNet)
        {
            this.fee = setFee;
            this.net = setNet;
        }

        public Fees(double setFee, double setNet)
        {
            this.fee = new Quantity(setFee);
            this.net = new Quantity(setNet);
        }
    }

    public class FeesResponse : CryptsyResponse<Fees>
    {
        public FeesResponse()
            : base()
        {
            this.ReturnValue = new Fees();
        }
    }

    public class FeesResponseConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(FeesResponse));
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            FeesResponse response = new FeesResponse();
            string propertyName = null;

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                        if (propertyName.Equals("return"))
                        {
                            ReadJsonFees(reader, objectType, response, serializer);
                        }
                        break;
                    case JsonToken.PropertyName:
                        propertyName = reader.Value.ToString();
                        break;
                    case JsonToken.Comment:
                        break;
                    default:
                        propertyName = null;
                        break;
                }
            }

            return response;
        }

        public void ReadJsonFees(JsonReader reader, Type objectType, FeesResponse existingValue, JsonSerializer serializer)
        {
            string propertyName = null;

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.EndObject:
                        return;
                    case JsonToken.PropertyName:
                        propertyName = reader.Value.ToString();
                        break;
                    case JsonToken.Float:
                    case JsonToken.Integer:
                    case JsonToken.String:
                        if (null == propertyName)
                        {
                            // Throw an exception?
                        }
                        else if (propertyName.Equals("fee"))
                        {
                            existingValue.ReturnValue.fee = Quantity.Parse(reader.Value.ToString());
                        }
                        else if (propertyName.Equals("net"))
                        {
                            existingValue.ReturnValue.net = Quantity.Parse(reader.Value.ToString());
                        }
                        break;
                    case JsonToken.Comment:
                        break;
                    default:
                        propertyName = null;
                        break;
                }
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanWrite is false. The type will skip the converter.");
        }
    }
}
