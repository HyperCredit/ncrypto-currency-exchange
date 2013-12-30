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
        private Price currentVolume;
        private Price lastTrade;
        private Price highTrade;
        private Price lowTrade;
        private DateTime created;

        public CryptsyMarket(CryptsyMarketId setMarketId, string baseCurrencyCode, string baseCurrencyName,
            string quoteCurrencyCode, string quoteCurrencyName, string label)
            : base(setMarketId, baseCurrencyCode, baseCurrencyName, quoteCurrencyCode, quoteCurrencyName, label)
        {

        }

        internal static CryptsyMarket Parse(JToken marketToken)
        {
            if (marketToken.Type != JTokenType.Object)
            {
                throw new CryptsyResponseException("Expected object, but found unexpected JSON token \""
                    + marketToken.Type + "\".");
            }

            JObject marketObj = (JObject)marketToken;

            CryptsyMarket market = new CryptsyMarket(new CryptsyMarketId(marketObj["marketid"].ToString()),
                marketObj["primary_currency_code"].ToString(), marketObj["primary_currency_name"].ToString(),
                marketObj["secondary_currency_code"].ToString(), marketObj["secondary_currency_name"].ToString(),
                marketObj["label"].ToString());

            market.currentVolume = Price.Parse(marketObj["current_volume"]);
            market.lastTrade = Price.Parse(marketObj["last_trade"]);
            market.highTrade = Price.Parse(marketObj["high_trade"]);
            market.lowTrade = Price.Parse(marketObj["low_trade"]);
            market.created = DateTime.Parse(marketObj["created"].ToString());

            return market;
        }
    }
}
