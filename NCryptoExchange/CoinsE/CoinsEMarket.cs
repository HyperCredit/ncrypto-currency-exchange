using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lostics.NCryptoExchange.CoinsE
{
    public class CoinsEMarket : Market<CoinsEMarketId>
    {
        public CoinsEMarket(CoinsEMarketId id, string baseCurrencyCode, string baseCurrencyName,
            string quoteCurrencyCode, string quoteCurrencyName, string label,
            MarketStatistics statistics, string status, decimal tradeFee)
            : base(id, baseCurrencyCode, baseCurrencyName, quoteCurrencyCode, quoteCurrencyName, label, statistics)
        {
            this.Status = status;
            this.TradeFee = tradeFee;
        }

        /// <summary>
        /// Parse market information from the market data API (as in https://www.coins-e.com/api/v2/markets/data/).
        /// </summary>
        /// <param name="coinShortCodeToLabel">A mapping from coin short codes to human readable labels</param>
        /// <param name="marketObj">The JSON object representing a market</param>
        /// <returns></returns>
        public static CoinsEMarket Parse(Dictionary<string, string> coinShortCodeToLabel, JObject marketObj)
        {
            MarketStatistics marketStats = ParseMarketStatistics(marketObj.Value<JObject>("marketstat"));

            return new CoinsEMarket(new CoinsEMarketId(marketObj.Value<string>("pair")),
                marketObj.Value<string>("c1"), coinShortCodeToLabel[marketObj.Value<string>("c1")],
                marketObj.Value<string>("c2"), coinShortCodeToLabel[marketObj.Value<string>("c2")],
                marketObj.Value<string>("pair"), marketStats,
                marketObj.Value<string>("status"), marketObj.Value<decimal>("trade_fee")
            );
        }

        private static MarketStatistics ParseMarketStatistics(JObject statisticsJson)
        {
            JObject twentyFourHours = statisticsJson.Value<JObject>("24h");

            return new MarketStatistics()
            {
                LastTrade = statisticsJson.Value<decimal>("ltp"),
                HighTrade = twentyFourHours.Value<decimal>("h"),
                LowTrade = twentyFourHours.Value<decimal>("l"),
                Volume24H = twentyFourHours.Value<decimal>("volume")
            };
        }

        public string Status { get; private set; }
        public decimal TradeFee { get; private set; }
    }
}
