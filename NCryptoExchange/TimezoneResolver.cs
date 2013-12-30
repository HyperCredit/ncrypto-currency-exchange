using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange
{
    public class TimeZoneResolver
    {
        private static Dictionary<string, TimeZoneInfo> timezonesByShortCode = new Dictionary<string, TimeZoneInfo>()
        {
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
