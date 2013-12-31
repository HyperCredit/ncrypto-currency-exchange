namespace Lostics.NCryptoExchange.Model
{
    public class MarketDepth
    {
        public MarketDepth(decimal setPrice, decimal setQuantity)
        {
            this.Price = setPrice;
            this.Quantity = setQuantity;
        }

        public decimal Price { get; private set; }
        public decimal Quantity { get; private set; }
    }
}
