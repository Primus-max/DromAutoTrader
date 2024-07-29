namespace DromAutoTrader.Prices
{
    public class PriceChannelMapping
    {
        public Price Price { get; set; }
        public List<Channel> SelectedChannels { get; set; }

        public PriceChannelMapping(Price price)
        {
            Price = price;
            SelectedChannels = new List<Channel>();
        }
    }
}
