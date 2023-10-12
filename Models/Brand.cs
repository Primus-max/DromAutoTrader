using DromAutoTrader.Models.Interfaces;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace DromAutoTrader.Models
{
    public class Brand : IBaseModel
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public List<string>? FindImageService { get; set; } = null!;
        public string? DefaultImage { get; set; } = string.Empty;

        [ForeignKey(typeof(Channel))]
        public int? ChannelId { get; set; }        
      
    }
}
