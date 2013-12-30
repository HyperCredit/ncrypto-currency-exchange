using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange.Cryptsy
{
    /// <summary>
    /// Translates timezone short codes provided by Cryptsy to timezone info.
    /// This is Cryptsy-specific as short-codes are geographical in nature
    /// (for example EDT can be UTC-4 or UTC-11 depending on which continent it
    /// refers to).
    /// </summary>
    public class TimeZoneResolver
    {
        private static Dictionary<string, TimeZoneInfo> timezonesByShortCode = new Dictionary<string, TimeZoneInfo>()
        {
            { "CST" , TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time") },
            { "EST" , TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time") },
            { "PST" , TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time") },
            { "UTC" , TimeZoneInfo.FindSystemTimeZoneById("UTC") }
        };

        public static TimeZoneInfo GetByShortCode(string shortCode)
        {
            return timezonesByShortCode[shortCode];
        }
    }
}
