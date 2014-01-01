using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public class CryptsyMarket : Market<CryptsyMarketId>
    {
        public CryptsyMarket(CryptsyMarketId setMarketId, string baseCurrencyCode, string baseCurrencyName,
            string quoteCurrencyCode, string quoteCurrencyName, string label,
            decimal currentVolume, decimal lastTrade, decimal highTrade, decimal lowTrade, DateTime created)
            : base(setMarketId, baseCurrencyCode, baseCurrencyName, quoteCurrencyCode, quoteCurrencyName, label)
        {
            this.CurrentVolume = currentVolume;
            this.LastTrade = lastTrade;
            this.HighTrade = highTrade;
            this.LowTrade = lowTrade;
            this.Created = created;
        }

        public static CryptsyMarket Parse(JObject marketObj, TimeZoneInfo timeZone) {
            DateTime created = DateTime.Parse(marketObj.Value<string>("created"));

            TimeZoneInfo.ConvertTimeToUtc(created, timeZone);

            return new CryptsyMarket(new CryptsyMarketId(marketObj.Value<string>("marketid")),
                marketObj.Value<string>("primary_currency_code"), marketObj.Value<string>("primary_currency_name"),
                marketObj.Value<string>("secondary_currency_code"), marketObj.Value<string>("secondary_currency_name"),
                marketObj.Value<string>("label"),
                marketObj.Value<decimal>("current_volume"), marketObj.Value<decimal>("last_trade"),
                marketObj.Value<decimal>("high_trade"), marketObj.Value<decimal>("low_trade"),
                created
            );
        }

        public decimal CurrentVolume { get; private set; }
        public decimal LastTrade { get; private set; }
        public decimal HighTrade { get; private set; }
        public decimal LowTrade { get; private set; }
        public DateTime Created { get; private set; }
    }
}
