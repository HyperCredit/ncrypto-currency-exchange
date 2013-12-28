using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public class CryptsyAccountInfo : AccountInfo<Wallet>
    {
        private string servertimezone;
        private DateTime serverdatetime;
        private int openordercount;

        public CryptsyAccountInfo()
            : base(new List<Wallet>(), DateTime.Now)
        {
        }

        public CryptsyAccountInfo(List<Wallet> setWallets, DateTime setSystemTime) : base(setWallets, setSystemTime)
        {

        }

        internal static AccountInfo<Wallet> ParseJson(string json)
        {
            throw new NotImplementedException();
        }
    }
}
