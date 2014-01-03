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
        private readonly string status;
        private readonly decimal tradeFee;

        public CoinsEMarket(CoinsEMarketId id, string baseCurrencyCode, string baseCurrencyName,
            string quoteCurrencyCode, string quoteCurrencyName, string label,
            string status, decimal tradeFee) : base(id, baseCurrencyCode, baseCurrencyName, quoteCurrencyCode, quoteCurrencyName, label)
        {
            this.status = status;
            this.tradeFee = tradeFee;
        }

        public static CoinsEMarket Parse(JObject marketObj)
        {
            return new CoinsEMarket(new CoinsEMarketId(marketObj.Value<string>("pair")),
                marketObj.Value<string>("c1"), marketObj.Value<string>("coin1"),
                marketObj.Value<string>("c2"), marketObj.Value<string>("coin2"),
                marketObj.Value<string>("pair"),
                marketObj.Value<string>("status"), marketObj.Value<decimal>("trade_fee")
            );
        }
    }
}
