namespace Lostics.NCryptoExchange.Model
{
    public class MarketDepth
    {
        private readonly Price price;
        private readonly Price quantity;

        public MarketDepth(Price setPrice, Price setQuantity)
        {
            this.price = setPrice;
            this.quantity = setQuantity;
        }

        public Price Price { get { return this.price; } }
        public Price Quantity { get { return this.quantity;  } }
    }
}
