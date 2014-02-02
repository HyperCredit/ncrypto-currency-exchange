using Lostics.NCryptoExchange.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange
{
    /// <summary>
    /// Abstract exchange for use where requests are signed with an SHA512 key.
    /// </summary>
    public abstract class AbstractSha512Exchange : IExchange
    {
        public string GenerateSHA512Signature(FormUrlEncodedContent request)
        {
            HMAC digester = new HMACSHA512(this.PrivateKeyBytes);
            StringBuilder hex = new StringBuilder();
            byte[] requestBytes = System.Text.Encoding.ASCII.GetBytes(request.ReadAsStringAsync().Result);

            return BitConverter.ToString(digester.ComputeHash(requestBytes)).Replace("-", "").ToLower();
        }

        public void SignRequest(FormUrlEncodedContent request)
        {
            request.Headers.Add(this.SignHeader, GenerateSHA512Signature(request));
            request.Headers.Add(this.KeyHeader, this.PublicKey);
        }

        public abstract void Dispose();

        public abstract Task<AccountInfo> GetAccountInfo();

        public abstract Task<List<Market>> GetMarkets();

        public abstract Task<Book> GetMarketDepth(MarketId marketId);

        public abstract Task<List<MyOrder>> GetMyActiveOrders(MarketId marketId, int? limit);

        public abstract string GetNextNonce();
        public abstract string KeyHeader { get; }
        public abstract string SignHeader { get; }
        public abstract string Label { get; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public byte[] PrivateKeyBytes
        {
            get
            {
                return System.Text.Encoding.ASCII.GetBytes(this.PrivateKey);
            }
        }
    }
}
