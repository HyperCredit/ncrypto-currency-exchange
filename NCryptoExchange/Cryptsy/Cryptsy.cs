using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Lostics.NCryptoExchange.Model;
using System.IO;

namespace Lostics.NCryptoExchange.Cryptsy
{
    public class Cryptsy : IExchange<CryptsyMarketId, CryptsyOrderId, Wallet>
    {
        public static const string PARAM_NONCE = "nonce";
        public static const string PARAM_ORDER_TYPE = "ordertype";
        public static const string PARAM_PRICE = "price";
        public static const string PARAM_QUANTITY = "quantity";

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

            string requestBody = await request.ReadAsStringAsync();

            HttpResponseMessage response = await client.PostAsync(privateUrl, request);

            // Get the response content.
            HttpContent responseContent = response.Content;

            // Get the stream of the content.
            String content;

            using (StreamReader reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
            {
                // Write the output.
                content = await reader.ReadToEndAsync();
            }

            return new Fees();
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
    }
}
