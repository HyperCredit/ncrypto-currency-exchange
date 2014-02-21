using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.Kraken
{
    public class KrakenMarket : Market
    {
        public KrakenMarket(KrakenMarketId id, string baseCurrencyCode, string quoteCurrencyCode,
            MarketStatistics statistics)
            : base(id, baseCurrencyCode, quoteCurrencyCode, id.ToString(), statistics)
        {
        }

        public static List<Market> ParseMarkets(JObject marketsJson)
        {
            List<Market> markets = new List<Market>();

            foreach (JProperty baseProperty in marketsJson.Properties())
            {
                string baseCurrency = baseProperty.Name;
                JObject quoteCurrencies = baseProperty.Value as JObject;

                if (null != quoteCurrencies)
                {
                    foreach (JProperty quoteProperty in quoteCurrencies.Properties())
                    {
                        markets.Add(KrakenMarket.Parse(baseCurrency, quoteProperty));
                    }
                }
            }

            return markets;
        }

        /// <summary>
        /// Parse market information from the market data API (as in https://vircurex.com/api/get_info_for_currency.json).
        /// </summary>
        /// <param name="currencyShortCodeToLabel">A mapping from coin short codes to human readable labels</param>
        /// <param name="marketObj">The JSON object representing a market</param>
        /// <returns></returns>
        public static KrakenMarket Parse(string baseCurrencyCode, JProperty marketProperty)
        {
            JObject marketJson = (JObject)marketProperty.Value;
            MarketStatistics marketStats = new MarketStatistics()
            {
                LastTrade = marketJson.Value<decimal>("ltp"),
                Volume24HBase = marketJson.Value<decimal>("volume")
            };
            string quoteCurrencyCode = marketProperty.Name;
            KrakenMarketId marketId = new KrakenMarketId(baseCurrencyCode, quoteCurrencyCode);

            return new KrakenMarket(marketId, baseCurrencyCode, quoteCurrencyCode, marketStats);
        }
    }
}
