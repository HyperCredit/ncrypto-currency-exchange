using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.VaultOfSatoshi
{
    public class VoSCurrency : Currency
    {
        public VoSCurrency(string currencyCode, string label) : base(currencyCode, label)
        {

        }

        public static VoSCurrency Parse(JObject currencyJson)
        {
            return new VoSCurrency(currencyJson.Value<string>("code"), currencyJson.Value<string>("name"))
            {
                Precision = currencyJson.Value<int>("precision"),
                Virtual = currencyJson.Value<int>("virtual") > 0,
                Tradeable = currencyJson.Value<int>("tradeable") > 0
            };
        }

        public int Precision { get; private set; }
        public bool Virtual { get; private set; }
        public bool Tradeable { get; private set; }
    }
}
