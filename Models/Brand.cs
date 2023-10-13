using DromAutoTrader.Models.Interfaces;
using SQLiteNetExtensions.Attributes;

namespace DromAutoTrader.Models
{
    public class Brand : IBaseModel
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? FindImageService { get; set; } = string.Empty;
        public string? DefaultImage { get; set; } = string.Empty;

        [ForeignKey(typeof(Channel))]
        public int? ChannelId { get; set; }

        [OneToMany]
        public List<ImageService>? ImageServices { get; set; }
    }
}
