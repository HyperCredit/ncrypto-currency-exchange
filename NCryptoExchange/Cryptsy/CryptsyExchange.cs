using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Lostics.NCryptoExchange.Model;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public class CryptsyExchange : IExchange<CryptsyMarketId, CryptsyOrderId, Wallet> , IDisposable
    {
        public const string HEADER_SIGN = "Sign";
        public const string HEADER_KEY = "Key";

        public const string METHOD_CANCEL_ORDER = "cancelorder";
        public const string METHOD_CANCEL_ALL_ORDERS = "cancelallorders";
        public const string METHOD_CANCEL_MARKET_ORDERS = "cancelmarketorder";
        public const string METHOD_CALCULATE_FEES = "calculatefees";
        public const string METHOD_GET_INFO = "getinfo";

        public const string PARAM_MARKET_ID = "marketid";
        public const string PARAM_METHOD = "method";
        public const string PARAM_NONCE = "nonce";
        public const string PARAM_ORDER_ID = "orderid";
        public const string PARAM_ORDER_TYPE = "ordertype";
        public const string PARAM_PRICE = "price";
        public const string PARAM_QUANTITY = "quantity";

        private HttpClient client = new HttpClient();
        private readonly string publicUrl = "http://pubapi.cryptsy.com/api.php";
        private readonly string privateUrl = "https://www.cryptsy.com/api";
        private readonly string publicKey;
        private readonly byte[] privateKey;

        public byte[] PrivateKey { get { return this.privateKey; } }

        public CryptsyExchange(string publicKey, string privateKey)
        {
            this.publicKey = publicKey;
            this.privateKey = System.Text.Encoding.ASCII.GetBytes(privateKey);
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

        public async Task CancelOrder(CryptsyOrderId orderId)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(PARAM_METHOD, METHOD_CANCEL_ORDER),
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce()),
                new KeyValuePair<string, string>(PARAM_ORDER_ID, orderId.ToString())
            });
            string requestBody = await request.ReadAsStringAsync();

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            JObject jsonObj = await GetResponseAsJObject(response);

            AssertSuccess(jsonObj);

            return;
        }

        public async Task CancelAllOrders()
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(PARAM_METHOD, METHOD_CANCEL_ALL_ORDERS),
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce())
            });
            string requestBody = await request.ReadAsStringAsync();

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            JObject jsonObj = await GetResponseAsJObject(response);

            AssertSuccess(jsonObj);

            return;
        }

        public async Task CancelMarketOrders(CryptsyMarketId marketId)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(PARAM_METHOD, METHOD_CANCEL_MARKET_ORDERS),
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce()),
                new KeyValuePair<string, string>(PARAM_MARKET_ID, marketId.ToString())
            });
            string requestBody = await request.ReadAsStringAsync();

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            JObject jsonObj = await GetResponseAsJObject(response);

            AssertSuccess(jsonObj);

            return;
        }

        public async Task<Fees> CalculateFees(OrderType orderType, Quantity quantity,
                Quantity price)
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(PARAM_METHOD, METHOD_CALCULATE_FEES),
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce()),
                new KeyValuePair<string, string>(PARAM_ORDER_TYPE, orderType.ToString()),
                new KeyValuePair<string, string>(PARAM_QUANTITY, quantity.ToString()),
                new KeyValuePair<string, string>(PARAM_PRICE, price.ToString())
            });

            await SignRequest(request);

            string requestBody = await request.ReadAsStringAsync();

            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            JObject jsonObj = await GetResponseAsJObject(response);

            AssertSuccess(jsonObj);

            JObject returnObj = (JObject)jsonObj["return"];

            return new Fees(Quantity.Parse(returnObj["fee"].ToString()),
                Quantity.Parse(returnObj["net"].ToString()));
        }

        public Task<CryptsyOrderId> CreateOrder(CryptsyMarketId marketId,
                OrderType orderType, Quantity quantity,
                Quantity price)
        {
            throw new NotImplementedException();
        }

        public void Dispose() {
            this.client.Dispose();
        }

        public async Task<AccountInfo<Wallet>> GetAccountInfo()
        {
            FormUrlEncodedContent request = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(PARAM_METHOD, METHOD_GET_INFO),
                new KeyValuePair<string, string>(PARAM_NONCE, GetNextNonce())
            });
            string requestBody = await request.ReadAsStringAsync();

            await SignRequest(request);
            HttpResponseMessage response = await client.PostAsync(privateUrl, request);
            JObject jsonObj = await GetResponseAsJObject(response);

            AssertSuccess(jsonObj);

            JObject returnObj = (JObject)jsonObj["return"];

            return CryptsyAccountInfo.Parse(returnObj);
        }

        public string GetNextNonce()
        {
            return DateTime.Now.Ticks.ToString();
        }

        public async Task<List<MarketOrder>> GetMarketOrders(CryptsyMarketId marketId)
        {
            throw new NotImplementedException();
        }

        public async Task<Address> GenerateNewAddress(string currencyCode)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Market<CryptsyMarketId>>> GetMarkets()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Transaction>> GetMyTransactons()
        {
            throw new NotImplementedException();
        }

        public async Task<List<MarketTrade>> GetMarketTrades(CryptsyMarketId marketId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<MyTrade>> GetMyTrades(CryptsyMarketId marketId, int? limit)
        {
            throw new NotImplementedException();
        }

        public async Task<List<MyTrade>> GetAllMyTrades(int? limit)
        {
            throw new NotImplementedException();
        }

        public async Task<List<MyOrder>> GetMyOrders(CryptsyMarketId marketId, int? limit)
        {
            throw new NotImplementedException();
        }

        public async Task<List<MyOrder>> GetAllMyOrders(int? limit)
        {
            throw new NotImplementedException();
        }

        public async Task<List<MarketDepth>> GetMarketDepth(CryptsyMarketId marketId)
        {
            throw new NotImplementedException();
        }

        private async Task<JObject> GetResponseAsJObject(HttpResponseMessage response)
        {
            JObject jsonObj;

            using (Stream jsonStream = await response.Content.ReadAsStreamAsync())
            {
                using (StreamReader jsonStreamReader = new StreamReader(jsonStream))
                {
                    jsonObj = JObject.Parse(await jsonStreamReader.ReadToEndAsync());
                }
            }

            return jsonObj;
        }

        public async Task<FormUrlEncodedContent> SignRequest(FormUrlEncodedContent request)
        {
            HMAC digester = new HMACSHA512(this.PrivateKey);
            StringBuilder hex = new StringBuilder();
            byte[] requestBytes = System.Text.Encoding.ASCII.GetBytes(await request.ReadAsStringAsync());

            request.Headers.Add(HEADER_SIGN, BitConverter.ToString(digester.ComputeHash(requestBytes)).Replace("-", "").ToLower());
            request.Headers.Add(HEADER_KEY, this.publicKey);

            return request;
        }
    }
}
