using DromAutoTrader.Models.Interfaces;

namespace DromAutoTrader.Models
{
    public class Channel : IBaseModel
    {
        public int Id { get; set; } // Уникальный идентификатор поставщика
        public string? Name { get; set; } = string.Empty;  // Название поставщика    
    }
}
