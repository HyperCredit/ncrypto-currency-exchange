namespace Lostics.NCryptoExchange.Model
{
    public class MarketDepth
    {
        private readonly Quantity price;
        private readonly Quantity quantity;

        public MarketDepth(Quantity setPrice, Quantity setQuantity)
        {
            this.price = setPrice;
            this.quantity = setQuantity;
        }

        public Quantity Price { get { return this.price; } }
        public Quantity Quantity { get { return this.quantity;  } }
    }
}
