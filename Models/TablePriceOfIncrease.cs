using SQLite;
using SQLiteNetExtensions.Attributes;

namespace DromAutoTrader.Models
{
    public class TablePriceOfIncrease
    {
        [SQLite.PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public int PriceIncrease { get; set; }

        // Внешний ключ для связи с каналом
        [ForeignKey(typeof(Channel))]
        public int ChannelId { get; set; }
    }
}
