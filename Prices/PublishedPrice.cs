namespace DromAutoTrader.Prices
{
    public class PublishedPrice
    {
        public int Id { get; set; }
        public string? Brand { get; set; } = string.Empty;
        public string? Artikul { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public decimal PriceBuy { get; set; }
        public int Count { get; set; }
        public string? KatalogName { get; set; } = string.Empty;
    }
}
