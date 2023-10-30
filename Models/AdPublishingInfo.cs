using System.ComponentModel.DataAnnotations.Schema;

namespace DromAutoTrader.Models
{
    public class AdPublishingInfo
    {
        public int? Id { get; set; }
        public decimal? InputPrice { get; set; }
        public decimal? OutputPrice { get; set; }
        public string? DatePublished { get; set; }
        public bool? IsArchived { get; set; }
        public string? Brand { get; set; } = string.Empty;
        public string? Artikul { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public decimal? PriceBuy { get; set; }
        public int? Count { get; set; }
        public string? KatalogName { get; set; } = string.Empty;
        public string? AdDescription { get; set; } = string.Empty;
        public string? ImagesPath { get; set; } = string.Empty;

        [NotMapped]
        public List<string>? ImagesPaths { get; set; }
    }
}


