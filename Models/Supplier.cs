using DromAutoTrader.Models.Interfaces;

namespace DromAutoTrader.Models
{
    public class Supplier: IBaseModel
    {
        public int Id { get; set; } // Уникальный идентификатор поставщика
        public string? Name { get; set; }  // Название поставщика       
    }

}
