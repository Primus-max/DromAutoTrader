using DromAutoTrader.Models.Interfaces;
using SQLiteNetExtensions.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace DromAutoTrader.Models
{
    public class Brand : IBaseModel
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? FindImageService { get; set; } = string.Empty;
        public string? DefaultImage { get; set; } = string.Empty;

        [SQLiteNetExtensions.Attributes.ForeignKey(typeof(Channel))]
        public int? ChannelId { get; set; }

        [ManyToMany(typeof(BrandImageServiceMapping))] // Многие-ко-многим через промежуточную таблицу
        public List<ImageService>? ImageServices { get; set; }

        [NotMapped]
        public List<ImageServiceWithState> ImageServicesWithState { get; set; } = new List<ImageServiceWithState>();
    }
}
