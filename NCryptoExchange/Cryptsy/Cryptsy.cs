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
    public class Cryptsy : IExchange<CryptsyMarketId, CryptsyOrderId, Wallet>
    {
        public static const string PARAM_NONCE = "nonce";
        public static const string PARAM_ORDER_TYPE = "ordertype";
        public static const string PARAM_PRICE = "price";
        public static const string PARAM_QUANTITY = "quantity";
        public static const string HEADER_SIGN = "Sign";
        public static const string HEADER_KEY = "Key";

        private readonly string publicUrl;
        private readonly string privateUrl;
        private readonly string publicKey;
        private readonly string privateKey;

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

        public override string GetNextNonce()
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
