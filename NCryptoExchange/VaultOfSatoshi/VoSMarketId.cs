using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Lostics.NCryptoExchange.VaultOfSatoshi
{
    public sealed class VoSMarketId : AbstractStringBasedId, MarketId
    {
        public VoSMarketId(string baseCurrency, string quoteCurrency) : base(baseCurrency + "/" + quoteCurrency)
        {
            this.BaseCurrencyCode = baseCurrency;
            this.QuoteCurrencyCode = quoteCurrency;
        }

        public string BaseCurrencyCode { get; private set; }
        public string BaseCurrencyCodeParameter
        {
            get
            {
                return "order_currency" + "="
                    + Uri.EscapeUriString(BaseCurrencyCode);
            }
        }
        public string QuoteCurrencyCode { get; private set; }
        public string QuoteCurrencyCodeParameter
        {
            get
            {
                return "payment_currency" + "="
                    + Uri.EscapeUriString(QuoteCurrencyCode);
            }
        }
    }
}
