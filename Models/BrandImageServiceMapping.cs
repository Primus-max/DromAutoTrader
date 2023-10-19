using SQLiteNetExtensions.Attributes;

namespace DromAutoTrader.Models
{
    public class BrandImageServiceMapping
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int? Id { get; set; }

        [ForeignKey(typeof(Brand))]
        public int? BrandId { get; set; }

        [ForeignKey(typeof(ImageService))]
        public int? ImageServiceId { get; set; }
    }
}
