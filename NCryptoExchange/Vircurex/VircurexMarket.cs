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
            string quoteCurrencyCode, string quoteCurrencyName, string label,
            MarketStatistics statistics)
            : base(id, baseCurrencyCode, baseCurrencyName, quoteCurrencyCode, quoteCurrencyName, label, statistics)
        {
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
            JObject marketJson = marketProperty.Value as JObject;
            MarketStatistics marketStats = new MarketStatistics()
            {
                LastTrade = marketJson.Value<decimal>("ltp"),
                Volume24HBase = marketJson.Value<decimal>("volume")
            };
            string quoteCurrencyCode = marketProperty.Name;
            string pair = baseCurrencyCode + "/"
                + quoteCurrencyCode;

            return new VircurexMarket(new VircurexMarketId(pair),
                baseCurrencyCode, currencyShortCodeToLabel[baseCurrencyCode],
                quoteCurrencyCode, currencyShortCodeToLabel[quoteCurrencyCode],
                pair, marketStats
            );
        }
    }
}
