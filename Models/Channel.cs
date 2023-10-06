using SQLite;
using SQLiteNetExtensions.Attributes;
using PrimaryKeyAttribute = SQLite.PrimaryKeyAttribute;

namespace DromAutoTrader.Models
{
    public class Channel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string? Name { get; set; }

        // Навигационное свойство для связи с ценами повышения
        [OneToOne]
        public TablePriceOfIncrease? PriceIncrease { get; set; }
    }
}
