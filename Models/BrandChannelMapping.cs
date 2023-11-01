namespace DromAutoTrader.Models
{
    public class BrandChannelMapping
    {
        public int BrandId { get; set; }
        public Brand? Brand { get; set; }

        public int ChannelId { get; set; }
        public Channel? Channel { get; set; }
    }

}
