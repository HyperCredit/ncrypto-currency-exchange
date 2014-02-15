using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Lostics.NCryptoExchange;
using Lostics.NCryptoExchange.Cryptsy;
using Lostics.NCryptoExchange.Model;

namespace PanicSell
{
    /// <summary>
    /// Worked example which finds all non-BTC positions, and closes them at current
    /// bid price.
    /// </summary>
    public class PanicSell
    {
        public static void Main(string[] argv)
        {
            using (CryptsyExchange cryptsy = new CryptsyExchange())
            {
                // TODO: Add in API keys

                DoPanicSell(cryptsy);
            }
        }

        private static void DoPanicSell(CryptsyExchange cryptsy)
        {
            // Cancel all orders
            Task cancelTask = cryptsy.CancelAllOrders();

            Task<List<Market>> marketsTask = cryptsy.GetMarkets();
            Task<AccountInfo> accountInfoTask = cryptsy.GetAccountInfo();

            List<Wallet> nonZeroWalletsExcludingBtc = accountInfoTask.Result.Wallets
                .Where(wallet => wallet.CurrencyCode != "BTC" && wallet.Balance > 0.00000000m).ToList();
            List<string> currenciesWithPositions = nonZeroWalletsExcludingBtc
                .Select(wallet => wallet.CurrencyCode).ToList();

            // Filter markets to BTC crosses with wallets as above.
            List<Market> relevantMarkets = marketsTask.Result
                .Where(market => market.BaseCurrencyCode == "BTC"
                    && currenciesWithPositions.Contains(market.QuoteCurrencyCode))
                .ToList();

            // Wait until we've finished cancelling orders before we create new ones
            cancelTask.Wait();

            List<Task<OrderId>> flattenTasks = new List<Task<OrderId>>();

            foreach (Market market in relevantMarkets)
            {
                foreach (Wallet wallet in nonZeroWalletsExcludingBtc)
                {
                    if (wallet.CurrencyCode == market.QuoteCurrencyCode
                        && wallet.Balance > 0.0m)
                    {
                        // Start the flatten requests, but don't block waiting for any of
                        // them to complete, yet
                        flattenTasks.Add(ClosePosition(cryptsy, market, wallet));
                    }
                }
            }

            // Wait for all of the orders to be issued.
            foreach (Task<OrderId> task in flattenTasks)
            {
                task.Wait();
            }
        }

        private async static Task<OrderId> ClosePosition(CryptsyExchange cryptsy, Market market, Wallet wallet)
        {
            // Get the market depth to determine best available price
            Book book = await cryptsy.GetMarketDepth(market.MarketId);
            decimal price;

            if (book.Bids.Count > 0)
            {
                price = book.Bids[0].Price;
            }
            else if (book.Asks.Count > 0)
            {
                price = book.Asks[0].Price;
            }
            else
            {
                List<MarketTrade> recentTrades = await cryptsy.GetMarketTrades(market.MarketId);

                price = recentTrades[0].Price;
            }

            return await cryptsy.CreateOrder(market.MarketId, OrderType.Sell,
                wallet.Balance, price);
        }
    }
}
