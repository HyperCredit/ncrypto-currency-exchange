using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Lostics.NCryptoExchange.VaultOfSatoshi
{
    public sealed class VoSMarketId : AbstractStringBasedId, MarketId
    {
        public const string PARAM_ORDER_CURRENCY = "order_currency";
        public const string PARAM_PAYMENT_CURRENCY = "payment_currency";

        public VoSMarketId(string baseCurrency, string quoteCurrency) : base(baseCurrency + "/" + quoteCurrency)
        {
            this.BaseCurrencyCode = baseCurrency;
            this.QuoteCurrencyCode = quoteCurrency;
        }

        public string BaseCurrencyCode { get; private set; }
        public KeyValuePair<string, string> BaseCurrencyCodeKeyValuePair
        {
            get
            {
                return new KeyValuePair<string, string>(PARAM_ORDER_CURRENCY, BaseCurrencyCode);
            }
        }
        public string BaseCurrencyCodeParameter
        {
            get
            {
                return PARAM_ORDER_CURRENCY + "="
                    + Uri.EscapeUriString(BaseCurrencyCode);
            }
        }
        public string QuoteCurrencyCode { get; private set; }
        public KeyValuePair<string, string> QuoteCurrencyCodeKeyValuePair
        {
            get
            {
                return new KeyValuePair<string, string>(PARAM_PAYMENT_CURRENCY, QuoteCurrencyCode);
            }
        }
        public string QuoteCurrencyCodeParameter
        {
            get
            {
                return PARAM_PAYMENT_CURRENCY + "="
                    + Uri.EscapeUriString(QuoteCurrencyCode);
            }
        }
    }
}
