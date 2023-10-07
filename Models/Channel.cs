using SQLite;
using SQLiteNetExtensions.Attributes;
using PrimaryKeyAttribute = SQLite.PrimaryKeyAttribute;

namespace DromAutoTrader.Models
{
    public class Channel
    {        
        public int Id { get; set; }
        public string? Name { get; set; }

       
        // Навигационное свойство для связи с ценами повышения
        [OneToMany]
        public List<TablePriceOfIncrease>? PriceIncreases { get; set; }
    }
}
