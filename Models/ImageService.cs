using DromAutoTrader.Models.Interfaces;
using SQLiteNetExtensions.Attributes;

namespace DromAutoTrader.Models
{
    public class ImageService : IBaseModel
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }
        public string? Name { get; set; } = string.Empty;       
    }
}
