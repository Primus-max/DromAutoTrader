using SQLite;
using SQLiteNetExtensions.Attributes;

namespace DromAutoTrader.Models
{
    public class Channel
    {
        public int Id { get; set; }
        public string? Name { get; set; } = null!;
        public string? Description { get; set; } = null!;


        // Навигационное свойство для связи с ценами повышения
        [OneToMany]
        public List<TablePriceOfIncrease>? PriceIncreases { get; set; }

        // Брэнды для этого канала
        [ManyToMany(typeof(BrandChannelMapping))]
        public List<Brand>? Brands { get; set; }

        [Ignore] // Используем атрибут Ignore, чтобы это свойство не сохранялось в базу данных
        public int BrandsCount => Brands?.Count ?? 0;
    }
}
