using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.Vircurex
{
    public class VircurexMarket : Market
    {
        public VircurexMarket(VircurexMarketId id, string baseCurrencyCode, string baseCurrencyName,
            string quoteCurrencyCode, string quoteCurrencyName,
            MarketStatistics statistics)
            : base(id, baseCurrencyCode, baseCurrencyName, quoteCurrencyCode, quoteCurrencyName,
                id.ToString(), statistics)
        {
        }

        public static List<Market> ParseMarkets(Dictionary<string, string> currencyShortCodeToLabel,
            JObject marketsJson)
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
                        markets.Add(VircurexMarket.Parse(currencyShortCodeToLabel, baseCurrency, quoteProperty));
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
        public static VircurexMarket Parse(Dictionary<string, string> currencyShortCodeToLabel,
            string baseCurrencyCode, JProperty marketProperty)
        {
            JObject marketJson = (JObject)marketProperty.Value;
            MarketStatistics marketStats = new MarketStatistics()
            {
                LastTrade = marketJson.Value<decimal>("ltp"),
                Volume24HBase = marketJson.Value<decimal>("volume")
            };
            string quoteCurrencyCode = marketProperty.Name;
            VircurexMarketId marketId = new VircurexMarketId(baseCurrencyCode, quoteCurrencyCode);

            return new VircurexMarket(marketId,
                baseCurrencyCode, currencyShortCodeToLabel[baseCurrencyCode],
                quoteCurrencyCode, currencyShortCodeToLabel[quoteCurrencyCode],
                marketStats
            );
        }
    }
}
