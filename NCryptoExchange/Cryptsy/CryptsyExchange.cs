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
using Newtonsoft.Json.Linq;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public class CryptsyExchange : IExchange<CryptsyMarketId, CryptsyOrderId, Wallet>
    {
        public const string HEADER_SIGN = "Sign";
        public const string HEADER_KEY = "Key";

        public const string METHOD_CALCULATE_FEES = "calculatefees";
        public const string METHOD_GET_INFO = "getinfo";

        public const string PARAM_METHOD = "method";
        public const string PARAM_NONCE = "nonce";
        public const string PARAM_ORDER_TYPE = "ordertype";
        public const string PARAM_PRICE = "price";
        public const string PARAM_QUANTITY = "quantity";

        private readonly string publicUrl = "http://pubapi.cryptsy.com/api.php";
        private readonly string privateUrl = "https://www.cryptsy.com/api";
        private readonly string publicKey;
        private readonly byte[] privateKey;

        public CryptsyExchange(string publicKey, string privateKey)
        {
            this.publicKey = publicKey;
            this.privateKey = System.Text.Encoding.ASCII.GetBytes(privateKey);
        }

        public async Task<Fees> CalculateFees(OrderType orderType, Quantity quantity,
                Quantity price)
        {
            HttpClient client = new HttpClient();
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(PARAM_METHOD, METHOD_CALCULATE_FEES),
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
            Fees fees;

            using (Stream jsonStream = await response.Content.ReadAsStreamAsync())
            {
                using (StreamReader jsonStreamReader = new StreamReader(jsonStream))
                {
                    JObject jsonObj = JObject.Parse(await jsonStreamReader.ReadToEndAsync());

                    AssertSuccess(jsonObj);

                    JObject returnObj = (JObject)jsonObj["return"];
                    fees = new Fees(Quantity.Parse(returnObj["fee"].ToString()),
                        Quantity.Parse(returnObj["net"].ToString()));
                }
            }

            return fees;
        }

        private void AssertSuccess(JObject jsonObj)
        {
            if (null == jsonObj["success"])
            {
                throw new CryptsyResponseException("No success value returned in response from Cryptsy.");
            }

            if (!(jsonObj["success"].ToString().Equals("1")))
            {
                throw new CryptsyFailureException("False success value returned in response from Cryptsy.");
            }
        }

        public async Task<FormUrlEncodedContent> GenerateAccountInfoRequest()
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(PARAM_METHOD, METHOD_GET_INFO),
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce())
            });

            string requestContent = await request.ReadAsStringAsync();

            request.Headers.Add(HEADER_SIGN, GenerateSignature(requestContent));
            request.Headers.Add(HEADER_KEY, this.publicKey);

            return request;
        }

        public string GenerateSignature(string request)
        {
            HMAC digester = new HMACSHA512(this.PrivateKey);
            StringBuilder hex = new StringBuilder();
            byte[] requestBytes = System.Text.Encoding.ASCII.GetBytes(request);

            return BitConverter.ToString(digester.ComputeHash(requestBytes)).Replace("-", "").ToLower();
        }

        public async Task<AccountInfo<Wallet>> GetAccountInfo()
        {
            return CryptsyAccountInfo.ParseJson(await GetAccountInfoRaw());
        }

        public async Task<string> GetAccountInfoRaw()
        {
            HttpClient client = new HttpClient();
            FormUrlEncodedContent request = await GenerateAccountInfoRequest();
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

        public byte[] PrivateKey { get { return this.privateKey; } }
    }
}
