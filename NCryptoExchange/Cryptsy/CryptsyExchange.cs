using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Lostics.NCryptoExchange.Model;
using System.Security.Cryptography;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public class CryptsyExchange : IExchange<CryptsyMarketId, CryptsyOrderId, Wallet>
    {
        public const string PARAM_NONCE = "nonce";
        public const string PARAM_ORDER_TYPE = "ordertype";
        public const string PARAM_PRICE = "price";
        public const string PARAM_QUANTITY = "quantity";
        public const string HEADER_SIGN = "Sign";
        public const string HEADER_KEY = "Key";

        private readonly string publicUrl = "http://pubapi.cryptsy.com/api.php";
        private readonly string privateUrl = "https://www.cryptsy.com/api";
        private readonly string publicKey;
        private readonly string privateKey;

        public CryptsyExchange(string publicKey, string privateKey)
        {
            this.publicKey = publicKey;
            this.privateKey = privateKey;
        }

        public async Task<Fees> CalculateFees(OrderType orderType, Quantity quantity,
                Quantity price)
        {
            HttpClient client = new HttpClient();
            FormUrlEncodedContent request = new FormUrlEncodedContent(new [] {
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce()),
                new KeyValuePair<string, string>(PARAM_ORDER_TYPE, orderType.ToString()),
                new KeyValuePair<string, string>(PARAM_QUANTITY, quantity.ToString()),
                new KeyValuePair<string, string>(PARAM_PRICE, price.ToString())
            });

            string requestContent = await request.ReadAsStringAsync();
            string signature = GenerateSignature(requestContent);

            request.Headers.Add(HEADER_SIGN, signature);
            request.Headers.Add(HEADER_KEY, this.publicKey);

            string requestBody = await request.ReadAsStringAsync();

            HttpResponseMessage response = await client.PostAsync(privateUrl, request);

            using (StreamReader reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                // Write the output.
                return Fees.ParseJson(await reader.ReadToEndAsync());
            }
        }

        private string GenerateSignature(string request)
        {
            HMAC digester = new HMACSHA512(this.PrivateKeyBytes);
            StringBuilder hex = new StringBuilder();
            byte[] requestBytes = System.Text.Encoding.ASCII.GetBytes(request);

            return BitConverter.ToString(requestBytes).Replace("-", "");
        }

        public async Task<AccountInfo<Wallet>> GetAccountInfo()
        {
            string json = await GetRawAccountInfo();

            return CryptsyAccountInfo.ParseJson(json);
        }

        public async Task<string> GetRawAccountInfo()
        {
            HttpClient client = new HttpClient();
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce())
            });

            string requestContent = await request.ReadAsStringAsync();
            string signature = GenerateSignature(requestContent);

            request.Headers.Add(HEADER_SIGN, signature);
            request.Headers.Add(HEADER_KEY, this.publicKey);

            string requestBody = await request.ReadAsStringAsync();

            HttpResponseMessage response = await client.PostAsync(privateUrl, request);

            using (StreamReader reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public string GetNextNonce()
        {
            return DateTime.Now.Ticks.ToString();
        }

        /* public List<MarketOrder> GetMarketOrders(CryptsyMarketId marketId)
        {

        }

        public Address GenerateNewAddress(string currencyCode)
        {

        } */

        protected string PrivateKey { get { return this.privateKey; } }
        protected byte[] PrivateKeyBytes { get { return System.Text.Encoding.ASCII.GetBytes(this.privateKey); } }
    }
}
