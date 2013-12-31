namespace Lostics.NCryptoExchange.Model
{
    public class MarketDepth
    {
        private readonly decimal price;
        private readonly decimal quantity;

        public MarketDepth(decimal setPrice, decimal setQuantity)
        {
            this.price = setPrice;
            this.quantity = setQuantity;
        }

        public decimal Price { get { return this.price; } }
        public decimal Quantity { get { return this.quantity;  } }
    }
}
