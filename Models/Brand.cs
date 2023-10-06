using DromAutoTrader.Models.Interfaces;

namespace DromAutoTrader.Models
{
    public class Brand : IBaseModel
    {
        public int Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? FindImageService { get; set; } = string.Empty;
        public string? DefaultImage { get; set; } = string.Empty;
    }
}
