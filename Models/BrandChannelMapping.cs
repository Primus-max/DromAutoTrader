namespace DromAutoTrader.Models
{
    public class BrandChannelMapping
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int? Id { get; set; }
        public int BrandId { get; set; }
        public Brand? Brand { get; set; }

        public int ChannelId { get; set; }
        public Channel? Channel { get; set; }
    }

}
