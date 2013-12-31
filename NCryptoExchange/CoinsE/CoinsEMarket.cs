using Lostics.NCryptoExchange.Model;
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
    }
}
