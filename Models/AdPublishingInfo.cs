using DromAutoTrader.Prices;

namespace DromAutoTrader.Models
{
    public class AdPublishingInfo: PublishedPrice
    {
        public string? AdDescription { get; set; } = string.Empty;
        public string? ImagesPaths { get; set; } = string.Empty;
    }
}


