using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Lostics.NCryptoExchange.VaultOfSatoshi
{
    public sealed class VoSMarketId : AbstractIntBasedId, MarketId
    {
        public VoSMarketId(int id, string urlSlug) : base(id)
        {
            string[] parts = urlSlug.Split(new[] { '_' });

            if (parts.Length != 2)
            {
                throw new ArgumentException("Invalid market URL slug; expected two currency codes separated by the '_' character, but found \""
                    + urlSlug + "\".");
            }

            this.BaseCurrencyCode = parts[0].ToUpper();
            this.QuoteCurrencyCode = parts[1].ToUpper();

            this.UrlSlug = urlSlug;
        }

        public string BaseCurrencyCode { get; private set; }
        public string QuoteCurrencyCode { get; private set; }
        public string UrlSlug { get; private set; }
    }
}
