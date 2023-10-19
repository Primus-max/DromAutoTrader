using DromAutoTrader.Models.Interfaces;
using SQLite;
using System.ComponentModel.DataAnnotations.Schema;

namespace DromAutoTrader.Models
{
    public class ImageService : IBaseModel
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }
        public string? Name { get; set; } = string.Empty;

        [NotMapped]
        public bool IsSelected { get; set; }
    }
}
